using System;

namespace network2.Models
{
    public class NotificationModel
    {
        public int Id { get; set; }
        public string PicturePath { get; set; }
        public int ReceivingPersonId { get; set; }
        public DateTime Time { get; set; }
        public String Text { get; set; }
        public int SenderId { get; set; }
        public string SenderType { get; set; }
        public int TargetId { get; set; }
        public string TargetType { get; set; }
    }
}