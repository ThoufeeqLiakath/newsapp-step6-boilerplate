using AuthenticationService.Models;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using UserService;
using UserService.Models;
using Xunit;

namespace Test.ControllerTests.IntegrationTest
{
    [Collection("Auth API")]
    [TestCaseOrderer("Test.PriorityOrderer", "test")]
    public class UserControllerTest:IClassFixture<UserWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client,_authclient;
        public UserControllerTest(UserWebApplicationFactory<Startup> factory, AuthWebApplicationFactory<AuthenticationService.Startup> authFactory)
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
        public async Task PostShouldSuccess()
        {
            var user = new UserProfile
            {
                UserId = "Jack",
                FirstName = "Jackson",
                LastName = "James",
                Contact = "9812345670",
                Email = "jack@ymail.com",
                CreatedAt = DateTime.Now
            };
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsync("/api/user", user, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);
            Assert.True(Convert.ToBoolean(stringResponse));
        }

        [Fact, TestPriority(2)]
        public async Task GetShouldReturnUser()
        {
            // The endpoint or route of the controller action.
            var httpResponse = await _client.GetAsync($"/api/user");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<UserProfile>(stringResponse);
            Assert.Equal("Jackson", user.FirstName);
        }

        [Fact, TestPriority(3)]
        public async Task UpdateUserShouldSuccess()
        {
            var user = new UserProfile
            {
                UserId = "Jack",
                FirstName = "Jackson",
                LastName = "James",
                Contact = "9877665544",
                Email = "jackson@ymail.com",
                CreatedAt = DateTime.Now
            };
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PutAsync($"/api/user", user, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.True(Convert.ToBoolean(stringResponse));
        }
        
        [Fact, TestPriority(4)]
        public async Task PostShouldReturnUnauthorized()
        {
            UserProfile user = new UserProfile { UserId = "John", FirstName = "Johnson", LastName = "dsouza", Contact = "9812345670", Email = "jogn@gmail.com", CreatedAt = DateTime.Now };
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PostAsync("/api/user", user, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.Unauthorized, httpResponse.StatusCode);
            Assert.Equal($"Your credentials doesn't match User Profile", stringResponse);
        }

        [Fact, TestPriority(5)]
        public async Task UpdateUserShouldReturnUnAuthorized()
        {
            UserProfile user = new UserProfile { UserId = "Sam", FirstName = "Sam", LastName = "Methews", Contact = "9812345677", Email = "sam@gmail.com", CreatedAt = DateTime.Now };
            HttpRequestMessage request = new HttpRequestMessage();
            MediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            // The endpoint or route of the controller action.
            var httpResponse = await _client.PutAsync($"/api/user", user, formatter);

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.Unauthorized, httpResponse.StatusCode);
            Assert.Equal($"You are not allowed to update {user.UserId} Profile", stringResponse);
        }
    }
}
