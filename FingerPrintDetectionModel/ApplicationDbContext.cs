using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FingerPrintDetectionModel
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, Role, long, UserLogin, UserRole, UserClaim>
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += (sender, e) => DateTimeKindAttribute.Apply(e.Entity);
            // Database.SetInitializer<ApplicationDbContext>(null);
            //Database.SetInitializer(new ApplicationDbInitializer());

        }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<RuleDefination>().HasOptional(p => p.Schedule).WithRequired(d => d.RuleDefination);
            modelBuilder.Entity<TargetDatasShortTerm>().Property(e => e.TIME).HasColumnType("datetime2");
            modelBuilder.Entity<TargetDatasLongTerm>().Property(e => e.TIME).HasColumnType("datetime2");
            // modelBuilder.Entity<TargetSystem>().HasOptional(f => f.Folder).WithRequired(s => s.TargetSystem);
        }
        public override int SaveChanges()
        {
            SetDatesToUtc(ChangeTracker.Entries());
            return base.SaveChanges();
            // return this.SaveChangesWithTriggers(base.SaveChanges);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            SetDatesToUtc(ChangeTracker.Entries());
            return base.SaveChangesAsync(cancellationToken);
            // return this.SaveChangesWithTriggersAsync(base.SaveChangesAsync, cancellationToken);
        }

        private static void SetDatesToUtc(IEnumerable<DbEntityEntry> changes)
        {
            foreach (var entity in changes.Where(x => x.State == EntityState.Added || x.State == EntityState.Modified).Select(entry => entry.Entity))
            {
                if (entity == null)
                    return;
                var properties = entity.GetType().GetProperties()
                    .Where(x => x.PropertyType == typeof(DateTime)
                                || x.PropertyType == typeof(DateTime?));
                foreach (var property in properties)
                {
                    var attr = property.GetCustomAttribute<DateTimeKindAttribute>();
                    if (attr == null)
                        continue;
                    var dt = property.PropertyType == typeof(DateTime?)
                        ? (DateTime?)property.GetValue(entity)
                        : (DateTime)property.GetValue(entity);
                    if (dt == null)
                        continue;
                    var value = dt.Value.ToUniversalTime();
                    property.SetValue(entity, value);
                }
            }
        }

        public DbSet<RuleDefination> RuleDefinations { get; set; }
        public DbSet<TargetSystem> TargetSystems { get; set; }

        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<RuleTargetState> RuleTargetStates { get; set; }
        public DbSet<TargetDatasShortTerm> TargetShortTermDatas { get; set; }
        public DbSet<TargetDatasLongTerm> TargetLongTermDatas { get; set; }

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
    public class UserStore : UserStore<ApplicationUser, Role, long, UserLogin, UserRole, UserClaim>
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
