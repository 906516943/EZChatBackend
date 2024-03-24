using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Core.Models;

namespace User.Core.Repos
{
    public interface IUserRepo
    {
        public Task<UserInfo> GetUserInfo(Guid id);

        public Task<Guid> SetUserInfo(UserInfo userInfo);

        public Task UpdateUserInfo(Guid id, UserInfo userInfo);

        public Task DeleteUserInfo(Guid id);



        public Task<GroupInfo> GetGroupInfo(Guid id);

        public Task<Guid> SetGroupInfo(GroupInfo groupInfo);

        public Task UpdateGroupInfo(Guid id, GroupInfo groupInfo);

        public Task DeleteGroupInfo(Guid id);



        public Task<List<Guid>> GetGroupUsers(Guid id);

        public Task AddGroupUser(Guid groupId, Guid userId);

        public Task RemoveGroupUser(Guid groupId, Guid userId);



        public Task<List<Guid>> GetUserGroups(Guid id);

        public Task AddUserGroup(Guid userId, Guid groupId);

        public Task RemoveUserGroup(Guid userId, Guid groupId);

        public Task UseTransaction(Func<Task> fun);
    }
}
