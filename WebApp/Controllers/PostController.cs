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
    public class PostController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationContext _appContext;
        private readonly PostService _postService;
        
        public PostController(ApplicationContext context, IWebHostEnvironment iWebHostEnvironment)
        {
            _appContext = context;
            _postService = new PostService(_appContext);
        }
        public List<Post> GetPostsByUserId(int id)
        {
            return _postService.GetPostsByUserId(id);
        }
        [HttpPost]
        public Post SaveAndReturnPost(string text)
        {
            PostModel postModel = new PostModel
            {
                Text = text, 
                Date = DateTime.Now, 
                OwnerId = _appContext.UserModels.FirstOrDefault( um => um.Nickname == User.Identity.Name).Id, 
                Rating = 0,
                Comments = new List<int>(),
                SharesPeople = new List<int>(),
                LikeUsers = new List<int>()
            };
            _appContext.PostModels.Add(postModel);
            _appContext.SaveChanges();
            Post post = new Post()
            {
                Date = postModel.Date,
                Id = postModel.Id,
                OwnerId = postModel.OwnerId,
                Text = postModel.Text,
                Owner = Mappers.BuildUser(_appContext.UserModels.FirstOrDefault(um => um.Id == postModel.OwnerId))
            };
            return post;
        }
        [HttpPost]
        public void DeletePost(int Id)
        {
            _appContext.PostModels.Remove(_appContext.PostModels.FirstOrDefault(pm => pm.Id == Id));
            _appContext.SaveChanges();
        }
        [HttpPost]
        public Comment SaveAndReturnComment(string text, int postId)
        {
            DateTime now = DateTime.Now;
            CommentModel commentModel = new CommentModel
            {
                Text = text, 
                Time = now, 
                OwnerId = _appContext.UserModels.FirstOrDefault( um => um.Nickname == User.Identity.Name).Id, 
                PostId = postId,
                Rating = 0,
            };
            _appContext.CommentModels.Add(commentModel);
            _appContext.SaveChanges();
            {
                UserModel me = _appContext.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name);
                UserModel notificationReceiver =
                    _appContext.UserModels.FirstOrDefault(um =>
                        um.Id == _appContext.PostModels.FirstOrDefault(pm => pm.Id == postId).OwnerId);
                if (_appContext.NotificationModels.FirstOrDefault(nm =>
                    nm.SenderId == me.Id && nm.TargetType == "post" && nm.TargetId == postId && nm.Type == "comment left by user") == null)
                    if (notificationReceiver != me)
                        _appContext.NotificationModels.Add(
                            NotificationTemplates.CommentLeftByUser(me, notificationReceiver, "post", postId));
            }

            _appContext.PostModels.FirstOrDefault(pm => pm.Id == postId).Comments
                .Add(commentModel.Id);
            _appContext.PostModels.FirstOrDefault(pm => pm.Id == postId).CommentQuantity 
                = _appContext.PostModels.FirstOrDefault(pm => pm.Id == postId).Comments.Count;
            _appContext.SaveChanges();
           
            Comment comment = new Comment()
            {
                Text = text, 
                Time = DateTime.Now, 
                OwnerId = _appContext.UserModels.FirstOrDefault( um => um.Nickname == User.Identity.Name).Id, 
                PostId = postId,
                Owner = Mappers.BuildUser(_appContext.UserModels.FirstOrDefault(um => um.Id == commentModel.OwnerId))
            };
            return comment;
        }
        public void CommentsCheckValidity(int postId)
        {
            foreach (var id in _appContext.PostModels.FirstOrDefault(pm => pm.Id == postId).Comments)
            {
                if (_appContext.CommentModels.FirstOrDefault(cm => cm.Id == id) == null)
                {
                    _appContext.PostModels.FirstOrDefault(pm => pm.Id == postId).Comments.Remove(id);
                }
            }
            _appContext.PostModels.FirstOrDefault(pm => pm.Id == postId).CommentQuantity 
                = _appContext.PostModels.FirstOrDefault(pm => pm.Id == postId).Comments.Count;

        }
        [HttpGet]
        public List<Comment> GetCommentsByPostId(int postId)
        {
            PostModel thisPhoto = _appContext.PostModels.FirstOrDefault(pm => pm.Id == postId);
            return _postService.BuildCommentWithUser(_appContext.CommentModels.Where(cm => thisPhoto.Comments.Contains(cm.Id)).OrderByDescending(cm => cm.Time).ToList());
        }
        [HttpPost]
        public void LikeButtonResponse(int id)
        {
            UserModel me = _appContext.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name);
            PostModel post = _appContext.PostModels.FirstOrDefault(pm => pm.Id == id);
            UserModel notificationReceiver = _appContext.UserModels.FirstOrDefault(um => um.Id == post.OwnerId);
            if (!_appContext.PostModels.FirstOrDefault(pm => pm.Id == id).LikeUsers.Contains(me.Id))
            {
                _appContext.PostModels.FirstOrDefault(pm => pm.Id == id).LikeUsers
                    .Add(me.Id);
                _appContext.PostModels.FirstOrDefault(pm => pm.Id == id).Rating++;
                if(_appContext.NotificationModels.FirstOrDefault(nm => nm.SenderId == me.Id && nm.TargetType == "post" && nm.TargetId == id && nm.Type == "liked by user") == null)
                    if(notificationReceiver != me)
                _appContext.NotificationModels.Add(
                    NotificationTemplates.PublicationIsLikedByUser(me, notificationReceiver, "post", id));
            }
            else
            {
                _appContext.PostModels.FirstOrDefault(pm => pm.Id == id).LikeUsers
                    .Remove(me.Id);
                _appContext.PostModels.FirstOrDefault(pm => pm.Id == id).Rating--;
            }

            _appContext.SaveChanges();
        }
        [HttpGet]
        public bool IsLikedByUser(int postId)
        {
            UserModel user = _appContext.UserModels.FirstOrDefault(um => um.Nickname == User.Identity.Name);
            return _appContext.PostModels.FirstOrDefault(pm => pm.Id == postId).LikeUsers.Contains(user.Id);
        }

        public Post GetPostById(int postId)
        {    
            Post result = Mappers.BuildPost(_appContext.PostModels.FirstOrDefault(pm => pm.Id == postId));
            result.Owner = Mappers.BuildUser(_appContext.UserModels.FirstOrDefault(um => um.Id == result.OwnerId));
            return result;
        }
    }
}