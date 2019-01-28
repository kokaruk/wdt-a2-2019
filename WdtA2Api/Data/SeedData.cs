using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using WdtModels.ApiModels;

using WdtUtils.Model;

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

                    var lis = new List<AccessLevel>();
                    Enum.GetNames(typeof(UserType)).ToList().ForEach(alvls => lis.Add(new AccessLevel { Name = alvls }));
                    context.AddRange(lis);

                    context.Faq.AddRange(
                        new Faq
                            {
                                Question = "Why I can't see my functions?",
                                Answer = "This is a closed system. You need to have login to use it."
                            },
                        new Faq
                            {
                                Question = "How to get a login?",
                                Answer = "You need to register with your RMIT email or Google account associated with your RMIT account."
                        },
                        new Faq
                            {
                                Question = "How to see available staff?",
                                Answer = "Select 'Staff availability' from menu",
                                AccessName = UserType.Staff.ToString()
                            },
                        new Faq
                            {
                                Question = "How to make a booking?",
                                Answer = "Select 'My bookings' from menu",
                                AccessName = UserType.Staff.ToString()
                        },
                        new Faq
                            {
                                Question = "How to see available rooms for booking?",
                                Answer = "Select 'Rooms availability' from menu",
                                AccessName = UserType.Student.ToString()
                        },
                        new Faq
                            {
                                Question = "How to reserve a slot for booking?",
                                Answer = "Select 'My bookings' from menu and follow the prompts",
                                AccessName = UserType.Student.ToString()
                        });

                    context.SaveChangesAsync();
                }
            }
        }
    }
}
