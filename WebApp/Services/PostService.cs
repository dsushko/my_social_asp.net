using System;
using System.Collections.Generic;
using System.Linq;
using network2.Models;
using WebApp.ViewModels;

namespace WebApp.Services
{
    public class PostService
    {
        private readonly ApplicationContext _db;

        public PostService(ApplicationContext db)
        {
            _db = db;
        }

        public List<Post> GetPostsByUserId(int id)
        {
            return BuildPostWithUser(_db.PostModels.Where(pm => pm.OwnerId == id).OrderByDescending(pm => pm.Date).ToList());
        }

        public List<Post> BuildPostWithUser(List<PostModel> models)
        {
            List<Post> posts = new List<Post>();
            foreach (var pm in models)
            {
                Post p = new Post()
                {
                    Date = pm.Date,
                    Id = pm.Id,
                    OwnerId = pm.OwnerId,
                    Rating = pm.Rating,
                    Text = pm.Text,
                    Owner = Mappers.BuildUser(_db.UserModels.FirstOrDefault(um => um.Id == pm.OwnerId)),
                    CommentQuantity = pm.CommentQuantity,
                    SharesQuantity = pm.SharesQuantity
                };
                posts.Add(p);
            }

            return posts;
        }
        public List<Comment> BuildCommentWithUser(List<CommentModel> models)
        {
            List<Comment> comments = new List<Comment>();
            foreach (var pm in models)
            {
                Comment p = new Comment()
                {
                    Time = pm.Time,
                    Id = pm.Id,
                    OwnerId = pm.OwnerId,
                    PostId = pm.PostId,
                    Rating = pm.Rating,
                    Text = pm.Text,
                    Owner = Mappers.BuildUser(_db.UserModels.FirstOrDefault(um => um.Id == pm.OwnerId))
                };
                comments.Add(p);
            }

            return comments;
        }
    }
}