using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using WdtA2Api.Models;

namespace WdtA2Api.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new WdtA2ApiContext(
                serviceProvider.GetRequiredService<DbContextOptions<WdtA2ApiContext>>()))
            {
                if (!context.Room.Any())
                {
                    context.Room.AddRange(
                        new Room{RoomID = "A"},
                        new Room { RoomID = "B"},
                        new Room { RoomID = "C"},
                        new Room { RoomID = "D" });
                    context.SaveChanges();
                }

                if (!context.User.Any())
                {
                    context.User.AddRange(
                        new User{UserID = "e12345", Name = "Matt", Email = "e12345@rmit.edu.au" },
                        new User { UserID = "e56789", Name = "Joe", Email = "e56789@rmit.edu.au" },
                        new User { UserID = "s1234567", Name = "Kevin", Email = "s1234567@student.rmit.edu.au" },
                        new User { UserID = "s4567890", Name = "Olivier", Email = "s4567890@student.rmit.edu.au" });
                    context.SaveChanges();
                }
            }
        }
    }
}
