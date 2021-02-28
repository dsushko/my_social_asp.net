using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using network2.Models;


namespace WebApp.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationContext _db;
        private IWebHostEnvironment _appEnvironment;
        
        public AccountController(ApplicationContext context, IWebHostEnvironment iWebHostEnvironment)
        {
            _db = context;
            _appEnvironment = iWebHostEnvironment;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        
        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                UserModel user1 = await _db.UserModels.FirstOrDefaultAsync(u => u.Email == model.Email);
                UserModel user2 = await _db.UserModels.FirstOrDefaultAsync(u => u.Nickname == model.Nickname);
                if (user1 == null && user2 == null)
                {
                    UserModel reguser = new UserModel
                    {
                        Email = model.Email, 
                        Password = model.Password, 
                        Nickname = model.Nickname, 
                        AvatarPath = "/img/avatar-default.png",
                        Bday = model.Bday,
                        Name = model.Name,
                        Surname = model.Surname,
                        Friends = new List<int>(),
                        Subscribers = new List<int>(),
                        OutputRequests = new List<int>(),
                        InputRequests = new List<int>(),
                        Songs = new List<int>(),
                        Photos = new List<int>(),
                        Notifications = new List<int>()
                    };
                    // добавляем пользователя в бд
                    _db.UserModels.Add(reguser);
                    await _db.SaveChangesAsync();

                    await Authenticate(model.Nickname); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                UserModel user = await _db.UserModels.FirstOrDefaultAsync(u => u.Nickname == model.Nickname && u.Password == model.Password);
                if (user != null)
                {
                    await Authenticate(model.Nickname); // аутентификация

                    return RedirectToAction("Index", "User", new {id = user.Id});
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }
        
        private async Task Authenticate(string userName)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
        
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}