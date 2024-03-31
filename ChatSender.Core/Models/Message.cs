using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatSender.Core.Models
{
    public class Message
    {
        /// <summary>
        /// Unique message Id
        /// </summary>
        public Guid? MessageId { get; set; }

        /// <summary>
        /// Destination
        /// </summary>
        public Guid ChannelId { get; set; }

        /// <summary>
        /// Sender
        /// </summary>
        public Guid? SenderId { get; set; }

        /// <summary>
        /// UTC unix timestamp
        /// </summary>
        public long? TimeStamp { get; set; }

        /// <summary>
        /// Text
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Images
        /// </summary>
        public List<Guid>? Images { get; set; }

    }
}
