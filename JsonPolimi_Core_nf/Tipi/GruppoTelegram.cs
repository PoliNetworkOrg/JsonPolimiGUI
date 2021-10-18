using Newtonsoft.Json;
using System;
using Telegram.Bot.Types;

namespace JsonPolimi_Core_nf.Tipi
{
    [Serializable]
    public class GruppoTelegram
    {
        public Chat Chat;
        public DateTime? LastUpdateInviteLinkTime;
        public bool? we_are_admin;

        [JsonConstructor]
        public GruppoTelegram(Chat messageChat, string lastUpdateInviteLinkTime, bool? we_are_admin)
        {
            Chat = messageChat;
            LastUpdateInviteLinkTime = Utils.Dates.DataFromString(lastUpdateInviteLinkTime);
            this.we_are_admin = we_are_admin;
        }
    }
}