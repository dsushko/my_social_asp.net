using System;

namespace network2.Models
{
    public class PhotoModel
    {
        public int Id{ get; set; }
        public int OwnerId { get; set; }
        public string Path { get; set; }
        public DateTime Time { get; set; }
        
    }
}