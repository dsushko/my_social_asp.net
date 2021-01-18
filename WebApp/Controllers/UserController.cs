using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using network2.Models;
using WebApp.Services;
using WebApp.ViewModels;

namespace network2.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IWebHostEnvironment _appEnvironment;
        private readonly ApplicationContext _db;
        private UserService _userService;
        
        public UserController(ApplicationContext context, IWebHostEnvironment iWebHostEnvironment)
        {
            _db = context;
            _appEnvironment = iWebHostEnvironment;
            _userService = new UserService(_db);
        }

        public async Task<IActionResult> Index(int id)
        {
                var user = await _db.UserModels.FirstOrDefaultAsync(u => u.Id == id).ConfigureAwait(true);
                var u = Mappers.BuildUser(user);
                return View(u);
            
        }
        public IActionResult IndexByName(string name)
        {
            var user = _db.UserModels.FirstOrDefault(um => um.Nickname == name);
            return RedirectToAction("Index","User", new { id= user.Id});
            
        }
        public async Task<IActionResult> Music(int id)
        {    
            var user = await _db.UserModels.FirstOrDefaultAsync(u => u.Id == id).ConfigureAwait(true);
            var u = Mappers.BuildUser(user);
            return View(u);
        }
        public async Task<IActionResult> Friends(int id)
        {
            var user = await _db.UserModels.FirstOrDefaultAsync(u => u.Id == id).ConfigureAwait(true);
            var u = Mappers.BuildUser(user);
            return View(u);
        }
        public async Task<IActionResult> Photos(int id)
        {
            var user = await _db.UserModels.FirstOrDefaultAsync(u => u.Id == id).ConfigureAwait(true);
            var u = Mappers.BuildUser(user);
            return View(u);
        }

        public User GetUserById(int id)
        {
            return _userService.GetUserById(id);
        }
        
        public UserModel GetUserModelById(int id)
        {
            return _db.UserModels.FirstOrDefault(um => um.Id == id);
        }
        
        public void FriendRequestButtonResponse(int id)
        {
            UserModel me = _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name);
            if(me.OutputRequests.Contains(id))
            {
                WithdrawFriendRequest(id);

            }
            else if (_db.UserModels.FirstOrDefault(um => um.Id == id).OutputRequests.Contains(me.Id))
            {
                AcceptFriendRequest(id);

            }
            else if (me.Friends.Contains(id))
            {
                RemoveFriend(id);
            } else
            if(!me.OutputRequests.Contains(id))
            {
                SendFriendRequest(id);
            }

        }
        public void SendFriendRequest(int id)
        {
            UserModel me = _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name);
            _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).OutputRequests.Add(id);
            _db.UserModels.FirstOrDefault(um => um.Id == id).InputRequests
                .Add(me.Id);
            _db.UserModels.FirstOrDefault(um => um.Id == id).Subscribers
                .Add(me.Id);
            _db.SaveChanges();
        }
        public void WithdrawFriendRequest(int id)
        {
            UserModel me = _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name);
            _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).OutputRequests.Remove(id);
            _db.UserModels.FirstOrDefault(um => um.Id == id).InputRequests
                .Remove(me.Id);
            _db.UserModels.FirstOrDefault(um => um.Id == id).Subscribers
                .Remove(me.Id);
            _db.SaveChanges();
        }
        public void AcceptFriendRequest(int id)
        {
            _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).InputRequests.Remove(id);
            _db.UserModels.FirstOrDefault(um => um.Id == id).OutputRequests.Remove(_db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).Id);
            
            _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).Friends.Add(id);
            _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).Subscribers.Remove(id);
            _db.UserModels.FirstOrDefault(um => um.Id == id).Friends.Add(_db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).Id);
            _db.SaveChanges();

        }
        public void RemoveFriend(int id) //id того кого кикаем из друзей
        {
            _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).Friends.Remove(id);
            _db.UserModels.FirstOrDefault(um => um.Id == id).Friends.Remove(_db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).Id);

            _db.UserModels.FirstOrDefault(um => um.Id == id).OutputRequests.Add(_db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).Id);
            _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).InputRequests.Add(id);
            _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).Subscribers.Add(id);
            
            _db.SaveChanges();
        }

        public void DenyFriendRequest(int id)
        {
            _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).InputRequests.Remove(id);
            _db.SaveChanges();
        }

        [HttpGet]
        public string CheckFriendStatus(int id)
        {
            UserModel me = _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name);
            if (me.Friends.Contains(id))
            {
                return "friends";
            }
            if (me.OutputRequests.Contains(id))
            {
                return "output";
            }
            if (me.InputRequests.Contains(id))
            {
                return "input";
            }
            if (me.Subscribers.Contains(id))
            {
                return "subscriber";
            }
            return "none";

        }


        [HttpGet]
        public List<User> GetFriendsByUserId(int id)
        {
            if (id != 0)
            {
                List<User> res = new List<User>();
                foreach (var fId in _db.UserModels.FirstOrDefault(um => um.Id == id).Friends)
                {
                    res.Add(_userService.GetUserById(fId));
                }

                return res;
            }

            return new List<User>();
            
        }
        public List<User> GetSubscribersByUserId(int id)
                 {
                     if (id != 0)
                     {
                         List<User> res = new List<User>();
                         foreach (var fId in _db.UserModels.FirstOrDefault(um => um.Id == id).Subscribers)
                         {
                             res.Add(_userService.GetUserById(fId));
                         }
         
                         return res;
                     }
         
                     return new List<User>();
                 }
        public List<User> GetInputRequestsByUserId(int id)
        {
            if (id != 0)
            {
                List<User> res = new List<User>();
                foreach (var fId in _db.UserModels.FirstOrDefault(um => um.Id == id).InputRequests)
                {
                    res.Add(_userService.GetUserById(fId));
                }
         
                return res;
            }
         
            return new List<User>();
        }
        public List<User> GetOutputRequestsByUserId(int id)
        {
            if (id != 0)
            {
                List<User> res = new List<User>();
                foreach (var fId in _db.UserModels.FirstOrDefault(um => um.Id == id).OutputRequests)
                {
                    res.Add(_userService.GetUserById(fId));
                }
         
                return res;
            }
         
            return new List<User>();
        }
    }
}