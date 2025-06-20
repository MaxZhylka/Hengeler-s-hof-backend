using Hengeler.Domain.Entities;
using Hengeler.Domain.Interfaces;
using Hengeler.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Hengeler.Domain.Services;

public class ContactDomainService(AppDbContext context) : IContactDomainService
{
  private readonly AppDbContext _context = context;

  public async Task<Contact> GetContactAsync()
  {
    return await _context.Contacts.FirstOrDefaultAsync()
    ?? throw new InvalidOperationException("Contact not found.");
  }
  public async Task<Contact> UpdateContactAsync(Contact updated)
  {
    var contact = await _context.Contacts.AsTracking().FirstOrDefaultAsync()
      ?? throw new InvalidOperationException("Contact not found.");

    contact.Name = updated.Name;
    contact.Email = updated.Email;
    contact.PhoneNumber = updated.PhoneNumber;
    contact.City = updated.City;
    contact.Street = updated.Street;
    contact.PostalCode = updated.PostalCode;
    contact.Country = updated.Country;

    await _context.SaveChangesAsync();
    return contact;
  }

  public async Task<Contact> UpdateSocialMediaAsync(Contact updated)
  {
    var contact = await _context.Contacts.AsTracking().FirstOrDefaultAsync()
      ?? throw new InvalidOperationException("Contact not found.");

    contact.Facebook = updated.Facebook;
    contact.Instagram = updated.Instagram;
    contact.Telegram = updated.Telegram;
    contact.Whatsapp = updated.Whatsapp;

    await _context.SaveChangesAsync();
    return contact;
  }
}

