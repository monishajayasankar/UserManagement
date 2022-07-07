using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.API
{
    public class DataSeeder
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<AppUser> userManager;
        private readonly UserManagementContext context;

        public DataSeeder(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, UserManagementContext context)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.context = context;
        }



        public async Task SeedRoles() 
        {
            var role1 = new IdentityRole { Name = "Admin" };
            var role2 = new IdentityRole { Name = "Employee" };
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(role1);
            if (!await roleManager.RoleExistsAsync("Employee"))
                await roleManager.CreateAsync(role2);
        }


        public async Task SeedAdmin()
        {
            var Admin = new AppUser
            {
                FullName = "Admin",
                Email = "admin@test.com",
                UserName = "admin@test.com",
                PhoneNumber = "1234567890"
            };


            if (await userManager.FindByNameAsync("admin@test.com") == null)
            {
                await userManager.CreateAsync(Admin, "Admin@1234");
                await userManager.AddToRoleAsync(Admin, "Admin");
                var admindetails = new Admin() { AppUser = Admin };
                context.Admins.Add(admindetails);
                await context.SaveChangesAsync();
            }



        }

    }
}
