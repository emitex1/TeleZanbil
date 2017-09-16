using System;
using System.Collections.Generic;
using ir.EmIT.EmITBotNet;
using Telegram.Bot.Types;

namespace TeleZanbil
{
    class TeleZanbil : EmITBotNetBase
    {
        public override void addNewUserSession(long currentUserID)
        {
            throw new NotImplementedException();
        }

        public override Message convertData(Message m)
        {
            throw new NotImplementedException();
        }

        public override void defineNFARulePostFunctions()
        {
            throw new NotImplementedException();
        }

        public override void defineNFARules()
        {
            throw new NotImplementedException();
        }

        public override List<long> getAuthenticatedUsers()
        {
            throw new NotImplementedException();
        }

        public override void getConvertedSessionData(Message m)
        {
            throw new NotImplementedException();
        }

        public override void initDatabase()
        {
            throw new NotImplementedException();
        }
    }
}
