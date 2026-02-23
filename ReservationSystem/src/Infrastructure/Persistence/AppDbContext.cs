using Microsoft.EntityFrameworkCore;
using ReservationSystem.src.Domain.Entities;

namespace ReservationSystem.src.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Slot> Slots { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reservation>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Slot>()
                .HasKey(x => x.Id);
        }
    }
}
