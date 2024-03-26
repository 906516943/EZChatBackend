using ChatSender.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatSender.Core.Externals
{
    public interface IUserApi 
    {
        Task<UserInfo> GetUser(Guid id);
    }

    public class UserApi : IUserApi
    {
        private readonly HttpClient _httpClient;

        public UserApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserInfo> GetUser(Guid id)
        {
            var respone = await _httpClient.GetAsync($"/UserApi/User?id={id.ToString()}");

            if (!respone.IsSuccessStatusCode)
                throw new InvalidDataException("Failed to access /UserApi/User");

            var str = await respone.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UserInfo>(str, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
    }
}
