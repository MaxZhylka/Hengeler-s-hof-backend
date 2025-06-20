
using Hengeler.Domain.Entities;
namespace Hengeler.Domain.Interfaces;

public interface IContactDomainService
{
  Task<Contact> GetContactAsync();
  Task<Contact> UpdateContactAsync(Contact updated);
  Task<Contact> UpdateSocialMediaAsync(Contact update);
}