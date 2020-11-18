using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using UserService.Models;
namespace UserService.Repository
{
    //Inherit the respective interface and implement the methods in 
    // this class i.e UserRepository by inheriting IUserRepository class 
    //which is used to implement all methods in the classs
    public class UserRepository : IUserRepository
    {
        //define a private variable to represent UserContext 
        private readonly UserContext _userContext;
        public UserRepository(UserContext userContext)
        {
            _userContext = userContext;
        }

        public Task<bool> AddUser(UserProfile user)
        {
            _userContext.Users.InsertOne(user);
            var insertedUser = GetUser(user.UserId).Result;
            return Task.FromResult(insertedUser != null);
        }

        public Task<bool> DeleteUser(string userId)
        {
            var deleted = _userContext.Users.DeleteOne(x => x.UserId == userId);
            return Task.FromResult(deleted.DeletedCount > 0);
        }

        public Task<UserProfile> GetUser(string userId)
        {
            var user = _userContext.Users.Find(x => x.UserId == userId).FirstOrDefault();
            return Task.FromResult(user);
        }

        public Task<bool> UpdateUser(UserProfile user)
        {
            var getVal = _userContext.Users.Find(x => x.UserId == user.UserId).FirstOrDefault();
            var filter = Builders<UserProfile>.Filter.Where(x => x.UserId == user.UserId);
            var update = Builders<UserProfile>.Update.Set(x => x.Email, user.Email)
                                                    .Set(x => x.Contact, user.Contact)
                                                    .Set(x => x.FirstName, user.FirstName)
                                                    .Set(x => x.LastName, user.LastName)
                                                    .Set(x => x.CreatedAt, user.CreatedAt);
            var updated = _userContext.Users.UpdateOne(filter, update);
            var getVal1 = _userContext.Users.Find(x => x.UserId == user.UserId).FirstOrDefault();
            var v1 = user.FirstName == getVal1.FirstName;
            var v2 = user.LastName == getVal1.LastName;
            var v3 = user.Email == getVal1.Email;
            var v4 = user.Contact == getVal1.Contact;

            return Task.FromResult(v1 && v2 && v3 && v4);
        }
    }
}
