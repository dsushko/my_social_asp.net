using System;
using System.Collections.Generic;

namespace network2.Models
{
    public class PostModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public int Rating { get; set; }
        public List<int> LikeUsers { get; set; }
        public int OwnerId { get; set; }
        public int CommentQuantity { get; set; }
        public List<int> Comments { get; set; }
        public int SharesQuantity { get; set; }
        public List<int> SharesPeople { get; set; }
        
    }
}