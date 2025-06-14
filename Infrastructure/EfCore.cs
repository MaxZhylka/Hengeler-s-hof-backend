using Microsoft.EntityFrameworkCore;
using Hengeler.Domain.Entities;

namespace Hengeler.Infrastructure;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
  public DbSet<User> Users { get; set; } = null!;

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<User>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Username).IsRequired().HasMaxLength(200);
      entity.Property(e => e.Email).HasMaxLength(100);
      entity.Property(e => e.PhoneNumber).HasMaxLength(15);
      entity.Property(e => e.ProfilePictureUrl).HasMaxLength(200);
    });
  }
}