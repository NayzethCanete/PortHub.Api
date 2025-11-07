using Microsoft.EntityFrameworkCore;
using PortHub.Api.Models;

namespace PortHub.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Airline> Airlines { get; set; }
        public DbSet<Gate> Gates { get; set; }
        public DbSet<Slot> Slots { get; set; }
        public DbSet<Boarding> Boardings { get; set; }
        public DbSet<User> Users { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== CONFIGURACIÓN DE AIRLINES (API KEY) =====
            modelBuilder.Entity<Airline>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Code).IsRequired().HasMaxLength(10);
                entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
                
                entity.Property(a => a.ApiKey).IsRequired().HasMaxLength(100);
                
                // Índices únicos
                entity.HasIndex(a => a.Code).IsUnique();
                entity.HasIndex(a => a.ApiKey).IsUnique();
            });

            // ===== CONFIGURACIÓN DE GATES =====
            modelBuilder.Entity<Gate>(entity =>
            {
                entity.HasKey(g => g.Id);
                entity.Property(g => g.Name).IsRequired().HasMaxLength(50);
                entity.Property(g => g.Location).IsRequired().HasMaxLength(100);
            });

            // ===== CONFIGURACIÓN DE SLOTS (API KEY) =====
            modelBuilder.Entity<Slot>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.ScheduleTime).IsRequired();
                entity.Property(s => s.Runway).IsRequired().HasMaxLength(10);
                entity.Property(s => s.Status).IsRequired().HasMaxLength(20);
                entity.Property(s => s.FlightCode).IsRequired().HasMaxLength(20);

                // CONSTRAINT CRÍTICO: No puede haber dos slots con mismo horario y pista
                entity.HasIndex(s => new { s.ScheduleTime, s.Runway })
                      .IsUnique()
                      .HasDatabaseName("IX_Slots_ScheduleTime_Runway");

                
                entity.HasOne(s => s.Gate)
                      .WithMany(g => g.Slots)
                      .HasForeignKey(s => s.GateId)
                      .OnDelete(DeleteBehavior.SetNull); 
            });

            // ===== CONFIGURACIÓN DE BOARDINGS (Trazabilidad) =====
            modelBuilder.Entity<Boarding>(entity =>
            {
                entity.HasKey(b => b.BoardingId);
            
               
                entity.HasOne(b => b.Slot)
                      .WithMany(s => s.Boardings)
                      .HasForeignKey(b => b.SlotId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(b => new { b.TicketNumber, b.SlotId })
                      .IsUnique()
                      .HasDatabaseName("IX_Boardings_TicketId_SlotId");
            });
            
            // ===== CONFIGURACIÓN DE USUARIOS (JWT) =====
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username)
                    .HasMaxLength(50)
                    .IsRequired();
                
                entity.HasIndex(u => u.Username)
                    .IsUnique();
                
                entity.Property(u => u.PasswordHash)
                    .IsRequired();
            });


            // ===== DATOS INICIALES (SEED) =====
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Gates
            modelBuilder.Entity<Gate>().HasData(
                new Gate { Id = 1, Name = "A1", Location = "Terminal A - Norte" },
                new Gate { Id = 2, Name = "A2", Location = "Terminal A - Norte" },
                new Gate { Id = 3, Name = "B1", Location = "Terminal B - Sur" },
                new Gate { Id = 4, Name = "B2", Location = "Terminal B - Sur" },
                new Gate { Id = 5, Name = "C1", Location = "Terminal C - Internacional" }
            );

            // Seed Airline de prueba (API Key)
            modelBuilder.Entity<Airline>().HasData(
                new Airline
                {
                    Id = 1,
                    Name = "Aerolíneas Argentinas",
                    Code = "AR",
                    Country = "Argentina",
                    BaseAddress = "Buenos Aires",
                    ApiUrl = "http://localhost:5241/api/airline",
                    ApiKey = "AR_KEY_123456789ABCDEF01234"
                }
            );
        }
    }
}