namespace Apo_ChanService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ReportItems", "ReportLat", c => c.Decimal(precision: 10, scale: 7));
            AlterColumn("dbo.ReportItems", "ReportLon", c => c.Decimal(precision: 10, scale: 7));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ReportItems", "ReportLon", c => c.Decimal(nullable: false, precision: 10, scale: 7));
            AlterColumn("dbo.ReportItems", "ReportLat", c => c.Decimal(nullable: false, precision: 10, scale: 7));
        }
    }
}
