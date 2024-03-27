using ChatSender.Core.Externals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatSender.Core.Models
{
    public class User
    {
        private IUserApi _userApi;
        private Guid _id;

        public Guid Id { get => _id; }

        public User(IUserApi userApi, Guid userId)
        {
            _userApi = userApi;
            _id = userId;
        }

        public async Task<UserInfo> GetInfo()
        {
            return await _userApi.GetUserInfo(_id);
        }

        public async Task<List<Group>> GetGroups() 
        {
            var groups = (await _userApi.GetUserGroups(_id))
                .Select(x => new Group(_userApi, x))
                .ToList();

            return groups;
        }

    }
}
