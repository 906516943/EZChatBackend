using ChatSender.Core.Externals;
using ChatSender.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatSender.Core.Services
{
    public interface IChatService 
    {
        Task<Models.User> GetUserByToken(string token);

        Models.Group GetGroup(Guid id);
    }

    public class ChatService : IChatService
    {
        private readonly IUserApi _userApi;
        private readonly IAuthApi _authApi;

        public ChatService(IUserApi userApi, IAuthApi authApi) 
        {
            _userApi = userApi;
            _authApi = authApi;
        }

        public Group GetGroup(Guid id)
        {
            return new Models.Group(_userApi, id);
        }

        public async Task<Models.User> GetUserByToken(string token)
        {
            var authInfo = await _authApi.GetAuthInfo(token);

            if (authInfo.ExpireDate <= DateTime.UtcNow)
                throw new InvalidDataException("User token expired");

            return new Models.User(_userApi, authInfo.UserId);
        }


    }
}
