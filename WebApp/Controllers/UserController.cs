using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using network2.Models;
using TagLib.Riff;
using WebApp.Services;
using WebApp.ViewModels;

namespace WebApp.Controllers
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
                var currUser = await _db.UserModels.FirstOrDefaultAsync(um => um.Nickname == User.Identity.Name);
                var cu = Mappers.BuildUser(currUser);
                return View(new UserWithLogged()
                {
                    User = u,
                    Logged = cu,
                });
        }
        public async Task<IActionResult> Music(int id)
        {    
            var user = await _db.UserModels.FirstOrDefaultAsync(u => u.Id == id).ConfigureAwait(true);
            var u = Mappers.BuildUser(user);
            var currUser = await _db.UserModels.FirstOrDefaultAsync(um => um.Nickname == User.Identity.Name);
            var cu = Mappers.BuildUser(currUser);
            return View(new UserWithLogged()
            {
                User = u,
                Logged = cu,
            });
        }
        public async Task<IActionResult> Friends(int id)
        {
            var user = await _db.UserModels.FirstOrDefaultAsync(u => u.Id == id).ConfigureAwait(true);
            var u = Mappers.BuildUser(user);
            var currUser = await _db.UserModels.FirstOrDefaultAsync(um => um.Nickname == User.Identity.Name);
            var cu = Mappers.BuildUser(currUser);
            return View(new UserWithLogged()
            {
                User = u,
                Logged = cu,
            });
        }
        public async Task<IActionResult> Photos(int id)
        {
            var user = await _db.UserModels.FirstOrDefaultAsync(u => u.Id == id).ConfigureAwait(true);
            var u = Mappers.BuildUser(user);
            var currUser = await _db.UserModels.FirstOrDefaultAsync(um => um.Nickname == User.Identity.Name);
            var cu = Mappers.BuildUser(currUser);
            return View(new UserWithLogged()
            {
                User = u,
                Logged = cu,
            });
        }

        public User GetUserById(int userId)
        {
            return _userService.GetUserById(userId);
        }
        
        public UserModel GetUserModelById(int userId)
        {
            return _db.UserModels.FirstOrDefault(um => um.Id == userId);
        }
        
        public void FriendRequestButtonResponse(int userId)
        {
            UserModel me = _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name);
            if(me.OutputRequests.Contains(userId))
            {
                WithdrawFriendRequest(userId);

            }
            else if (_db.UserModels.FirstOrDefault(um => um.Id == userId).OutputRequests.Contains(me.Id))
            {
                AcceptFriendRequest(userId);

            }
            else if (me.Friends.Contains(userId))
            {
                RemoveFriend(userId);
            } else
            if(!me.OutputRequests.Contains(userId))
            {
                SendFriendRequest(userId);
            }

        }
        public void SendFriendRequest(int userId)
        {
            UserModel me = _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name);
            _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).OutputRequests.Add(userId);
            _db.UserModels.FirstOrDefault(um => um.Id == userId).InputRequests
                .Add(me.Id);
            _db.UserModels.FirstOrDefault(um => um.Id == userId).Subscribers
                .Add(me.Id);
            _db.NotificationModels.Add(
                NotificationTemplates.FriendRequestIsSent(me, _db.UserModels.FirstOrDefault(um => um.Id == userId)));
            _db.SaveChanges();
        }
        public void WithdrawFriendRequest(int userId)
        {
            UserModel me = _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name);
            _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).OutputRequests.Remove(userId);
            _db.UserModels.FirstOrDefault(um => um.Id == userId).InputRequests
                .Remove(me.Id);
            _db.UserModels.FirstOrDefault(um => um.Id == userId).Subscribers
                .Remove(me.Id);
            _db.SaveChanges();
        }
        public void AcceptFriendRequest(int userId)
        {
            _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).InputRequests.Remove(userId);
            _db.UserModels.FirstOrDefault(um => um.Id == userId).OutputRequests.Remove(_db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).Id);
            
            _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).Friends.Add(userId);
            _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).Subscribers.Remove(userId);
            _db.UserModels.FirstOrDefault(um => um.Id == userId).Friends.Add(_db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).Id);
            _db.SaveChanges();

        }
        public void RemoveFriend(int userId) //id того кого кикаем из друзей
        {
            _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).Friends.Remove(userId);
            _db.UserModels.FirstOrDefault(um => um.Id == userId).Friends.Remove(_db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).Id);

            _db.UserModels.FirstOrDefault(um => um.Id == userId).OutputRequests.Add(_db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).Id);
            _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).InputRequests.Add(userId);
            _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).Subscribers.Add(userId);
            
            _db.SaveChanges();
        }

        public void DenyFriendRequest(int userId)
        {
            _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).InputRequests.Remove(userId);
            _db.SaveChanges();
        }
        [HttpGet]
        public string CheckFriendStatus(int userId)
        {
            UserModel me = _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name);
            if (me.Friends.Contains(userId))
            {
                return "friends";
            }
            if (me.OutputRequests.Contains(userId))
            {
                return "output";
            }
            if (me.InputRequests.Contains(userId))
            {
                return "input";
            }
            if (me.Subscribers.Contains(userId))
            {
                return "subscriber";
            }
            return "none";

        }
        [HttpGet]
        public List<User> GetFriendsByUserId(int userId)
        {
            if (userId != 0)
            {
                List<User> res = new List<User>();
                foreach (var fId in _db.UserModels.FirstOrDefault(um => um.Id == userId).Friends)
                {
                    res.Add(_userService.GetUserById(fId));
                }

                return res;
            }

            return new List<User>();
            
        }
        public List<User> GetSubscribersByUserId(int userId)
                 {
                     if (userId != 0)
                     {
                         List<User> res = new List<User>();
                         foreach (var fId in _db.UserModels.FirstOrDefault(um => um.Id == userId).Subscribers)
                         {
                             res.Add(_userService.GetUserById(fId));
                         }
         
                         return res;
                     }
         
                     return new List<User>();
                 }
        public List<User> GetInputRequestsByUserId(int userId)
        {
            if (userId != 0)
            {
                List<User> res = new List<User>();
                foreach (var fId in _db.UserModels.FirstOrDefault(um => um.Id == userId).InputRequests)
                {
                    res.Add(_userService.GetUserById(fId));
                }
         
                return res;
            }
         
            return new List<User>();
        }
        public List<User> GetOutputRequestsByUserId(int userId)
        {
            if (userId != 0)
            {
                List<User> res = new List<User>();
                foreach (var fId in _db.UserModels.FirstOrDefault(um => um.Id == userId).OutputRequests)
                {
                    res.Add(_userService.GetUserById(fId));
                }
         
                return res;
            }
         
            return new List<User>();
        }
        [HttpGet]
        public List<PhotoModel> GetPhotosByUserId(int userId)
        {
            return _db.PhotoModels.Where(u => u.OwnerId == userId).OrderByDescending(pm => pm.Id).ToList();
        }
        [HttpGet]
        public List<Notification> GetNotificationsByReceiverId(int userId)
        {
            List<NotificationModel> appropModels = new List<NotificationModel>();
            appropModels =  _db.NotificationModels.Where(nm => nm.ReceivingPersonId == userId).ToList();
            
            List<Notification> result = new List<Notification>();
            result = _db.NotificationModels.Where(nm => nm.ReceivingPersonId == userId).
                Select(Mappers.BuildNotification).ToList();
            
            foreach (var nm in appropModels)
            {
                result.Find(n => n.Id == nm.Id).SenderUser
                    = Mappers.BuildUser(_db.UserModels.FirstOrDefault(um => um.Id == nm.SenderId));
                if (nm.Type == "friend request is sent" || nm.Type == "friend request is accepted")
                {
                    result.Find(n => n.Id == nm.Id).TargetUser
                        = Mappers.BuildUser(_db.UserModels.FirstOrDefault(um => um.Id == nm.TargetId));
                }
            }
            return result;
        }
        [HttpGet]
        public List<Photo> GetNLastPhotosByUserId(int id, int n)
        {
            List<Photo> result = new List<Photo>();

            int listCount = _db.UserModels.FirstOrDefault(um => um.Id == id).Photos.Count;
            int quantityOfPhotosWillBeReturned = listCount > n ? n : listCount;
            for (int i = 0; i < quantityOfPhotosWillBeReturned; i++)
            {
                int photoId = (_db.UserModels.FirstOrDefault(um => um.Id == id).Photos)[i];
                result.Add(Mappers.BuildPhoto(_db.PhotoModels.FirstOrDefault(pm => pm.Id == photoId)));
            }
         
            return result.OrderByDescending(p => p.Id).ToList();
        }
    }
}