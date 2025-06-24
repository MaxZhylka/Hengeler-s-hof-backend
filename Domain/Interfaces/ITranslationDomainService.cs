using Hengeler.Domain.Entities;

namespace Hengeler.Domain.Interfaces;
public interface ITranslationDomainService
{
    Task<IEnumerable<Translations>> GetTranslationsAsync(IEnumerable<Guid> keys);
    Task<IEnumerable<Translations>> GetAllTranslationsAsync();
    Task<Translations?> GetTranslationAsync(Guid key);
    Task CreateTranslationsAsync(IEnumerable<Translations> translations);
    Task UpdateTranslationsAsync(IEnumerable<Translations> translations);
}