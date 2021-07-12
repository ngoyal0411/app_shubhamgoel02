using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevOpsnMicroServices.Models;

namespace DevOpsnMicroServices.Interfaces
{
    interface IUserProvider
    {        
        User GetUser(int Id);
        List<User> GetUsers();
    }
}
