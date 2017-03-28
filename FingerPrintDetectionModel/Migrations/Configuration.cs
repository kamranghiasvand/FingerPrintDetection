using System.Data.Entity.Migrations;
using System.Linq;
using Microsoft.AspNet.Identity;

namespace FingerPrintDetectionModel.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            CreateRoles(context);
            CreateUser(context);
        }
        private void CreateRoles(ApplicationDbContext context)
        {

            var temp = context.Roles.FirstOrDefault(m => m.Name == "Admin");
            if (temp == null)
            {
                var role = new Role
                {
                    Name = "Admin"
                };
                context.Roles.Add(role);
                context.SaveChanges();
                context.Entry(role).Reload();


            }
            temp = context.Roles.FirstOrDefault(m => m.Name == "User");
            if (temp == null)
            {

                var role = new Role
                {
                    Name = "User"
                };
                context.Roles.Add(role);
                context.SaveChanges();
                context.Entry(role).Reload();

            }
        }
        private void CreateUser(ApplicationDbContext context)
        {
            var userManager = new UserManager<LogicalUser, long>(new UserStore(context));

            var temp = context.Users.FirstOrDefault(m => m.UserName == "Admin");
            if (temp == null)
            {
                var user = new LogicalUser
                {
                    UserName = "Admin",
                    Email = "Admin@Admin.com",
                    FirstName = "Admin",
                    LastName = "Dashboard",
                    PhoneNumber = "+981233451345",

                };
                userManager.Create(user, "123456");
                context.SaveChanges();
                userManager.AddToRole(user.Id, "Admin");
            }

            temp = context.Users.FirstOrDefault(m => m.UserName == "User");
            if (temp == null)
            {
                var user = new LogicalUser
                {
                    UserName = "User",
                    Email = "User@Admin.com",
                    FirstName = "User",
                    LastName = "Dashboard",
                    PhoneNumber = "+981233451345",

                };
                userManager.Create(user, "123456");
                context.SaveChanges();
                userManager.AddToRole(user.Id, "User");

            }
        }
    }
}
