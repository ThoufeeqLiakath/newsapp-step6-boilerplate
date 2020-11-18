using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using NewsService.Exceptions;
using NewsService.Models;
using NewsService.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;

namespace NewsService.Controllers
{
    /*
    * For creating RESTful web service,annotate the class with [ApiController] annotation and define the controller 
    * level route as per REST Api standard and Authorize the NewsController with Authorize atrribute
    */
    [Authorize]
    public class NewsController : ControllerBase
    {
        /*
        * NewsService should  be injected through constructor injection. 
        * Please note that we should not create service object using the 
        * new keyword*/
        string userId = string.Empty;
		        private readonly INewsService _newsService;
        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }
        /* Implement HttpVerbs and its Functionalities asynchronously*/

        [HttpGet]
        [Route("/api/user/healthcheck")]
        public IActionResult HealthCheck()
        {
            return Ok("Alive : Docker Env: " + Environment.GetEnvironmentVariable("MongoDB"));
        }
        /*
         * Define a handler method which will get us the news by a userId.
         * 
         * This handler method should return any one of the status messages basis on
         * different situations: 
         * 1. 200(OK) - If the news found successfully.
         * This handler method should map to HTTP GET method
         */
        [HttpGet]
        [Route("api/news")]

        public async Task<IActionResult> Get()
        {
            try
            {
                var test=ClaimsPrincipal.Current;
                var userId = User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value;
                var result = await _newsService.FindAllNewsByUserId(userId);
                return Ok(result);
            }
            catch (NoNewsFoundException nnf)
            {
                return NotFound(nnf.Message);
            }

            catch (NewsAlreadyExistsException nnf)
            {
                return Conflict(nnf.Message);
            }
            catch (System.Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        /*
        * Define a handler method which will create a specific news by reading the
        * Serialized object from request body and save the news details in a News table
        * in the database.
        * 
        * Please note that CreateNews method should add a news and also handle the exception.
        * This handler method should return any one of the status messages basis on different situations: 
        * 1. 201(CREATED) - If the news created successfully. 
        * 2. 409(CONFLICT) - If the userId conflicts with any existing newsid
        * 
        * This handler method should map to the URL "/api/news" using HTTP POST method
        */
        [HttpPut]
        [Route("/api/news/{newsId}/reminder")]//api/news/userId/newsId/reminder
        public async Task<IActionResult> Post( int newsId, [FromBody]Reminder reminder)        
        { 
            try
            {
                var userId = User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value;
                return Created("", await _newsService.AddOrUpdateReminder(userId, newsId, reminder));
            }
            catch (NoNewsFoundException nnf)
            {
                return NotFound(nnf.Message);
            }

            catch (NewsAlreadyExistsException nnf)
            {
                return Conflict(nnf.Message);
            }
            catch (System.Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        /*
         * Define a handler method which will delete a news from a database.
         * 
         * This handler method should return any one of the status messages basis on
         * different situations: 
         * 1. 200(OK) - If the news deleted successfully from database. 
         * 2. 404(NOT FOUND) - If the news with specified newsId is not found.
         * 
         * This handler method should map to the URL "/api/news/userId/newsId" using HTTP Delete
         * method" where "id" should be replaced by a valid newsId without {}
         */
        [HttpDelete]
        [Route("/api/news/{newsId}")]
        public async Task<IActionResult> Delete(int newsId)
        {
            try
            {
                var userId = User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value;
                return Ok(await _newsService.DeleteNews(userId, newsId));
            }
            catch (NoNewsFoundException nnf)
            {
                return NotFound(nnf.Message);
            }

            catch (NewsAlreadyExistsException nnf)
            {
                return Conflict(nnf.Message);
            }
            catch (System.Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        /*
         * Define a handler method (DeleteReminder) which will delete a news from a database.
         * 
         * This handler method should return any one of the status messages basis on
         * different situations: 
         * 1. 200(OK) - If the news deleted successfully from database using userId with newsId
         * 2. 404(NOT FOUND) - If the news with specified newsId is not found.
         * 
         * This handler method should map to the URL "/api/news/userId/newsId/reminder" using HTTP Delete
         * method" where "id" should be replaced by a valid newsId without {}
         */

        [HttpDelete]
        [Route("/api/news/{newsId:int}/reminder")]
        public async Task<IActionResult> DeleteReminder( int newsId)
        {
            try
            {
                var userId = User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value;
                return Ok(await _newsService.DeleteReminder(userId, newsId));
            }
            catch (NoNewsFoundException nnf)
            {
                return NotFound(nnf.Message);
            }
            catch(NoReminderFoundException nnf)
            {
                return NotFound(nnf.Message);
            }            
            catch (NewsAlreadyExistsException nnf)
            {
                return Conflict(nnf.Message);
            }
            catch (System.Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        /*
         * Define a handler method (Put) which will update a news by userId,newsId and with Reminder Details
         * 
         * This handler method should return any one of the status messages basis on
         * different situations: 
         * 1. 200(OK) - If the news updated successfully to the database using userId with newsId
         * 2. 404(NOT FOUND) - If the news with specified newsId is not found.
         * 
         * This handler method should map to the URL "/api/news/userId/newsId/reminder" using HTTP PUT
         * method" where "id" should be replaced by a valid newsId without {}
         */
        [HttpPost]
        [Route("/api/news")]//api/news
        
        public async Task<IActionResult> Post([FromBody]News news)
        {
            try
            {
                var userId = User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value;
                return Created("", await _newsService.CreateNews(userId, news));
            }
            catch (NoNewsFoundException nnf)
            {
                return NotFound(nnf.Message);
            }

            catch (NewsAlreadyExistsException nnf)
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
