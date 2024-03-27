using ChatSender.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatSender.Core.Externals
{
    public interface IAuthApi 
    {
        Task<AuthInfo> GetAuthInfo(string token);
    }

    public class AuthApi : IAuthApi
    {
        private readonly HttpClient _httpClient;

        public AuthApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AuthInfo> GetAuthInfo(string token) 
        {
            return await _httpClient.DoGet<AuthInfo>($"/AuthApi/AuthInfo?token={token}");
        }
    }
}
