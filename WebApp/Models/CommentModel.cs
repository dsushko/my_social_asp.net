using System;

namespace network2.Models
{
    public class CommentModel
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public string Text { get; set; }
        public int OwnerId { get; set; }
        public int PostId { get; set; }
        public int ReplyCommentId { get; set; }
        public int Rating { get; set; }
    }
}