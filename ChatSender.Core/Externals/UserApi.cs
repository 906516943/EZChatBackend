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
        Task<UserInfo> GetUserInfo(Guid id);

        Task<List<Guid>> GetUserGroups(Guid userId);

        Task<GroupInfo> GetGroupInfo(Guid id);

        Task<List<Guid>> GetGroupUsers(Guid id);
    }

    public class UserApi : IUserApi
    {
        private readonly HttpClient _httpClient;

        public UserApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<UserInfo> GetUserInfo(Guid id)
        {
            return await _httpClient.DoGet<UserInfo>($"/UserApi/User?id={id}");
        }

        public async Task<List<Guid>> GetUserGroups(Guid userId) 
        {
            return await _httpClient.DoGet<List<Guid>>($"/UserApi/User/{userId}/Groups");
        }

        public async Task<GroupInfo> GetGroupInfo(Guid id) 
        {
            return await _httpClient.DoGet<GroupInfo>($"/UserApi/Group/{id}");
        }

        public async Task<List<Guid>> GetGroupUsers(Guid id) 
        {
            return await _httpClient.DoGet<List<Guid>>($"/UserApi/Group/{id}/Users");
        }
    }
}
