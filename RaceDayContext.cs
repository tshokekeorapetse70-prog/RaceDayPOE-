using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;

namespace RaceDay.Api.Data;

public class RaceDayContext(
    DbContextOptions<RaceDayContext> options) : DbContext(options)
{
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<EventOrganizer> EventOrganizers => Set<EventOrganizer>();
    public DbSet<RaceEvent> Events => Set<RaceEvent>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Participant> Participants => Set<Participant>();
    public DbSet<Registration> Registrations => Set<Registration>();
    public DbSet<Result> Results => Set<Result>();
    public DbSet<WeatherRecord> Weather => Set<WeatherRecord>();
    public DbSet<RouteRecord> Routes => Set<RouteRecord>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<UserAccount>()
            .HasIndex(x => x.Email).IsUnique();

        b.Entity<EventOrganizer>()
            .HasIndex(x => x.Email).IsUnique();

        b.Entity<Participant>()
            .HasIndex(x => x.Email).IsUnique();

        b.Entity<Category>()
            .HasIndex(x => new { x.EventID, x.CategoryName })
            .IsUnique();

        b.Entity<Registration>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_Registration_Status",
                "RegistrationStatus IN ('Pending','Confirmed','Cancelled')"));

        b.Entity<Registration>()
            .HasIndex(x => new
            {
                x.ParticipantID,
                x.EventID,
                x.CategoryID
            }).IsUnique();

        b.Entity<Result>()
            .HasIndex(x => x.RegistrationID).IsUnique();

        b.Entity<RaceEvent>()
            .HasOne<EventOrganizer>()
            .WithMany()
            .HasForeignKey(x => x.OrganizerID)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<Category>()
            .HasOne<RaceEvent>()
            .WithMany()
            .HasForeignKey(x => x.EventID)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<Registration>()
            .HasOne<Participant>()
            .WithMany()
            .HasForeignKey(x => x.ParticipantID)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<Registration>()
            .HasOne<RaceEvent>()
            .WithMany()
            .HasForeignKey(x => x.EventID)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<Registration>()
            .HasOne<Category>()
            .WithMany()
            .HasForeignKey(x => x.CategoryID)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<Result>()
            .HasOne<Registration>()
            .WithOne()
            .HasForeignKey<Result>(x => x.RegistrationID)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<WeatherRecord>()
            .HasOne<RaceEvent>()
            .WithMany()
            .HasForeignKey(x => x.EventID)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<RouteRecord>()
            .HasOne<RaceEvent>()
            .WithMany()
            .HasForeignKey(x => x.EventID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}