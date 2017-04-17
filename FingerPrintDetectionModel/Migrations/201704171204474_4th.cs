namespace FingerPrintDetectionModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _4th : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RealUsers", "LoginUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.RealUsers", new[] { "LoginUser_Id" });
            AddColumn("dbo.RealUsers", "Deleted", c => c.Boolean(nullable: false));
            DropColumn("dbo.RealUsers", "LoginUser_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RealUsers", "LoginUser_Id", c => c.Long());
            DropColumn("dbo.RealUsers", "Deleted");
            CreateIndex("dbo.RealUsers", "LoginUser_Id");
            AddForeignKey("dbo.RealUsers", "LoginUser_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
