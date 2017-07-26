namespace FingerPrintDetectionModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _8th : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ScannerManagerStates",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Started = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ScannerManagerStates");
        }
    }
}
