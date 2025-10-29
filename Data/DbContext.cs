using Microsoft.EntityFrameworkCore;
using PortHub.Api.Models;

namespace PortHub.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Airline> Airlines { get; set; }
    public DbSet<Slot> Slots { get; set; }
    public DbSet<Gate> Gates { get; set; }
    public DbSet<Boarding> Boardings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //  CONFIGURACION DE AIRLINNE
        modelBuilder.Entity<Airline>(entity =>
        {
            entity.ToTable("Airlines");
            
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(5);
            entity.Property(e => e.Country).IsRequired().HasMaxLength(50);
            entity.Property(e => e.BaseAddress).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ApiKey).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ApiUrl).IsRequired().HasMaxLength(200);
            
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.ApiKey).IsUnique();
        });

        // CONFIGRACION DE SLOT
        modelBuilder.Entity<Slot>(entity =>
        {
            entity.ToTable("Slots");
            
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            
            entity.Property(e => e.FlightNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.ScheduledTime).IsRequired();
            entity.Property(e => e.Runway).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);
            
            
            entity.HasIndex(e => new { e.Runway, e.ScheduledTime })
                .IsUnique()
                .HasDatabaseName("IX_Slot_Runway_ScheduledTime");

            
            entity.HasOne(e => e.Airline)
                .WithMany(a => a.Slots)
                .HasForeignKey(e => e.AirlineId)
                .OnDelete(DeleteBehavior.Restrict);

            
            entity.HasOne(e => e.Gate)
                .WithMany(g => g.Slots)
                .HasForeignKey(e => e.GateId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
        });

        // CONFIGURACION DE GATE
        modelBuilder.Entity<Gate>(entity =>
        {
            entity.ToTable("Gates");
            
            entity.HasKey(e => e.GateId);
            entity.Property(e => e.GateId).ValueGeneratedOnAdd();
            
            entity.Property(e => e.GateName).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Location).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Available).IsRequired().HasDefaultValue(true);
            
            entity.HasIndex(e => e.GateName).IsUnique();
        });

        // CONFIGURACION DE BOARDING
        modelBuilder.Entity<Boarding>(entity =>
        {
            entity.ToTable("Boardings");
            
            entity.HasKey(e => e.BoardingId);
            entity.Property(e => e.BoardingId).ValueGeneratedOnAdd();
            
            entity.Property(e => e.TicketId).IsRequired();
            entity.Property(e => e.AccessTime).IsRequired();
            entity.Property(e => e.Validation).IsRequired().HasDefaultValue(false);
            
            // Relación con Slot
            entity.HasOne(e => e.Slot)
                .WithMany(s => s.Boardings)
                .HasForeignKey(e => e.SlotId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // DATA INICIAL
        SeedInitialData(modelBuilder);
    }

    private void SeedInitialData(ModelBuilder modelBuilder)
    {
        // Gates iniciales
        modelBuilder.Entity<Gate>().HasData(
            //AGREGAR DATA DE GATES UNA VEZ HECHOS DTOS Y CRUDS
        );

        // Aerolínea de prueba
        modelBuilder.Entity<Airline>().HasData(
            new Airline
            {
                Id = 1,
                Name = "AeroSol",
                Code = "AS",
                Country = "Argentina",
                BaseAddress = "Buenos Aires",
                ApiKey = "AS_DEV_KEY_123456789ABCDEF",
                ApiUrl = "https://localhost:7001/api"
            }
        );
    }
}