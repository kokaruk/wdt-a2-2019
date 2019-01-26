using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Identity;

namespace WdtApiLogin.Areas.Identity.Data
{
    public static class SeedData
    {
        /// <summary>
        /// http://www.binaryintellect.net/articles/5e180dfa-4438-45d8-ac78-c7cc11735791.aspx
        /// </summary>
        public static void SeedUsers(UserManager<WdtApiLoginUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var users = new List<WdtApiLoginUser>
                                {
                                    new WdtApiLoginUser
                                        {
                                            UserName = "e12345", Email = "e12345@rmit.edu.au", Name = "Matt"
                                        },
                                    new WdtApiLoginUser
                                        {
                                            UserName = "e56789", Email = "e56789@rmit.edu.au", Name = "Joe"
                                        },
                                    new WdtApiLoginUser
                                        {
                                            UserName = "s1234567",
                                            Email = "s1234567@student.rmit.edu.auu",
                                            Name = "Kevin"
                                        },
                                    new WdtApiLoginUser
                                        {
                                            UserName = "s4567890",
                                            Email = "s4567890@student.rmit.edu.au",
                                            Name = "Olivier"
                                        }
                                };

                foreach (var user in users)
                {
                    var result = userManager.CreateAsync(user, "password").Result;
                }
            }
        }
    }
}
