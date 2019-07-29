using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace JsonPolimi.Tipi
{
    [Serializable]
    public class GruppoTelegram
    {
        public Chat Chat;
        public DateTime? LastUpdateInviteLinkTime;

        [JsonConstructor]
        public GruppoTelegram(Chat messageChat, string lastUpdateInviteLinkTime)
        {
            Chat = messageChat;
            LastUpdateInviteLinkTime = Form1.DataFromString(lastUpdateInviteLinkTime);
        }
    }
}