using Hengeler.Domain.Entities;
using Hengeler.Infrastructure;
using Microsoft.EntityFrameworkCore;

public class ContactSeeder(AppDbContext context)
{
  private readonly AppDbContext _context = context;

  public async Task SeedAsync()
  {
    bool exists = await _context.Contacts.AnyAsync();

    if (!exists)
    {
      var contact = new Contact
      {
        Name = "Max Mustermann",
        Email = "hengeler.shofrohnhofen3@gmail.com",
        PhoneNumber = "+49 123 45678",
        City = "Altusried",
        Street = "Frohnhofen 3",
        PostalCode = "87452",
        Country = "Deutschland",
        Facebook = "https://www.facebook.com/profile.php?id=61574880403131&rdid=2Shf0hUhGskhz29d&share_url=https%3A%2F%2Fwww.facebook.com%2Fshare%2F16aatisrdk%2F",
        Instagram = "https://www.instagram.com/hengeler.shof/?igsh=ZXlmN3FrcWZma2xu",
        Telegram = "",
        Whatsapp = ""
      };

      _context.Contacts.Add(contact);

    }

    await _context.SaveChangesAsync();

    bool bookingFeatureExists = await _context.Features
      .AnyAsync(f => f.FeatureName == "booking-enabled");

    if (!bookingFeatureExists)
    {
      var bookingFeature = new Feature
      {
        FeatureName = "booking-enabled",
        IsActive = true
      };

      _context.Features.Add(bookingFeature);
    }

    await _context.SaveChangesAsync();
  }
}
