using System.Collections.Generic;
using System.Linq;
using network2.Models;
using WebApp.ViewModels;

namespace WebApp.Services
{
    public class PhotoService
    {
        
        private readonly ApplicationContext _db;

        public PhotoService(ApplicationContext db)
        {
            _db = db;
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