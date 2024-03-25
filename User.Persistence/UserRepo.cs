using Microsoft.EntityFrameworkCore;
using User.Core.Models;
using User.Core.Repos;
using User.Persistence.Contexts;
using User.Persistence.Contexts.Models;

namespace User.Persistence
{
    public class UserRepo : IUserRepo
    {
        private IUserContext _context;

        public UserRepo(IUserContext context) 
        { 
            _context = context;
        }

        public async Task AddGroupUser(Guid groupId, Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user is null)
                throw new InvalidDataException("User not found");

            var group = await _context.Groups.Where(x => x.Id == groupId)
                .Include(x => x.Users)
                .FirstOrDefaultAsync();

            if (group is null)
                throw new InvalidDataException("Group not found");

            if (group.Users.Any(x => x.Id == userId))
                throw new InvalidDataException("User already in this group");


            group.Users.Add(user);

            await _context.Ctx.SaveChangesAsync();
        }

        public async Task AddUserGroup(Guid userId, Guid groupId)
        {
            var group = await _context.Groups.FindAsync(groupId);

            if (group is null)
                throw new InvalidDataException("Group not found");

            var user = await _context.Users.Where(x => x.Id == userId)
                .Include(x => x.Groups)
                .FirstOrDefaultAsync();

            if (user is null)
                throw new InvalidDataException("User not found");

            if (user.Groups.Any(x => x.Id == groupId))
                throw new InvalidDataException("User already in this group");

            user.Groups.Add(group);

            await _context.Ctx.SaveChangesAsync();
        }

        public async Task DeleteGroupInfo(Guid id)
        {
            var group = await _context.Groups.FindAsync(id);

            if (group is null)
                throw new InvalidDataException("Group not found");

            _context.Groups.Remove(group);
            await _context.Ctx.SaveChangesAsync();
        }

        public async Task DeleteUserInfo(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user is null)
                throw new InvalidDataException("User not found");

            _context.Users.Remove(user);
            await _context.Ctx.SaveChangesAsync();
        }

        public async Task<GroupInfo> GetGroupInfo(Guid id)
        {
            var res = await _context.Groups.FindAsync(id);

            if (res is null)
                throw new InvalidDataException("Group not found");

            return new GroupInfo(res.Name);
        }

        public async Task<List<Guid>> GetGroupUsers(Guid id)
        {
            var res = await _context.Groups.Where(x => x.Id == id)
                .Include(x => x.Users)
                .FirstOrDefaultAsync();

            if (res is null)
                throw new InvalidDataException("Group not found");

            return res.Users.Select(x => x.Id).ToList();
        }

        public async Task<List<Guid>> GetUserGroups(Guid id)
        {
            var res = await _context.Users.Where(x => x.Id == id)
                .Include(x => x.Groups)
                .FirstOrDefaultAsync();

            if (res is null)
                throw new InvalidDataException("User not found");

            return res.Groups.Select(x => x.Id).ToList();
        }

        public async Task<UserInfo> GetUserInfo(Guid id)
        {
            var res = await _context.Users.FindAsync(id);

            if (res is null)
                throw new InvalidDataException("User not found");

            return new UserInfo(res.Name);
        }

        public async Task RemoveGroupUser(Guid groupId, Guid userId)
        {
            var group = await _context.Groups.Where(x => x.Id == groupId)
                .Include(x => x.Users)
                .FirstOrDefaultAsync();

            if (group is null)
                throw new InvalidDataException("Group not found");

            var user = group.Users.Where(x => x.Id == userId).FirstOrDefault();

            if (user is null)
                throw new InvalidDataException("User not found");

            group.Users.Remove(user);

            await _context.Ctx.SaveChangesAsync();
        }

        public async Task RemoveUserGroup(Guid userId, Guid groupId)
        {
            var user = await _context.Users.Where(x => x.Id == userId)
                .Include(x => x.Groups)
                .FirstOrDefaultAsync();

            if (user is null)
                throw new InvalidDataException("User not found");

            var group = user.Groups.Where(x => x.Id == groupId).FirstOrDefault();

            if (group is null)
                throw new InvalidDataException("Group not found");

            user.Groups.Remove(group);

            await _context.Ctx.SaveChangesAsync();
        }

        public async Task<Guid> SetGroupInfo(GroupInfo groupInfo)
        {
            var ezGroup = new EzGroup
            {
                Name = groupInfo.Name,
            };

            _context.Groups.Add(ezGroup);
            await _context.Ctx.SaveChangesAsync();

            return ezGroup.Id;
        }

        public async Task<Guid> SetUserInfo(UserInfo userInfo)
        {
            var ezUser = new EzUser 
            { 
                Name = userInfo.Name, 
            };

            _context.Users.Add(ezUser);
            await _context.Ctx.SaveChangesAsync();

            return ezUser.Id;

        }

        public async Task UpdateGroupInfo(Guid id, GroupInfo groupInfo)
        {
            var group = await _context.Groups.FindAsync(id);

            if (group is null)
                throw new InvalidDataException("Group not found");

            group.Name = groupInfo.Name;

            await _context.Ctx.SaveChangesAsync();
        }

        public async Task UpdateUserInfo(Guid id, UserInfo userInfo)
        {
            var user = await _context.Users.FindAsync(id);

            if (user is null)
                throw new InvalidDataException("User not found");

            user.Name = userInfo.Name;
            
            await _context.Ctx.SaveChangesAsync();
        }

        public async Task UseTransaction(Func<Task> fun)
        {
            using (var trans = await _context.Ctx.Database.BeginTransactionAsync()) 
            {
                await fun();
            }
        }
    }
}
