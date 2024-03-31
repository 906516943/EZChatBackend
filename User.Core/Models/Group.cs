using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Core.Repos;

namespace User.Core.Models
{
    public class Group
    {
        private IUserRepo _repo;
        private Guid _id;
        
        public Guid Id { get => _id; }

        public Group(IUserRepo repo, Guid id) 
        {
            _repo = repo;
            _id = id;
        }

        public async Task<GroupInfo> GetInfo() 
        {
            return (await _repo.GetGroupInfo(new List<Guid>() { _id })).First();
        }

        public async Task UpdateInfo(GroupInfo info) 
        {
            await _repo.UpdateGroupInfo(_id, info);
        }

        public async Task<List<User>> GetUsers() 
        {
            var users = (await _repo.GetGroupUsers(_id))
                .Select(x => new User(_repo, x))
                .ToList();
        
            return users;
        }

        public async Task AddUser(User user) 
        {
            await _repo.AddGroupUser(_id, user.Id);
        }

        public async Task RemoveUser(User user) 
        { 
            await _repo.RemoveGroupUser(_id,user.Id);
        }
    }
}
