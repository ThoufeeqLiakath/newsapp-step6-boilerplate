using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using AuthenticationService.Exceptions;
using AuthenticationService.Models;
using AuthenticationService.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
namespace AuthenticationService.Controllers
{
    /* Annotate the class with [ApiController] annotation and define the controller level 
     * route as per REST Api standard.
     */
    public class AuthController : Controller
    {
        /*
         AuthService should  be injected through constructor injection. Please note that we should not create service
         object using the new keyword

        */
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /*
       * Define a handler method which will create a specific user by reading the
       * Serialized object from request body and save the user details in the
       * database. This handler method should return any one of the status messages
       * basis on different situations:
       * 1. 201(CREATED) - If the user created successfully. 
       * 2. 409(CONFLICT) - If the userId conflicts with any existing user
       * 
       * This handler method should map to the URL "/api/auth/register" using HTTP POST method
       */
        [HttpPost]
        [Route("api/auth/register")]
        public IActionResult Register([FromBody]User user)
        {
            try
            {
                var registresetedUser = _authService.RegisterUser(user);
                var token = GenerateToken(user);
                return Created("", token);
            }

            catch (UserAlreadyExistsException uae)
            {
                return Conflict(uae.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        /* Define a handler method which will authenticate a user by reading the Serialized user
        * object from request body containing the username and password. The username and password 
        * should be validated before proceeding ahead with JWT token generation. The user credentials 
        * will be validated against the database entries. 
        * 
        * The error should be return if validation is not successful. If credentials are validated successfully, 
        * then JWT token will be generated. The token should be returned back to the caller along with the API 
        * response. This handler method should return any one of the status messages basis on different
        * situations:
        * 1. 200(OK) - If login is successful
        * 2. 401(UNAUTHORIZED) - If login is not successful
        * 
        * This handler method should map to the URL "/api/auth/login" using HTTP POST method
       */
        [HttpPost]
        [Route("api/auth/login")]
        public IActionResult Login([FromBody]User user)
        {
            try
            {
                //User user = JsonConvert.DeserializeObject<User>(usr);
                if (_authService.LoginUser(user))
                {
                    var token = GenerateToken(user);
                    return Ok(token);
                }
                return Unauthorized("Invalid user id or password");

            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpGet]
        [Route("api/auth/healthcheck")]
        public IActionResult HealthCheck()
        {
            return Ok("Alive: SqlDocker : " + Environment.GetEnvironmentVariable("ConnectionString"));
        }

        private string GenerateToken(User user)
        {
            var claims = new Claim[]
            {
                new Claim("userId",user.UserId),
                new Claim("password",user.Password)
            };
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("ThoufeeqLiakath_Is_My_Name_And_That_Is_Also_The_Key_Value_for_This"));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new JwtSecurityToken("AuthService", "StackRouteApis", claims, null, DateTime.UtcNow.AddMinutes(2000), cred);
            var response = new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
            return JsonConvert.SerializeObject(response);

        }
    }
}
