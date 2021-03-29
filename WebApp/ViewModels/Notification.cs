using System;

namespace WebApp.ViewModels
{
    public class Notification
    {
        public int Id { get; set; }
        public String Type { get; set; }
        public string PicturePath { get; set; }
        public int ReceivingPersonId { get; set; }
        public DateTime Time { get; set; }
        public String Text { get; set; }
        public User SenderUser { get; set; }
        //public Group SenderGroup { get; set; }
        public string SenderType { get; set; }
        public User TargetUser { get; set; }
        //public Froup TargetGroup {get; set;}
        public string TargetType { get; set; }
        public int TargetId { get; set; }
    }
}