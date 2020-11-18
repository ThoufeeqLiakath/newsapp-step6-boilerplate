using AuthenticationService.Models;

namespace AuthenticationService.Repository
{
    /*
    * Should not modify this interface. You have to implement these methods of interface 
    * in corresponding Implementation classes
    */
    public interface IAuthRepository
    {
        bool CreateUser(User user);
        bool IsUserExists(string userId);
        bool LoginUser(User user);
    }
}