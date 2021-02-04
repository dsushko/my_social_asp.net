using System;
using System.Collections.Generic;

namespace network2.Models
{
    public class PhotoModel
    {
        public int Id{ get; set; }
        public int Hash { get; set; }
        public string Path { get; set; }
        public int OwnerId { get; set; }
        public DateTime Time { get; set; }
        public int Rating { get; set; }
        public List<int> LikeUsers { get; set; }
        public int CommentQuantity { get; set; }
        public List<int> Comments { get; set; }
    }
}