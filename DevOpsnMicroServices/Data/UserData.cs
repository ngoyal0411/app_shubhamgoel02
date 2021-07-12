using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevOpsnMicroServices.Models;

namespace DevOpsnMicroServices.Data
{
    public class UserData
    {
        public static List<User> GetUsers()
        {
            return new List<User>()
            {
                new User() {Id = 1, Name = "Chris"},
                new User() {Id = 2, Name = "Peter"},
                new User() {Id = 3, Name = "Stas"},
                new User() {Id = 4, Name = "John"},
                new User() {Id = 5, Name = "Lenis"},
            };
        }
    }
}
