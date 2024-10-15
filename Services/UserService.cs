using System.Security.Claims;

namespace WindTalkerMessenger.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _http;
        
        public UserService(IHttpContextAccessor http)
        {
            _http = http;
        }

        public string GetUserUserName()
        {
            string userName = _http.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

            return userName;
        }
    }
}
