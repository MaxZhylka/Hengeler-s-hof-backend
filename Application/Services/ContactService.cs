
using Hengeler.Application.DTOs.Contacts;
using Hengeler.Application.Interfaces;
using Hengeler.Domain.Interfaces;

namespace Hengeler.Application.Services;

public class ContactService(
  IContactDomainService domainService
) : IContactService
{
  public async Task<ContactDto> GetContactAsync()
  {
    var contact = await domainService.GetContactAsync();
    return new ContactDto
    {
      Name = contact.Name,
      Email = contact.Email,
      PhoneNumber = contact.PhoneNumber,
      City = contact.City,
      Street = contact.Street,
      PostalCode = contact.PostalCode,
      Country = contact.Country,
      Facebook = contact.Facebook,
      Instagram = contact.Instagram,
      Telegram = contact.Telegram,
      Whatsapp = contact.Whatsapp
    };
  }

  public async Task<ContactDto> UpdateContactAsync(UpdateContactDto dto)
  {
    var updatedContact = await domainService.UpdateContactAsync(new Domain.Entities.Contact
    {
      Name = dto.Name,
      Email = dto.Email,
      PhoneNumber = dto.PhoneNumber,
      City = dto.City,
      Street = dto.Street,
      PostalCode = dto.PostalCode,
      Country = dto.Country,
      Facebook = dto.Facebook,
      Instagram = dto.Instagram,
      Telegram = dto.Telegram,
      Whatsapp = dto.Whatsapp
    });

    return new ContactDto
    {
      Name = updatedContact.Name,
      Email = updatedContact.Email,
      PhoneNumber = updatedContact.PhoneNumber,
      City = updatedContact.City,
      Street = updatedContact.Street,
      PostalCode = updatedContact.PostalCode,
      Country = updatedContact.Country,
      Facebook = updatedContact.Facebook,
      Instagram = updatedContact.Instagram,
      Telegram = updatedContact.Telegram,
      Whatsapp = updatedContact.Whatsapp
    };
  }

    public async Task<ContactDto> UpdateSocialMediaAsync(UpdateContactDto dto)
  {
    var updatedContact = await domainService.UpdateSocialMediaAsync(new Domain.Entities.Contact
    {
      Name = dto.Name,
      Email = dto.Email,
      PhoneNumber = dto.PhoneNumber,
      City = dto.City,
      Street = dto.Street,
      PostalCode = dto.PostalCode,
      Country = dto.Country,
      Facebook = dto.Facebook,
      Instagram = dto.Instagram,
      Telegram = dto.Telegram,
      Whatsapp = dto.Whatsapp
    });

    return new ContactDto
    {
      Name = updatedContact.Name,
      Email = updatedContact.Email,
      PhoneNumber = updatedContact.PhoneNumber,
      City = updatedContact.City,
      Street = updatedContact.Street,
      PostalCode = updatedContact.PostalCode,
      Country = updatedContact.Country,
      Facebook = updatedContact.Facebook,
      Instagram = updatedContact.Instagram,
      Telegram = updatedContact.Telegram,
      Whatsapp = updatedContact.Whatsapp
    };
  }
}
