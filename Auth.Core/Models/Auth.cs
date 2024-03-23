using Auth.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Core.Models
{
    public class Auth
    {

        public string Token { get; private set; }

        private readonly IAuthService _authService;

        public Auth(string token, IAuthService service) 
        {
            Token = token;
            _authService = service;
        }

        public async Task<AuthInfo> GetAuthInfo() 
        {
            return await _authService.GetAuthInfo(Token);
        }

        public async Task UpdateExpireDate(DateTime newDate) 
        {
            var authInfo = await _authService.GetAuthInfo(Token);
            var newAuthInfo = authInfo with { ExpireDate = newDate };

            await _authService.SetAuthInfo(Token, newAuthInfo);
        }
    }
}
