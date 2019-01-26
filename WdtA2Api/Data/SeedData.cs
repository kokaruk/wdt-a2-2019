using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using WdtModels.ApiModels;

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
                        new Room { RoomID = "A" },
                        new Room { RoomID = "B" },
                        new Room { RoomID = "C" },
                        new Room { RoomID = "D" });

                    context.User.AddRange(
                        new User { UserID = "e12345", Name = "Matt", Email = "e12345@rmit.edu.au" },
                        new User { UserID = "e56789", Name = "Joe", Email = "e56789@rmit.edu.au" },
                        new User { UserID = "s1234567", Name = "Kevin", Email = "s1234567@student.rmit.edu.au" },
                        new User { UserID = "s4567890", Name = "Olivier", Email = "s4567890@student.rmit.edu.au" });

                    context.Faq.AddRange(
                        new Faq
                            {
                                Question = "Sample question",
                                Answer =
                                    "This is a test answer. If you're reading this, then it's a freshly started app and no contend has been provided for FAQ section"
                            });

                    var lis = new List<AccessLevel>();

                    Enum.GetNames(typeof(AccessLevel))
                        .ToList()
                        .ForEach(als => lis.Add(new AccessLevel { Name = als }));

                    context.AddRange(lis);

                    context.SaveChangesAsync();
                }
            }
        }
    }
}
