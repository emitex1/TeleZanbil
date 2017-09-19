using ir.EmIT.EmITBotNet;
using ir.EmIT.TeleZanbil.Models;
using System;
using System.Data.Entity;

namespace ir.EmIT.TeleZanbil
{
    internal class TeleZanbilContext : EmITBotNetContext
    {
        public TeleZanbilContext():base()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Directory.GetCurrentDirectory());
        }

        public DbSet<User> users { get; set; }
        public DbSet<Role> roles { get; set; }
    }
}