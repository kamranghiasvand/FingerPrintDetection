namespace FingerPrintDetectionModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2nd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SoundTracks", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SoundTracks", "Name");
        }
    }
}
