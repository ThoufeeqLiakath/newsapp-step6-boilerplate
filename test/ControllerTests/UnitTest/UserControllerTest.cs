using System;
using Xunit;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserService.Models;
using UserService.Services;
using UserService.Controllers;
using UserService.Exceptions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Test.ControllerTests.UnitTest
{
    public class UserControllerTest
    {
        [Fact]
        public async Task PostShouldReturnCreated()
        {
            string userId = "John";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("userId", userId)
            }, "mock"));

            UserProfile _user = new UserProfile { UserId = "John", FirstName = "Johnson", LastName = "dsouza", Contact = "7869543210", Email = "john@gmail.com", CreatedAt = DateTime.Now };
            var mockService = new Mock<IUserService>();
            mockService.Setup(svc => svc.AddUser(_user)).Returns(Task.FromResult(true));
            var controller = new UserController(mockService.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

            var actual = await controller.Post(_user);
            var actionResult = Assert.IsType<CreatedResult>(actual);
            Assert.True(Convert.ToBoolean(actionResult.Value));
        }

        [Fact]
        public async Task GetShouldReturnOk()
        {
            string userId = "John";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("userId", userId)
            }, "mock"));

            UserProfile _user = new UserProfile { UserId = "John", FirstName = "Johnson", LastName = "dsouza", Contact = "7869543210", Email = "john@gmail.com", CreatedAt = DateTime.Now };
            var mockService = new Mock<IUserService>();
            mockService.Setup(svc => svc.GetUser(userId)).Returns(Task.FromResult(_user));
            var controller = new UserController(mockService.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

            var actual = await controller.Get();
            var actionResult = Assert.IsType<OkObjectResult>(actual);
            Assert.IsAssignableFrom<UserProfile>(actionResult.Value);
        }

        [Fact]
        public async Task PutShouldReturnOk()
        {
            string userId = "Jack";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("userId", userId)
            }, "mock"));
            UserProfile _user = new UserProfile { UserId = "Jack", FirstName = "Jackson", LastName = "James", Contact = "9812345670", Email = "jack@ymail.com", CreatedAt = DateTime.Now };
            var mockService = new Mock<IUserService>();
            mockService.Setup(svc => svc.UpdateUser(userId,_user)).Returns(Task.FromResult(true));
            var controller = new UserController(mockService.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

            var actual = await controller.Put(_user);
            var actionResult = Assert.IsType<OkObjectResult>(actual);
            Assert.True(Convert.ToBoolean(actionResult.Value));
        }

        [Fact]
        public async Task PostShouldReturnConflict()
        {
            string userId = "John";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("userId", userId)
            }, "mock"));

            UserProfile _user = new UserProfile { UserId = "John", FirstName = "Johnson", LastName = "dsouza", Contact = "7869543210", Email = "john@gmail.com", CreatedAt = DateTime.Now };
            var mockService = new Mock<IUserService>();
            mockService.Setup(svc => svc.AddUser(_user)).Throws(new UserAlreadyExistsException($"{_user.UserId} is already in use"));
            var controller = new UserController(mockService.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

            var actual = await controller.Post(_user);
            var actionResult = Assert.IsType<ConflictObjectResult>(actual);
            Assert.Equal($"{_user.UserId} is already in use", actionResult.Value);
        }
       
        [Fact]
        public async Task GetShouldReturnNotFound()
        {
            string userId = "Kevin";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("userId", userId)
            }, "mock"));
            var mockService = new Mock<IUserService>();
            mockService.Setup(svc => svc.GetUser(userId)).Throws(new UserNotFoundException($"This user id doesn't exist"));
            var controller = new UserController(mockService.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

            var actual = await controller.Get();
            var actionResult = Assert.IsType<NotFoundObjectResult>(actual);
            Assert.Equal($"This user id doesn't exist", actionResult.Value);
        }

        
       [Fact]
       public async Task PutShouldReturnNotFound()
       {
            string userId = "Kevin";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("userId", userId)
            }, "mock"));

            UserProfile _user = new UserProfile { UserId = "Kevin", FirstName = "Kevin", LastName = "Lloyd", Contact = "9812345670", Email = "kevin@gmail.com", CreatedAt = DateTime.Now };
            var mockService = new Mock<IUserService>();
            mockService.Setup(svc => svc.UpdateUser(userId, _user)).Throws(new UserNotFoundException($"This user id doesn't exist"));
            var controller = new UserController(mockService.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };

            var actual = await controller.Put(_user);
            var actionResult = Assert.IsType<NotFoundObjectResult>(actual);
            Assert.Equal($"This user id doesn't exist", actionResult.Value);
        }
    }
}
