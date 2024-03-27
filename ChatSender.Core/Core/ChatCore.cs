using ChatSender.Core.Models;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatSender.Core.Core
{
    public class ChatCore
    {
        public async Task<Message> MakeReturnMessage(Message message, Models.User user, DateTime lastSentTime) 
        {
            
            if ((DateTime.UtcNow - lastSentTime).Milliseconds < 100)
                throw new InvalidDataException("Last sent time interval is < 100ms");


            if (string.IsNullOrEmpty(message.Text) || message.Text.Length > 2000)
                throw new InvalidDataException("Text is either too long or empty");


            var userGroups = await user.GetGroups();
            if (!userGroups.Any(x => message.ChannelId == x.Id))
                throw new InvalidDataException("User is not in this group");


            var ret = new Message();

            ret.MessageId = Guid.NewGuid();
            ret.ChannelId = message.ChannelId;
            ret.SenderId = user.Id;
            ret.TimeStamp = (long)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds;
            ret.Text = message.Text;
            ret.Images = ret.Images;

            return ret;
        }
    }
}
