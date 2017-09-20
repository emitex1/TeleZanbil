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

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<Family> Families { get; set; }
    }
}