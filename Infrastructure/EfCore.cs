using Hengeler.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hengeler.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
  public DbSet<User> Users { get; set; } = null!;
  public DbSet<Booking> Bookings { get; set; } = null!;
  public DbSet<Translations> Translations { get; set; } = null!;
  public DbSet<Event> Events { get; set; } = null!;
  public DbSet<Room> Rooms { get; set; } = null!;
  public DbSet<Slider> Sliders { get; set; } = null!;
  public DbSet<Slide> Slides { get; set; } = null!;
  public DbSet<Contact> Contacts { get; set; } = null!;

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<User>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.HasIndex(e => e.Email).IsUnique();
      entity.Property(e => e.Username).IsRequired().HasMaxLength(200);
      entity.Property(e => e.Email).HasMaxLength(100);
      entity.Property(e => e.PhoneNumber).HasMaxLength(15);
      entity.Property(e => e.ProfilePictureUrl).HasMaxLength(200);
    });

    modelBuilder.Entity<Booking>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.HasIndex(e => new { e.StartDate, e.EndDate });
      entity.HasIndex(e => e.RoomId);
      entity.Property(e => e.RoomId)
            .IsRequired()
            .HasMaxLength(100);
      entity.Property(e => e.Status)
            .IsRequired();
    });
  }

}