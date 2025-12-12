using JOBPORTALWEB.DOMAIN.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace JOBPORTALWEB.INFRASTRUCTURE.Data.Seed
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
        {
            // Lặp qua các giá trị trong Enum UserRole
            foreach (var role in Enum.GetNames(typeof(UserRole)))
            {
                if (await roleManager.FindByNameAsync(role) == null)
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }
        }
    }
}