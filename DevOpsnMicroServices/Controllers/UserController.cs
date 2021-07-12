using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DevOpsnMicroServices.Models;
using DevOpsnMicroServices.Providers;

namespace DevOpsnMicroServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public List<User> Get()
        {
            return (new UserProvider().GetUsers());
        }

        [HttpGet]
        [Route("{Id}")]
        public User Get(int Id)
        {
            return (new UserProvider().GetUser(Id));
        }
    }
}