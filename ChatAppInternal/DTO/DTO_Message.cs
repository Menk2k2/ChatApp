using System;

namespace DTO
{
    public class DTO_Message
    {
        private int id;
        private int? senderId;
        private int? channelId;
        private string displayName;
        private bool? sent;
        private DateTime? dateSent;
        private bool? seen;
        private DateTime? dateSeen;
        private string content;


        public DTO_Message()
        {
        }

        public DTO_Message(int id, int? SenderId, int? ChannelId,
            string DisplayName, bool? Sent, DateTime? DateSent, bool? Seen, DateTime? DateSeen, string Content)
        {
            this.id = id;
            this.senderId = SenderId;
            this.channelId = ChannelId;
            this.displayName = DisplayName;
            this.sent = Sent;
            this.dateSent = DateSent;
            this.seen = Seen;
            this.dateSeen = DateSeen;
            this.content = Content;
        }

        public int Id { get => id; set => id = value; }
        public int? ChannelId { get => channelId; set => channelId = value; }
        public int? SenderId { get => senderId; set => senderId = value; }
        public string DisplayName { get => displayName; set => displayName = value; }
        public bool? Sent { get => sent; set => sent = value; }
        public bool? Seen { get => seen; set => seen = value; }
        public DateTime? DateSent { get => dateSent; set => dateSent = value; }
        public DateTime? DateSeen { get => dateSeen; set => dateSeen = value; }
        public string Content { get => content; set => content = value; }
    }
}
