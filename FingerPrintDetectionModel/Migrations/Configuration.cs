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
            if (temp != null) return;
            var role = new Role
            {
                Name = "Admin"
            };
            context.Roles.Add(role);
            context.SaveChanges();
            context.Entry(role).Reload();
        }
        private static void CreateUser(ApplicationDbContext context)
        {
            var userManager = new UserManager<LoginUser, long>(new UserStore(context));

            var temp = context.Users.FirstOrDefault(m => m.UserName == "Admin");
            if (temp != null) return;
            var user = new LoginUser
            {
                UserName = "Admin",
                Email = "Admin@Admin.com",

            };
            userManager.Create(user, "123456");
            context.SaveChanges();
            userManager.AddToRole(user.Id, "Admin");
        }
    }
}
