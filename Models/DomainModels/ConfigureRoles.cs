using Microsoft.AspNetCore.Identity;
using WindTalkerMessenger.Models.DataLayer;

namespace WindTalkerMessenger.Models.DomainModels
{
    public class ConfigureRoles
    {


        public static async Task CreateAdminRole(IServiceProvider provider)
        {
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();

            string username = "admin@email.com";
            string password = "Password123!";
            string roleName = "Admin";

            if(await roleManager.FindByNameAsync(roleName) == null) 
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
            if (await userManager.FindByNameAsync(username) == null)
            {
                ApplicationUser user = new ApplicationUser { UserName = username,ChatName = "Admin" };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, roleName);
                }
            }
        }
    }
}
