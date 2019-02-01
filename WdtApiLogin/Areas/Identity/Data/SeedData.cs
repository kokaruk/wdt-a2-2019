using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using WdtUtils;
using WdtUtils.Model;

namespace WdtApiLogin.Areas.Identity.Data
{
    public static class SeedData
    {
        /// <summary>
        /// http://www.binaryintellect.net/articles/5e180dfa-4438-45d8-ac78-c7cc11735791.aspx
        /// https://docs.microsoft.com/en-us/aspnet/core/security/authorization/secure-data?view=aspnetcore-2.2
        /// </summary>
        /// <returns>
        /// Asynchronous Task
        /// </returns>
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var accessLevel in Enum.GetNames(typeof(UserType)).ToList())
            {
                if (!await roleManager.RoleExistsAsync(accessLevel))
                {
                    await roleManager.CreateAsync(new IdentityRole {Name = accessLevel});
                }
            }


            var users = new List<WdtApiLoginUser>
            {
                new WdtApiLoginUser
                {
                    UserName = "e12345", Email = "e12345@rmit.edu.au", Name = "Matt"
                },
                new WdtApiLoginUser {UserName = "e56789", Email = "e56789@rmit.edu.au", Name = "Joe"},
                new WdtApiLoginUser
                {
                    UserName = "s1234567", Email = "s1234567@student.rmit.edu.auu", Name = "Kevin"
                },
                new WdtApiLoginUser
                {
                    UserName = "s4567890", Email = "s4567890@student.rmit.edu.au", Name = "Olivier"
                }
            };

            foreach (var user in users)
            {
                var userId = await EnsureUser(serviceProvider, testUserPw, user);
                var role = user.Email.GetUserRoleFromUserName();
                await EnsureRole(serviceProvider, userId, role);
            }
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider, string uid, string role)
        {
            IdentityResult ir = null;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            if (!await roleManager.RoleExistsAsync(role))
            {
                ir = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<WdtApiLoginUser>>();

            var user = await userManager.FindByIdAsync(uid);

            ir = await userManager.AddToRoleAsync(user, role);

            return ir;
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string testUserPw,
            WdtApiLoginUser userInMemory)
        {
            var userManager = serviceProvider.GetService<UserManager<WdtApiLoginUser>>();

            var user = await userManager.FindByEmailAsync(userInMemory.Email);
            if (user == null)
            {
                await userManager.CreateAsync(userInMemory, testUserPw);
                user = await userManager.FindByEmailAsync(userInMemory.Email);
            }

            return user.Id;
        }
    }
}