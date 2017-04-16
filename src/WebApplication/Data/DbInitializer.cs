using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Data
{
    public class DbInitializer
    {
        private static readonly string[] Roles = new string[] { "Admin", "User" };

        public static async Task SeedRoles(IServiceProvider serviceProvider)
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                dbContext.Database.EnsureCreated();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                foreach (var role in Roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
                if (!dbContext.Users.Any(t => t.UserName == "admin@admin.com"))
                {
                    var user = new ApplicationUser
                    {
                        UserName = "admin@admin.com",
                        Email = "admin@admin.com",
                    };
                    await userManager.CreateAsync(user, "P@ssword1");
                    
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
        public static void Initialze(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            context.Database.EnsureCreated();
            var userStore = new UserStore<ApplicationUser>(context);
            //RoleManager<IdentityUser> roleManager = new RoleManager<IdentityUser>(userStore,null,null,null,null,null);
            if (!context.Users.Any(t => t.UserName == "admin@admin.com"))
            {
                var user = new ApplicationUser
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com",
                };
                userManager.CreateAsync(user, "P@ssword1");
                if (!context.Roles.Any(t => t.Name == "admin"))
                {
                   // roleManager.CreateAsync(new IdentityUser("Admin"));
                }
                userManager.AddToRoleAsync(user, "admin");
            }
        }
    }
}
