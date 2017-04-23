namespace FingerPrintDetectionModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _7th : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Logs", "PlanId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Logs", "PlanId");
        }
    }
}
