using ChatSender.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatSender.Core.Services
{
    public class GlobalService
    {
        private IDictionary<string, Models.User> _connectionId2UserLookup = new ConcurrentDictionary<string, Models.User>();
        private IDictionary<string, DateTime> _connectionId2LastSentTime = new ConcurrentDictionary<string, DateTime>();
        private IDictionary<Guid, string> _userId2ConnectionIdLookup = new ConcurrentDictionary<Guid, string>();
        
        public GlobalService()
        {

        }

        public void OnUserConnect(string connectionId, Models.User user) 
        {
            _connectionId2UserLookup.Add(connectionId, user);
            _connectionId2LastSentTime.Add(connectionId, DateTime.MinValue);
            _userId2ConnectionIdLookup.Add(user.Id, connectionId);
        }

        public void OnUserDisconnect(string connectionId) 
        {
            _connectionId2UserLookup.TryGetValue(connectionId, out var user);

            if (user is not null) 
            {
                _connectionId2UserLookup.Remove(connectionId);
                _connectionId2LastSentTime.Remove(connectionId);
                _userId2ConnectionIdLookup.Remove(user.Id);
            }
        }

        public Models.User GetUser(string connectionId) 
        {
            _connectionId2UserLookup.TryGetValue(connectionId, out var user);

            if (user is null)
                throw new InvalidDataException("User not found");

            return user;
        }

        public string GetConnectionId(Guid userId) 
        {
            _userId2ConnectionIdLookup.TryGetValue(userId, out var connectionId);

            if (connectionId is null)
                throw new InvalidDataException("Connection ID not found");

            return connectionId;
        }

        public DateTime GetUserLastSentTime(string connectionId) 
        {
            _connectionId2LastSentTime.TryGetValue(connectionId, out var userLastSentTime);

            if (userLastSentTime == default(DateTime))
                throw new InvalidDataException("User not found");

            return userLastSentTime;
        }

        public void UpdateUserLastSentTime(string connectionId)
        {
            try
            {
                _connectionId2LastSentTime[connectionId] = DateTime.UtcNow;
            }
            catch
            {
                throw new InvalidDataException("User not found");
            }
        }
    }
}
