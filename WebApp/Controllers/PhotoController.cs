

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using network2.Models;
using WebApp.Services;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class PhotoController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationContext _db;
        private PhotoService _photoService;
        public PhotoController(ApplicationContext db, IWebHostEnvironment iWebHostEnvironment)
        {
            _db = db;
            _photoService = new PhotoService(_db);
        }

        [HttpGet]
        public List<Comment> GetCommentsByPhotoId(int photoId)
        {
            PhotoModel thisPhoto = _db.PhotoModels.FirstOrDefault(pm => pm.Id == photoId);
            return _photoService.BuildCommentWithUser(_db.CommentModels.Where(cm => thisPhoto.Comments.Contains(cm.Id)).OrderByDescending(cm => cm.Time).ToList());
        }

        [HttpGet]
        public bool IsLikedByUser(int photoId)
        {
            UserModel user = _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name);
            return _db.PhotoModels.FirstOrDefault(pm => pm.Id == photoId).LikeUsers.Contains(user.Id);
        }

        [HttpPost]
        public void LikeButtonResponse(int photoId)
        {
            UserModel me = _db.UserModels.FirstOrDefault(um =>  um.Nickname == User.Identity.Name);
            UserModel receiver = _db.UserModels.FirstOrDefault(um => um.Id == _db.PhotoModels.FirstOrDefault( um => um.Id == photoId).OwnerId);
            if (!_db.PhotoModels.FirstOrDefault(pm => pm.Id == photoId).LikeUsers.Contains(me.Id))
            {
                _db.PhotoModels.FirstOrDefault(pm => pm.Id == photoId).LikeUsers
                    .Add(me.Id);
                _db.PhotoModels.FirstOrDefault(pm => pm.Id == photoId).Rating++;
                if(_db.NotificationModels.
                    FirstOrDefault(nm => nm.SenderId == me.Id 
                                         && nm.TargetType == "photo" 
                                         && nm.TargetId == photoId 
                                         && nm.Type == "liked by user") == null)
                    if (receiver != me)
                    {
                        _db.NotificationModels.Add(
                            NotificationTemplates.PublicationIsLikedByUser
                                (me, receiver, "photo", photoId));
                    }
            }
            else
            {
                _db.PhotoModels.FirstOrDefault(pm => pm.Id == photoId).LikeUsers
                    .Remove(me.Id);
                _db.PhotoModels.FirstOrDefault(pm => pm.Id == photoId).Rating--;
            }

            _db.SaveChanges();
        }
        [HttpGet]
        public Comment SaveAndReturnComment(string text, int photoId)
        {
            DateTime now = DateTime.Now;
            CommentModel commentModel = new CommentModel
            {
                Text = text, 
                Time = now, 
                OwnerId = _db.UserModels.FirstOrDefault( um => um.Nickname == User.Identity.Name).Id, 
                PostId = photoId,
            };
            _db.CommentModels.Add(commentModel);
            _db.SaveChanges();
            {
                UserModel me = _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name);
                UserModel notificationReceiver =
                    _db.UserModels.FirstOrDefault(um =>
                        um.Id == _db.PhotoModels.FirstOrDefault(pm => pm.Id == photoId).OwnerId);
                if (_db.NotificationModels.FirstOrDefault(nm =>
                    nm.SenderId == me.Id 
                    && nm.TargetType == "photo" 
                    && nm.TargetId == photoId 
                    && nm.Type == "comment left by user") == null)
                    if (notificationReceiver != me)
                        _db.NotificationModels.Add(
                            NotificationTemplates.CommentLeftByUser
                                (me, notificationReceiver, "photo", photoId));
            }

            _db.PhotoModels.FirstOrDefault(pm => pm.Id == photoId).Comments
                .Add(commentModel.Id);
            _db.PhotoModels.FirstOrDefault(pm => pm.Id == photoId).CommentQuantity 
                = _db.PhotoModels.FirstOrDefault(pm => pm.Id == photoId).Comments.Count;

            _db.SaveChanges();
            Comment comment = new Comment()
            {
                Text = text, 
                Time = DateTime.Now, 
                OwnerId = _db.UserModels.FirstOrDefault( um => um.Nickname == User.Identity.Name).Id, 
                PostId = photoId,
                Owner = Mappers.BuildUser(_db.UserModels.FirstOrDefault(um => um.Id == commentModel.OwnerId))
            };
            return comment;
            }

        [HttpPost]
        public void ChangeAvatarByPhotoId(int photoId)
        {
            _db.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name).AvatarPath =
                _db.PhotoModels.FirstOrDefault(pm => pm.Id == photoId).Path;
            _db.SaveChanges();
        }
    }
}