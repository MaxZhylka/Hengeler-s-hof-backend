using Hengeler.Application.DTOs.Contacts;

namespace Hengeler.Application.Interfaces;

public interface IContactService
{
  Task<ContactDto> GetContactAsync();
  Task<ContactDto> UpdateContactAsync(UpdateContactDto dto);
  Task<ContactDto> UpdateSocialMediaAsync(UpdateContactDto update);
}
