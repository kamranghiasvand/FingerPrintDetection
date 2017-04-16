namespace FingerPrintDetectionModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2nd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Plans", "StartTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Plans", "EndTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Plans", "EndTime");
            DropColumn("dbo.Plans", "StartTime");
        }
    }
}
