namespace Apo_ChanService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserItems", "UserName", c => c.String(maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserItems", "UserName");
        }
    }
}
