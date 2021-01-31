using System.Security.Cryptography;
using network2.Models;
using WebApp.ViewModels;

namespace WebApp.Services
{
    public static class Mappers
    {
        public static User BuildUser(UserModel um)
        {
            User u = new User
            {
                Nickname = um.Nickname,
                Name = um.Name,
                Surname = um.Surname,
                AvatarPath = um.AvatarPath,
                Bday = um.Bday,
                Id = um.Id
            };
            return u;
        }

        public static Post BuildPost(PostModel pm)
        {
            Post p = new Post()
            {
                Date = pm.Date,
                Id = pm.Id,
                OwnerId = pm.OwnerId,
                Rating = pm.Rating,
                Text = pm.Text,
                SharesQuantity = pm.SharesQuantity,
                CommentQuantity = pm.CommentQuantity
            };
            return p;
        }
        public static Comment BuildComment(CommentModel cm)
        {
            Comment c = new Comment()
            {
                OwnerId = cm.OwnerId,
                Rating =  cm.Rating,
                ReplyCommentId = cm.ReplyCommentId,
                Text = cm.Text,
                Time = cm.Time,
                Id = cm.Id,
            };
            return c;
        }

        public static Notification BuildNotification(NotificationModel nm)
        {
            Notification n = new Notification()
            {
                PicturePath = nm.PicturePath,
                Id = nm.Id,
                ReceivingPersonId = nm.ReceivingPersonId,
                SenderType = nm.SenderType, 
                SenderUser = new User(),
                TargetType = nm.TargetType,
                TargetUser = new User(),
                Text = nm.Text,
                Time = nm.Time
            };
            return n;
        }

        public static Photo BuildPhoto(PhotoModel pm)
        {
            Photo p = new Photo()
            {
                CommentQuantity = pm.CommentQuantity,
                Comments = pm.Comments,
                Id=pm.Id,
                LikeUsers = pm.LikeUsers,
                Owner = new User(),
                OwnerId = pm.OwnerId,
                Path = pm.Path,
                Rating = pm.Rating,
                Time = pm.Time
            };
            return p;
        }
    }
}