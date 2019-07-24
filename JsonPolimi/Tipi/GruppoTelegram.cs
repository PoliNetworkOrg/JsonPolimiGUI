using System.Collections.Generic;
using Telegram.Bot.Types;

namespace JsonPolimi.Tipi
{
    public class GruppoTelegram
    {
        public List<ChatMember> Admins;
        public Chat Chat;

        public GruppoTelegram(Chat messageChat, List<ChatMember> admins)
        {
            Chat = messageChat;
            Admins = admins;
        }
    }
}