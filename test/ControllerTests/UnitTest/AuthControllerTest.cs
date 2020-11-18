using AuthenticationService.Service;
using AuthenticationService.Models;
using Moq;
using Xunit;
using AuthenticationService.Controllers;
using Microsoft.AspNetCore.Mvc;
using AuthenticationService.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Collections.Generic;

namespace Test.ControllerTests.UnitTest
{
    public class AuthControllerTest
    {
        [Fact]
        public void RegisterShouldReturnCreated()
        {
            User user = new User { UserId="Jack", Password="password@123" };
            var mockService = new Mock<IAuthService>();
            mockService.Setup(svc => svc.RegisterUser(user)).Returns(true);
            var controller = new AuthController(mockService.Object);

            var actual = controller.Register(user);
            var actionResult = Assert.IsType<CreatedResult>(actual);
            var actionValue = Assert.IsAssignableFrom<bool>(actionResult.Value);
            Assert.True(actionValue);
        }
        [Fact]
        public void RegisterShouldReturnConflict()
        {
            User user = new User { UserId = "Jack", Password = "password@123" };
            var mockService = new Mock<IAuthService>();
            mockService.Setup(svc => svc.RegisterUser(user)).Throws(new UserAlreadyExistsException($"This userId {user.UserId} already in use"));
            var controller = new AuthController(mockService.Object);

            var actual = controller.Register(user);
            var actionResult = Assert.IsType<ConflictObjectResult>(actual);
            Assert.Equal($"This userId {user.UserId} already in use",actionResult.Value);
        }

        [Fact]
        public void LoginShouldReturnJWT()
        {
            User user = new User { UserId = "Jack", Password = "password@123" };
            var mockService = new Mock<IAuthService>();
            mockService.Setup(svc => svc.LoginUser(user)).Returns(true);
            var controller = new AuthController(mockService.Object);

            var actual = controller.Login(user);
            var actionResult = Assert.IsType<OkObjectResult>(actual);
            var data= JsonConvert.DeserializeObject<TokenModel>(actionResult.Value.ToString());

            var jwt = data.Token;
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            var lstclaims= token.Claims as List<Claim>;
            Assert.NotNull(lstclaims.Find(c=>c.Type=="userId"));
        }

        [Fact]
        public void LoginShouldReturnUnauthorized()
        {
            User user = new User { UserId = "Sam", Password = "password@123" };
            var mockService = new Mock<IAuthService>();
            mockService.Setup(svc => svc.LoginUser(user)).Returns(false);
            var controller = new AuthController(mockService.Object);

            var actual = controller.Login(user);
            var actionResult = Assert.IsType<UnauthorizedObjectResult>(actual);
            Assert.Equal("Invalid user id or password", actionResult.Value);
        }
    }
}
