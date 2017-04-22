namespace FingerPrintDetectionModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _5th : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RealUsers", "Birthday", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RealUsers", "Birthday");
        }
    }
}
