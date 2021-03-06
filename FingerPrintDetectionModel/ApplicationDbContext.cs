﻿using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FingerPrintDetectionModel
{
    public class ApplicationDbContext : IdentityDbContext<LoginUser, Role, long, UserLogin, UserRole, UserClaim>
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {

        }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public DbSet<RealUser> RealUsers { get; set; }
        public DbSet<SoundTrack> SoundTracks { get; set; }
        public DbSet<LogicalUser> LogicalUsers { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<ScannerManagerState> ScannerManagerStates { get; set; }
        
    }
    public class UserClaim : IdentityUserClaim<long>
    {
    }
    public class UserLogin : IdentityUserLogin<long>
    {
    }
    public class UserRole : IdentityUserRole<long>
    {
    }
    public class UserStore : UserStore<LoginUser, Role, long, UserLogin, UserRole, UserClaim>
    {
        public UserStore(ApplicationDbContext context) : base(context)
        {
        }
    }
    public class Role : IdentityRole<long, UserRole>
    {
        public Role() { }
        public Role(string name) { Name = name; }
        public bool Deleted { get; set; }
    }
}
