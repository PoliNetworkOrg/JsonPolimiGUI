using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace JsonPolimi.Tipi
{
    [Serializable]
    public class GruppoTelegram
    {
        public List<ChatMember> Admins;
        public Chat Chat;
        public bool IsBot;
        public DateTime? LastUpdateInviteLinkTime;

        [JsonConstructor]
        public GruppoTelegram(Chat messageChat, List<ChatMember> admins, bool isBot, DateTime? lastUpdateInviteLinkTime)
        {
            Chat = messageChat;
            Admins = admins;
            IsBot = isBot;
            LastUpdateInviteLinkTime = lastUpdateInviteLinkTime;
        }
    }
}