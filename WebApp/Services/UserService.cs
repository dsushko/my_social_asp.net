using System.Linq;
using network2.Models;
using WebApp.ViewModels;


namespace WebApp.Services
{
    public class UserService
    {
        private readonly ApplicationContext _db;
        public UserService(ApplicationContext db)
        {
            _db = db;
        }
        
        public User GetUserById(int id)
        {
            return Mappers.BuildUser(_db.UserModels.FirstOrDefault(um => um.Id == id));
        }

        public User GetUserByName(string name)
        {
            return Mappers.BuildUser(_db.UserModels.FirstOrDefault(um => um.Nickname == name));
        }

    }
}