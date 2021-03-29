using System.Collections.Generic;

namespace WebApp.ViewModels
{
    public class User
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public string AvatarPath { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Bday { get; set; }
        public List<int> InputRequests { get; set; }
        public List<int> Subscribers { get; set; }
        public List<int> OutputRequests { get; set; }
        public List<int> Friends { get; set; }
        public List<int> Songs { get; set; }
        public List<int> Photos { get; set; }
    }
}