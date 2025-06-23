using Hengeler.Application.DTOs.Translation;

namespace Hengeler.Application.Interfaces;

public interface ITranslationService
{
    Task<IEnumerable<TranslationDto>> GetAllTranslationsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TranslationDto>> GetTranslationsByKeysAsync(IEnumerable<Guid> keys, CancellationToken cancellationToken = default);
    Task<TranslationDto?> GetTranslationByKeyAsync(Guid key, CancellationToken cancellationToken = default);
}
