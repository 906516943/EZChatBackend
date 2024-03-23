using Auth.Core.Models;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Auth.Core.Service
{
    public interface IAuthService 
    {
        Task SetAuthInfo(string token, AuthInfo authInfo);

        Task<AuthInfo> GetAuthInfo(string token);

        Task DeleteAuthInfo(string token);

        Task<Models.Auth> MakeAuthRecord(AuthInfo authInfo);

        Task<Models.Auth> GetAuthRecord(string token);
    }

    public class AuthService : IAuthService
    {
        private readonly IDatabase _redis;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IConnectionMultiplexer connection, ILogger<AuthService> logger) 
        { 
            _redis = connection.GetDatabase();
            _logger = logger;
        }

        public async Task SetAuthInfo(string token, AuthInfo authInfo) 
        { 
            if(authInfo.ExpireDate < DateTime.UtcNow)
                throw new InvalidDataException("Invalid expire date");

            var timeDiff = authInfo.ExpireDate - DateTime.UtcNow;
            var res = await _redis.StringSetAsync("auth:token:" + token, JsonSerializer.Serialize(authInfo), timeDiff);

            if (!res)
                throw new InvalidOperationException("Set auth token failed");
        }

        public async Task<AuthInfo> GetAuthInfo(string token) 
        {
            var res = (string?)(await _redis.StringGetAsync("auth:token:" + token));

            if (res is null)
                throw new InvalidOperationException("Get auth token failed");

            return JsonSerializer.Deserialize<AuthInfo>(res)!;
        }

        public async Task DeleteAuthInfo(string token)
        {
            await _redis.StringGetDeleteAsync("auth:token:" + token);
        }

        public async Task<Models.Auth> MakeAuthRecord(AuthInfo authInfo)
        {
            var token = Utils.MakeToken(128);
            await SetAuthInfo(token, authInfo);

            return new Models.Auth(token, this);
        }

        public async Task<Models.Auth> GetAuthRecord(string token) 
        { 
            return new Models.Auth(token, this);
        }
    }
}
