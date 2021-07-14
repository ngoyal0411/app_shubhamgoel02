using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevOpsnMicroServices.Interfaces;
using DevOpsnMicroServices.Models;
using DevOpsnMicroServices.Data;

namespace DevOpsnMicroServices.Providers
{
    public class UserProvider : IUserProvider
    {
        public User GetUser(int Id)
        {
            int UserCount = UserData.GetUsers().Count(x => x.Id == Id);
            if (UserCount == 0)
                return new User();
            else
                return UserData.GetUsers().First(x => x.Id == Id);
        }

        public List<User> GetUsers()
        {
            return UserData.GetUsers();
        }
    }
}
