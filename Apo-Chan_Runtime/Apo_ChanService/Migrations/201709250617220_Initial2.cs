namespace Apo_ChanService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReportItems", "ReportAddress", c => c.String(maxLength: 512));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReportItems", "ReportAddress");
        }
    }
}
