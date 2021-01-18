using System;

namespace WebApp.ViewModels
{
    public class Comment
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public string Text { get; set; }
        public int OwnerId { get; set; }
        public int PostId { get; set; }
        public int ReplyCommentId { get; set; }
        public int Rating { get; set; }
        public User Owner { get; set; }
    }
}