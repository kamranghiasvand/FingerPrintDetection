namespace FingerPrintDetectionModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _3rd : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "Plan_Id", "dbo.Plans");
            DropForeignKey("dbo.AspNetUsers", "Sound_Id", "dbo.SoundTracks");
            DropIndex("dbo.AspNetUsers", new[] { "Plan_Id" });
            DropIndex("dbo.AspNetUsers", new[] { "Sound_Id" });
            DropColumn("dbo.AspNetUsers", "Plan_Id");
            DropColumn("dbo.AspNetUsers", "Sound_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Sound_Id", c => c.Long());
            AddColumn("dbo.AspNetUsers", "Plan_Id", c => c.Long());
            CreateIndex("dbo.AspNetUsers", "Sound_Id");
            CreateIndex("dbo.AspNetUsers", "Plan_Id");
            AddForeignKey("dbo.AspNetUsers", "Sound_Id", "dbo.SoundTracks", "Id");
            AddForeignKey("dbo.AspNetUsers", "Plan_Id", "dbo.Plans", "Id");
        }
    }
}
