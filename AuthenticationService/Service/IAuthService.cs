using AuthenticationService.Models;
namespace AuthenticationService.Service
{
    /*
    * Should not modify this interface. You have to implement these methods of interface 
    * in corresponding Implementation classes
    */
    public interface IAuthService
    {
        bool LoginUser(User user);
        bool RegisterUser(User user);
    }
}