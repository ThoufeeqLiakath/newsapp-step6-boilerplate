using AuthenticationService.Models;
using NewsService;
using NewsService.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Xunit;

namespace Test.ControllerTests.IntegrationTest
{
    [Collection("Auth API")]
    [TestCaseOrderer("Test.PriorityOrderer", "test")]
    public class NewsControllerTest :IClassFixture<NewsWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client, _authclient;
        public NewsControllerTest(NewsWebApplicationFactory<Startup> factory, AuthWebApplicationFactory<AuthenticationService.Startup> authFactory)
        {
            //calling Auth API to get JWT
            User user = new User { UserId = "Jack", Password = "password@123" };
            _authclient = authFactory.CreateClient();
            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the authentication controller action.
            var httpResponse = _authclient.PostAsync<User>("/api/auth/login", user, formatter);
            httpResponse.Wait();
            // Deserialize and examine results.
            var stringResponse = httpResponse.Result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<TokenModel>(stringResponse.Result);

            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {response.Token}");
        }

        [Fact, TestPriority(1)]
        public async Task PostShouldReturnNews()
        {
            News news = new News { Title = "chandrayaan2-spacecraft", Content = "The Lander of Chandrayaan-2 was named Vikram after Dr Vikram A Sarabhai", PublishedAt = DateTime.Now };
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsync($"/api/news", news, formatter);

            // Deserialize and examine results.
            Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            Assert.Equal(102, Convert.ToInt32(stringResponse));
        }

        [Fact, TestPriority(2)]
        public async Task GetByUserIdShouldReturnListOfNews()
        {
            // The endpoint or route of the controller action.
            var httpResponse = await _client.GetAsync($"/api/news");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var lstnews= JsonConvert.DeserializeObject<List<News>>(stringResponse);
            Assert.NotNull(lstnews);
            Assert.Equal(2,lstnews.Count);
        }

        [Fact, TestPriority(3)]
        public async Task DeleteShouldSuccess()
        {
            int newsId = 102;
            // The endpoint or route of the controller action.
            var httpResponse = await _client.DeleteAsync($"/api/news/{newsId}");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            Assert.True(Convert.ToBoolean(stringResponse));
        }

        [Fact, TestPriority(4)]
        public async Task PostShouldReturnConflict()
        {
            string userId = "Jack";
            News news = new News { Title = "IT industry in 2020", Content = "It is expected to have positive growth in 2020.", PublishedAt = DateTime.Now, UrlToImage = null, Url = null };
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();
            
            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsync($"/api/news", news, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.Conflict, httpResponse.StatusCode);
            Assert.Equal($"{userId} have already added this news", stringResponse);
        }

        [Fact, TestPriority(5)]
        public async Task DeleteShouldReturnNotFound()
        {
            string userId = "Jack";
            int newsId = 104;
            // The endpoint or route of the controller action.
            var httpResponse = await _client.DeleteAsync($"/api/news/{newsId}");

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
            Assert.Equal($"NewsId {newsId} for {userId} doesn't exist", stringResponse);
        }

        [Fact, TestPriority(9)]
        public async Task PutReminderShouldReturnTrue()
        {
            int newsId = 101;
            Reminder reminder = new Reminder { Schedule = DateTime.Now.AddDays(3) };
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PutAsync($"/api/news/{newsId}/reminder", reminder, formatter);

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            Assert.True(Convert.ToBoolean(stringResponse));
        }

        [Fact, TestPriority(10)]
        public async Task DeleteReminderShouldReturnTrue()
        {
            int newsId = 101;

            // The endpoint or route of the controller action.
            var httpResponse = await _client.DeleteAsync($"/api/news/{newsId}/reminder");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            Assert.True(Convert.ToBoolean(stringResponse));
        }



        [Fact, TestPriority(11)]
        public async Task PutReminderShouldReturnNotFound()
        {
            string userId = "Jack";
            int newsId = 103;
            Reminder reminder = new Reminder { Schedule = DateTime.Now.AddDays(3) };
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PutAsync($"/api/news/{newsId}/reminder", reminder, formatter);

            // Deserialize and examine results.
            Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            Assert.Equal($"NewsId {newsId} for {userId} doesn't exist", stringResponse);
        }

        [Fact, TestPriority(12)]
        public async Task DeleteReminderShouldReturnNotFound()
        {
            int newsId = 103;

            // The endpoint or route of the controller action.
            var httpResponse = await _client.DeleteAsync($"/api/news/{newsId}/reminder");

            // Deserialize and examine results.
            Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            Assert.Equal("No reminder found for this news", stringResponse);
        }
    }
}
