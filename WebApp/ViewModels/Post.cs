using System;
using System.Collections.Generic;

namespace WebApp.ViewModels
{
    public class Post
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public Post ForwardedPost { get; set; }
        public int Rating { get; set; }
        public int OwnerId { get; set; }
        public User Owner { get; set; }
        public int CommentQuantity { get; set; }
        public int SharesQuantity { get; set; }
    }
}