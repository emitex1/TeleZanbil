namespace ir.EmIT.TeleZanbil.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Families",
                c => new
                    {
                        FamilyId = c.Int(nullable: false, identity: true),
                        FamilyName = c.String(),
                    })
                .PrimaryKey(t => t.FamilyId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Families");
        }
    }
}
