using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationService.Models;
namespace AuthenticationService.Repository
{
    //Inherit the respective interface and implement the methods in 
    // this class i.e AuthRepository by inheriting IAuthRepository class 
    //which is used to implement all methods in the classs.
    public class AuthRepository:IAuthRepository
    {
        //Define a private variable to represent AuthDbContext
        private readonly AuthDbContext _context;
        public AuthRepository(AuthDbContext dbContext)
        {
            _context = dbContext;
        }

        public bool CreateUser(User user)
        {
            var createdUser=_context.Add(user);
            if(_context.SaveChanges()>0)
            {
                return true;
            }
            return false;
        }

        public bool IsUserExists(string userId)
        {
            var user = _context.Users.Where(x => x.UserId == userId).FirstOrDefault();
            if(user==null)
            {
                return false;
            }
            return true;
        }

        public bool LoginUser(User user)
        {
            var retrivedUser = _context.Users.Where(x => x.UserId == user.UserId&&x.Password==user.Password).FirstOrDefault();
            if(retrivedUser==null)
            {
                return false;
            }
            return true;
        }

        /* Implement all the methods of respective interface asynchronously*/

        //Implement the method  'CreateUser' which is used to create a new user.

        //Implement the method  'IsUserExists' which is used to check userId exist or not.

        //Implement the method 'LoginUser' which is used to login for the existing user.
    }
}
