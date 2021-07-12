using System;
using Xunit;
using DevOpsnMicroServices.Providers;
using DevOpsnMicroServices.Models;

namespace DevOpsnMicroServices.Tests
{
    public class UserTests
    {
        [Fact]
        public void GetUsers_ReturnsUsers()
        {
            bool result = (new UserProvider()).GetUsers().Count > 0;
            Assert.True(result, "User List is empty");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void GetUser_ReturnsNoUser(int Id)
        {
            User user = (new UserProvider().GetUser(Id));
            bool result = user.Id <= 0 ? true : false;
            Assert.True(result, "User should not be found.");
        }

        [Theory]
        [InlineData(1)]
        public void GetUser_ReturnsUser(int Id)
        {
            User user = (new UserProvider().GetUser(Id));
            bool result = user.Id > 0 ? true : false;
            Assert.True(result, "User should be found.");
        }
    }
}
