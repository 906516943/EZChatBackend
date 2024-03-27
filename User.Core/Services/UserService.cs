using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using User.Core.Models;
using User.Core.Repos;

namespace User.Core.Services
{
    public interface IUserService 
    {
        Task<Models.User> MakeNewUser(UserInfo info);

        Task RemoveUser(Models.User user);

        Models.User GetUser(Guid id);



        Task<Models.Group> MakeNewGroup(GroupInfo group);

        Task RemoveGroup(Models.Group group);

        Models.Group GetGroup(Guid id);
    }

    public class UserService: IUserService
    {
        private readonly IUserRepo _repo;

        public UserService(IUserRepo repo) 
        {
            _repo = repo;
        }

        public async Task<Models.User> MakeNewUser(UserInfo info) 
        {
            var userId = await _repo.SetUserInfo(info);

            //user default added to the world channel
            var worldChannelId = new Group(_repo, await _repo.GetGroupIdByName("World Channel"));
            var user = new Models.User(_repo, userId);

            await user.AddGroup(worldChannelId);

            return user;
        }

        public async Task RemoveUser(Models.User user) 
        {
            await _repo.DeleteUserInfo(user.Id);
        }

        public async Task<Models.Group> MakeNewGroup(GroupInfo group) 
        {
            return new Models.Group(_repo, await _repo.SetGroupInfo(group));
        }

        public async Task RemoveGroup(Models.Group group) 
        { 
            await _repo.DeleteGroupInfo(group.Id);
        }

        public Models.User GetUser(Guid id) 
        {
            return new Models.User(_repo, id);
        }

        public Models.Group GetGroup(Guid id) 
        {
            return new Models.Group(_repo, id);
        }
    }
}
