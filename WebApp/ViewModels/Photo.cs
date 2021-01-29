using System;
using System.Collections.Generic;

namespace WebApp.ViewModels
{
    public class Photo
    {
        public int Id{ get; set; }
        public string Path { get; set; }
        public int OwnerId { get; set; }
        public User Owner { get; set; }
        public DateTime Time { get; set; }
        public int Rating { get; set; }
        public List<int> LikeUsers { get; set; }
        public int CommentQuantity { get; set; }
        public List<int> Comments { get; set; }
    }
}