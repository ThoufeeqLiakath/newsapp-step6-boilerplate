using AuthenticationService.Exceptions;
using AuthenticationService.Models;
using AuthenticationService.Repository;
namespace AuthenticationService.Service
{
    //Inherit the respective interface and implement the methods in 
    // this class i.e AuthService by inheriting IAuthService class 
    //which is used to implement all methods in the classs.
    public class AuthService:IAuthService
    {
        //define a private variable to represent repository
        private readonly IAuthRepository _authRepository;
        //Use constructor Injection to inject all required dependencies.
        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public bool LoginUser(User user)
        {
            return _authRepository.LoginUser(user);
        }

        public bool RegisterUser(User user)
        {
            if(_authRepository.IsUserExists(user.UserId))
            {
                throw new UserAlreadyExistsException($"This userId {user.UserId} already in use");
            }
            return _authRepository.CreateUser(user);
        }
        /* Implement all the methods of respective interface asynchronously*/

        //Implement the method  'RegisterUser' which is used to register a new user and 
        // handle the Custom Exception for UserAlreadyExistsException


        //Implement the method 'LoginUser' which is used to login existing user and also handle the Custom Exception 

    }
}
