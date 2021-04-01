using System;
using network2.Models;

namespace WebApp.Services
{
    public static class NotificationTemplates
    {
        public static NotificationModel FriendRequestIsSent(UserModel sender, UserModel reciever)
        {
            return new NotificationModel()
            {
                Text = " sent you a friend request",
                PicturePath = sender.AvatarPath,
                ReceivingPersonId = reciever.Id,
                SenderId = sender.Id,
                SenderType = "user",
                TargetId = reciever.Id,
                TargetType = "user",
                Time = DateTime.Now,
                Type = "friend request is sent"
            };
        }
        
        public static NotificationModel FriendRequestIsAccepted(UserModel sender, UserModel reciever)
        {
            return new NotificationModel()
            {
                Text = " accepted your friend request",
                PicturePath = sender.AvatarPath,
                ReceivingPersonId = reciever.Id,
                SenderId = sender.Id,
                SenderType = "user",
                TargetId = reciever.Id,
                TargetType = "user",
                Time = DateTime.Now,
                Type = "friend request is accepted"
            };
        }
        public static NotificationModel PublicationIsLikedByUser(
            UserModel sender, 
            UserModel reciever, 
            String TargetPublicationType, 
            int TargetPublicationId
            )
        {
            return new NotificationModel()
            {
                Text = " liked your ",
                PicturePath = sender.AvatarPath,
                ReceivingPersonId = reciever.Id,
                SenderId = sender.Id,
                SenderType = "user",
                TargetId = TargetPublicationId,
                TargetType = TargetPublicationType,
                Time = DateTime.Now,
                Type = "liked by user"
            };
        }
        public static NotificationModel CommentLeftByUser(
            UserModel sender, 
            UserModel reciever, 
            String TargetPublicationType, 
            int TargetPublicationId
        )
        {
            return new NotificationModel()
            {
                Text = " commented your ",
                PicturePath = sender.AvatarPath,
                ReceivingPersonId = reciever.Id,
                SenderId = sender.Id,
                SenderType = "user",
                TargetId = TargetPublicationId,
                TargetType = TargetPublicationType,
                Time = DateTime.Now,
                Type = "comment left by user"
            };
        }
    }
}