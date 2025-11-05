using Microsoft.EntityFrameworkCore;
using PortHub.Api.Models;

namespace PortHub.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Airline> Airlines { get; set; }
        
        //public DbSet<Flight> Flights { get; set; }
        public DbSet<Gate> Gates { get; set; }
        public DbSet<Slot> Slots { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Boarding> Boardings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /* Airline -> Flight
            modelBuilder.Entity<Airline>()
                .HasMany(a => a.Flights)
                .WithOne(f => f.Airline)

            // Flight -> Ticket
            modelBuilder.Entity<Flight>()
                .HasMany(f => f.Tickets)
                .WithOne(t => t.Flight)
                .HasForeignKey(t => t.FlightId);
                
            // Flight -> Slot (One-to-One)
            modelBuilder.Entity<Flight>()
                .HasOne(f => f.Slot)
                .WithOne(s => s.Flight)
                .HasForeignKey<Slot>(s => s.FlightId); 
                */

            // Gate -> Slot (One-to-Many)
            modelBuilder.Entity<Gate>()
                .HasMany(g => g.Slots)
                .WithOne(s => s.Gate)
                .HasForeignKey(s => s.GateId);

            // Slot -> Boarding (One-to-Many)
            modelBuilder.Entity<Slot>()
                .HasMany(s => s.Boardings)
                .WithOne(b => b.Slot)
                .HasForeignKey(b => b.SlotId);
                
            // Ticket -> Boarding (One-to-One)
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Boarding)
                .WithOne(b => b.Ticket)
                .HasForeignKey<Boarding>(b => b.TicketId);
        }
    }
}