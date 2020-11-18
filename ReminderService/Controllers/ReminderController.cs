using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReminderService.Exceptions;
using ReminderService.Models;
using ReminderService.Services;
namespace ReminderService.Controllers
{
    /*
    * As in this assignment, annotate
    * the class with [ApiController] annotation and define the controller level route as per REST Api standard. 
    * and Authorize the Reminder Controller with Authorize atrribute
    */
    [Authorize]
    public class ReminderController : ControllerBase
    {
        /*
        * ReminderService should  be injected through constructor injection. 
        * Please note that we should not create Reminderservice object using the new keyword
        */
        private readonly IReminderService _reminderService;

        public ReminderController(IReminderService reminderService)
        {
            _reminderService = reminderService;
        }
        /* Implement HttpVerbs and its Functionalities asynchronously*/
        [HttpGet]
        [Route("/api/user/healthcheck")]
        public IActionResult HealthCheck()
        {
            return Ok("Alive : Docker Env: " + Environment.GetEnvironmentVariable("MongoDB"));
        }
        /*
        * Define a handler method which will get us the reminders by a userId.
        * 
        * This handler method should return any one of the status messages basis on
        * different situations: 
        * 1. 200(OK) - If the reminder found successfully.
        * 
        * This handler method should map to the URL using HTTP GET method
        * and also handle the custom exception for the same
        */
        [HttpGet]
        [Route("/api/reminder")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var userId = User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value;
                return Ok(await _reminderService.GetReminders(userId));
            }
            catch (NoReminderFoundException nnf)
            {
                return NotFound(nnf.Message);
            }
            catch (ReminderAlreadyExistsException nnf)
            {
                return Conflict(nnf.Message);
            }
            catch (System.Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        /*
        * Define a handler method which will create a reminder by reading the
        * Serialized reminder object from request body and save the reminder in
        * reminder table in database. 
        * This handler method should return any one of the status messages
        * basis on different situations: 
        * 1. 201(CREATED - In case of successful creation of the reminder 
        * 2. 409(CONFLICT) - In case of duplicate reminder ID
        * This handler method should use HTTP POST
        * method".
        */

        [HttpPost]
        [Route("/api/reminder")]
        public async Task<IActionResult> Post([FromBody]Reminder reminder)
        {
            try
            {                
                var result=await _reminderService.CreateReminder(reminder.UserId, reminder.Email, reminder.NewsReminders[0]);                
                return Created("",result);
            }
            catch (NoReminderFoundException nnf)
            {
                return NotFound(nnf.Message);
            }
            catch (ReminderAlreadyExistsException nnf)
            {
                return Conflict(nnf.Message);
            }
            catch (System.Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        /*
        * Define a handler method which will delete a reminder from a database.
        * This handler method should return any one of the status messages basis on
        * different situations: 
        * 1. 200(OK) - If the reminder deleted successfully from database. 
        * 2. 404(NOT FOUND) - If the reminder with specified userId with newsId is  not found. 
        * This handler method should map to HTTP Delete.
        */
        [HttpDelete]
        [Route("/api/reminder")]
        public async Task<IActionResult> Delete( [FromQuery]int newsId)
        {
            try
            {
                var userId = User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value;
                return Ok(await _reminderService.DeleteReminder(userId, newsId));
            }
            catch (NoReminderFoundException nnf)
            {
                return NotFound(nnf.Message);
            }
            catch (ReminderAlreadyExistsException nnf)
            {
                return Conflict(nnf.Message);
            }
            catch (System.Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        /*
         * Define a handler method (Put) which will update a reminder by userId,newsId and with Reminder Details
         * 
         * This handler method should return any one of the status messages basis on
         * different situations: 
         * 1. 200(OK) - If the news updated successfully to the database using userId with newsId
         * 2. 404(NOT FOUND) - If the news with specified newsId is not found.
         * 
         * This handler method should be used to update the existing reminder details.
         */
        [HttpPut]
        [Route("/api/reminder")]
        public async Task<IActionResult> Put([FromBody] ReminderSchedule reminderSchedule)
        {
            try
            {
                var userId = User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value;
                return Ok(await _reminderService.UpdateReminder(userId, reminderSchedule));
            }
            catch (NoReminderFoundException nnf)
            {
                return NotFound(nnf.Message);
            }
            catch (ReminderAlreadyExistsException nnf)
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
