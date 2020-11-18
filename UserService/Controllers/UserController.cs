using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Exceptions;
using UserService.Models;
using UserService.Services;
namespace UserService.Controllers
{
    /*
     * As in this assignment, we are working with creating RESTful web service, hence annotate
     * the class with [ApiController] annotation and define the controller level route as per 
     * REST Api standard.
     * 
     * Authorize the controller by using attribute to the Controller
     */
    [Authorize]
    public class UserController : ControllerBase
    {
        /*
        UserService should  be injected through constructor injection. 
        Please note that we should not create service
        object using the new keyword
        */
        string userId = string.Empty;
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /* Implement HttpVerbs and its Functionality asynchronously*/

        /*
         * Define a handler method which will get us the user by a userId.
         * This handler method should return any one of the status messages basis on
         * different situations: 
         * 1. 200(OK) - If the news found successfully.
         * 2. Handle Custom Exception when expected userId not found
         * 3. Use HttpGet to get the user by userId
         */
        [HttpGet]
        [Route("/api/user")]
        public async Task<IActionResult> Get()
        {
            try
            {

                var userId = User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value;
                return Ok(await _userService.GetUser(userId));
            }
            catch (UserNotFoundException nnf)
            {
                return NotFound(nnf.Message);
            }
            catch (UserAlreadyExistsException nnf)
            {
                return Conflict(nnf.Message);
            }
            catch (System.Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /*
        * Define a handler method which will create a specific UserProfile by reading the
        * Serialized object from request body and save the user details in a User table
        * in the database.
        * 
        * Please note that AddUser method should add a userdetails and also handle the exception.
        * Dispaly the status messages basis on different situations: 
        * 1. 201(CREATED) - If the userProfile details created successfully. 
        * 2. 409(CONFLICT) - If the userId conflicts with any existing userId
        * 
        * use HTTP POST method to Add user Details.
        */

        [HttpPost]
        [Route("/api/user")]
        public async Task<IActionResult> Post([FromBody]UserProfile user)
        {
            try
            {
                var userId = User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value;
                if (userId != user.UserId)
                {
                    return Unauthorized("Your credentials doesn't match User Profile");
                }
                return Created("", await _userService.AddUser(user));
            }
            catch (UserNotFoundException nnf)
            {
                return NotFound(nnf.Message);
            }
            catch (UserAlreadyExistsException nnf)
            {
                return Conflict(nnf.Message);
            }
            catch (System.Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("/api/user/healthcheck")]
        public IActionResult HealthCheck()
        {
            try
            {
                return Ok("Alive : Docker Env: " + Environment.GetEnvironmentVariable("MongoDB"));
            }
            catch (Exception)
            {

                throw;
            }
        }
        /*
       * Define a handler method which will update a specific user by reading the
       * Serialized object from request body and save the updated user details in a
       * user table in database handle exception as well.
       * This handler method should return any one of the status messages basis on different situations: 
       * 1. 200(OK) - If the user updated successfully. 
       * 2. 404(NOT FOUND) - If the user with specified userId is not found. 
       * 
       * use HTTP PUT method.
       */
        [HttpPut]
        [Route("/api/user")]
        public async Task<IActionResult> Put([FromBody] UserProfile user)
        {
            try
            {
                var userId = User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value;
                
                if (userId != user.UserId)
                {
                    return Unauthorized($"You are not allowed to update {user.UserId} Profile");
                }
                return Ok(await _userService.UpdateUser(userId, user));
            }
            catch (UserNotFoundException nnf)
            {
                return NotFound(nnf.Message);
            }
            catch (UserAlreadyExistsException nnf)
            {
                return Conflict(nnf.Message);
            }
            catch (System.Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
