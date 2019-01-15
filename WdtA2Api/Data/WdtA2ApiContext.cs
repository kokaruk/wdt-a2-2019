using Microsoft.EntityFrameworkCore;

namespace WdtA2Api.Models
{
    public class WdtA2ApiContext : DbContext
    {
        public WdtA2ApiContext(DbContextOptions<WdtA2ApiContext> options)
            : base(options)
        {
        }

        public DbSet<Room> Room { get; set; }

        public DbSet<Slot> Slot { get; set; }

        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Slot>().HasKey(slot => new { slot.RoomID, slot.StartTime });
        }
    }
}
