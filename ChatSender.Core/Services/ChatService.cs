using ChatSender.Core.Externals;
using ChatSender.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace ChatSender.Core.Services
{
    public interface IChatService 
    {
        Task<Models.User> GetUserByToken(string token);

        Models.Group GetGroup(Guid id);

        Task<Message> MakeReturnMessage(Message msg, Models.User user, DateTime lastSentTime);
    }

    public class ChatService : IChatService
    {
        private readonly IUserApi _userApi;
        private readonly IAuthApi _authApi;
        private readonly IImageApi _imageApi;

        public ChatService(IUserApi userApi, IAuthApi authApi, IImageApi imageApi) 
        {
            _userApi = userApi;
            _authApi = authApi;
            _imageApi = imageApi;
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

        public async Task<Message> MakeReturnMessage(Message message, Models.User user, DateTime lastSentTime)
        {
            var ret = new Message();


            //limit message rate to 20ms
            if ((DateTime.UtcNow - lastSentTime).Milliseconds < 20)
                throw new InvalidDataException("Last sent time interval is < 20ms");


            //limit text to 2000 chars
            if ((message.Text ?? "").Length > 2000)
                throw new InvalidDataException("Text is either too long or empty");


            //make sure user in to the group
            var userGroups = await user.GetGroups();
            if (!userGroups.Any(x => message.ChannelId == x.Id))
                throw new InvalidDataException("User is not in this group");


            //make sure imgs are valid
            List<ImageInfo>? images = null;
            if (message.ImagesByHash is not null) 
            {
                if (message.ImagesByHash.Count > 10)
                    throw new InvalidDataException("Number of images exceeds the limit");

                images = await _imageApi.GetImageIdFromHash(message.ImagesByHash);
                var hashSet = images.Select(x => x.hash).ToHashSet();

                if(message.ImagesByHash.Any(x => !hashSet.Contains(x)))
                    throw new InvalidDataException("Invalid image hash");
            }

            ret.MessageId = Guid.NewGuid();
            ret.ChannelId = message.ChannelId;
            ret.SenderId = user.Id;
            ret.TimeStamp = (long)(DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds * 1000);
            ret.Text = message.Text;
            ret.ImagesById = images;

            return ret;
        }
    }
}
