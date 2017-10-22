namespace FingerPrintDetectionModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _9th : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Plans", "StartTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Plans", "EndTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.RealUsers", "Birthday", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Logs", "Time", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Logs", "Time", c => c.DateTime(nullable: false));
            AlterColumn("dbo.RealUsers", "Birthday", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Plans", "EndTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Plans", "StartTime", c => c.DateTime(nullable: false));
        }
    }
}
