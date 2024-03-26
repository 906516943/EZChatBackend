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
            var respone = await _httpClient.GetAsync($"/AuthApi/AuthInfo?token={token}");

            if (!respone.IsSuccessStatusCode)
                throw new InvalidDataException("Failed to access /AuthApi/AuthInfo");

            var str = await respone.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AuthInfo>(str, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
    }
}
