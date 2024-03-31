using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Core.Repos;

namespace User.Core.Models
{
    public class User
    {
        private Guid _id;
        private IUserRepo _repo;

        public Guid Id { get => _id; }

        public User(IUserRepo repo, Guid id) 
        {
            _repo = repo;
            _id = id;
        }

        public async Task<UserInfo> GetInfo() 
        {
            return (await _repo.GetUserInfo(new List<Guid>() { _id })).First();
        }

        public async Task UpdateInfo(UserInfo info) 
        { 
            await _repo.UpdateUserInfo(_id, info);
        }

        public async Task<List<Group>> GetGroups() 
        {
            var groups = (await _repo.GetUserGroups(_id))
                .Select(x => new Group(_repo, x))
                .ToList();

            return groups;
        }

        public async Task AddGroup(Group group) 
        {
            await _repo.AddUserGroup(_id, group.Id);
        }

        public async Task RemoveGroup(Group group) 
        { 
            await _repo.RemoveUserGroup(_id, group.Id);
        }

    }
}
