using ChatSender.Core.Externals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatSender.Core.Models
{
    public class Group
    {
        private IUserApi _userApi;
        private Guid _id;

        public Guid Id { get => _id; }

        public Group(IUserApi userApi, Guid id)
        {
            _userApi = userApi;
            _id = id;
        }

        public async Task<GroupInfo> GetInfo() 
        {
            return await _userApi.GetGroupInfo(_id);
        }

        public async Task<List<Models.User>> GetUsers() 
        {
            return (await _userApi.GetGroupUsers(_id))
                .Select(x => new Models.User(_userApi, x))
                .ToList();
        }
    }
}
