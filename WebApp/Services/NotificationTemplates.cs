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
                Time = DateTime.Now
            };
        }
    }
}