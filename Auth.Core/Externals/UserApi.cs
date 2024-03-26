using Auth.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Auth.Core.Externals
{
    public interface IUserApi 
    {
         Task<Guid> MakeUser(UserInfo userInfo);

         Task<UserInfo> GetUser(Guid id);
    }

    public class UserApi : IUserApi
    {
        private readonly HttpClient _httpClient;

        public UserApi(HttpClient httpClient) 
        { 
            _httpClient = httpClient;
        }

        public async Task<Guid> MakeUser(UserInfo userInfo)
        {
            var str = JsonSerializer.Serialize(userInfo);
            var respone = await _httpClient.PutAsync("/UserApi/User", new StringContent(str, Encoding.UTF8, "application/json"));

            if (!respone.IsSuccessStatusCode)
                throw new InvalidDataException("Failed to access /UserApi/User");

            return JsonSerializer.Deserialize<Guid>(await respone.Content.ReadAsStringAsync())!;
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
