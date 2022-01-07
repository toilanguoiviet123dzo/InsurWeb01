using System.Threading.Tasks;

namespace Gosu.GrpcClient.Authentication
{
    public interface IAuthenticationService
    {
        Task<bool> Login(AuthenticationRequestModel userForAuthentication);
        void Logout();
        void Development_Login();
    }
}