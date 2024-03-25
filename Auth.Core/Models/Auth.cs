using Auth.Core.Repos;
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

        private readonly IAuthRepo _authRepo;

        public Auth(string token, IAuthRepo repo) 
        {
            Token = token;
            _authRepo = repo;
        }

        public async Task<AuthInfo> GetAuthInfo() 
        {
            return await _authRepo.GetAuthInfo(Token);
        }

        public async Task UpdateExpireDate(DateTime newDate) 
        {
            var authInfo = await _authRepo.GetAuthInfo(Token);
            var newAuthInfo = authInfo with { ExpireDate = newDate };

            await _authRepo.SetAuthInfo(Token, newAuthInfo);
        }
    }
}
