namespace ir.EmIT.TeleZanbil.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MessageLogs",
                c => new
                    {
                        MessageLogId = c.Int(nullable: false, identity: true),
                        MessageDateTime = c.DateTime(nullable: false),
                        SenderID = c.Int(nullable: false),
                        SenderUserName = c.String(),
                        Message = c.String(),
                    })
                .PrimaryKey(t => t.MessageLogId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        RoleName = c.String(),
                    })
                .PrimaryKey(t => t.RoleId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        TelegramUserID = c.Long(nullable: false),
                        UserRole_RoleId = c.Int(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Roles", t => t.UserRole_RoleId)
                .Index(t => t.UserRole_RoleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "UserRole_RoleId", "dbo.Roles");
            DropIndex("dbo.Users", new[] { "UserRole_RoleId" });
            DropTable("dbo.Users");
            DropTable("dbo.Roles");
            DropTable("dbo.MessageLogs");
        }
    }
}
