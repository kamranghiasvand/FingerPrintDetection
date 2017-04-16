namespace FingerPrintDetectionModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1st : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LogicalUsers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Deleted = c.Boolean(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Plan_Id = c.Long(),
                        Sound_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Plans", t => t.Plan_Id)
                .ForeignKey("dbo.SoundTracks", t => t.Sound_Id)
                .Index(t => t.Plan_Id)
                .Index(t => t.Sound_Id);
            
            CreateTable(
                "dbo.Plans",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Deleted = c.Boolean(nullable: false),
                        Description = c.String(),
                        RepeatNumber = c.Int(nullable: false),
                        MaxNumberOfUse = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        Plan_Id = c.Long(),
                        Sound_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Plans", t => t.Plan_Id)
                .ForeignKey("dbo.SoundTracks", t => t.Sound_Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex")
                .Index(t => t.Plan_Id)
                .Index(t => t.Sound_Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.RealUsers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        FirstFinger = c.Binary(),
                        SecondFinger = c.Binary(),
                        ThirdFinger = c.Binary(),
                        LoginUser_Id = c.Long(),
                        LogicalUser_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.LoginUser_Id)
                .ForeignKey("dbo.LogicalUsers", t => t.LogicalUser_Id)
                .Index(t => t.LoginUser_Id)
                .Index(t => t.LogicalUser_Id);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.Long(nullable: false),
                        RoleId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.SoundTracks",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Uri = c.String(),
                        Duration = c.Long(nullable: false),
                        Type = c.Int(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Deleted = c.Boolean(nullable: false),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.LogicalUsers", "Sound_Id", "dbo.SoundTracks");
            DropForeignKey("dbo.RealUsers", "LogicalUser_Id", "dbo.LogicalUsers");
            DropForeignKey("dbo.LogicalUsers", "Plan_Id", "dbo.Plans");
            DropForeignKey("dbo.AspNetUsers", "Sound_Id", "dbo.SoundTracks");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.RealUsers", "LoginUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "Plan_Id", "dbo.Plans");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.RealUsers", new[] { "LogicalUser_Id" });
            DropIndex("dbo.RealUsers", new[] { "LoginUser_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", new[] { "Sound_Id" });
            DropIndex("dbo.AspNetUsers", new[] { "Plan_Id" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.LogicalUsers", new[] { "Sound_Id" });
            DropIndex("dbo.LogicalUsers", new[] { "Plan_Id" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.SoundTracks");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.RealUsers");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Plans");
            DropTable("dbo.LogicalUsers");
        }
    }
}
