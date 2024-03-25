using Auth.Core.Models;
using Auth.Core.Repos;
using Microsoft.Extensions.Logging;

namespace Auth.Core.Services
{
    public interface IAuthService 
    {

        Task<Models.Auth> MakeAuthRecord(AuthInfo authInfo);

        Task<Models.Auth> GetAuthRecord(string token);
    }

    public class AuthService : IAuthService
    {
        private readonly IAuthRepo _repo;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IAuthRepo repo, ILogger<AuthService> logger) 
        { 
            _repo = repo;
            _logger = logger;
        }

        public async Task<Models.Auth> GetAuthRecord(string token) 
        { 
            return new Models.Auth(token, _repo);
        }

        public async Task<Models.Auth> MakeAuthRecord(AuthInfo authInfo)
        {
            var token = Utils.MakeToken(128);
            await _repo.SetAuthInfo(token, authInfo);

            return new Models.Auth(token, _repo);
        }
    }
}
