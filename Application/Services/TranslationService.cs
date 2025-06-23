using Hengeler.Application.DTOs.Translation;
using Hengeler.Application.Interfaces;
using Hengeler.Domain.Interfaces;

namespace Hengeler.Application.Services;

public class TranslationService(ITranslationDomainService translationDomainService) : ITranslationService
{
    private readonly ITranslationDomainService _translationDomainService = translationDomainService;

    public async Task<IEnumerable<TranslationDto>> GetAllTranslationsAsync(CancellationToken cancellationToken = default)
    {
        var translations = await _translationDomainService.GetAllTranslationsAsync();
        return translations.Select(ToDto);
    }

    public async Task<IEnumerable<TranslationDto>> GetTranslationsByKeysAsync(IEnumerable<Guid> keys, CancellationToken cancellationToken = default)
    {
        var translations = await _translationDomainService.GetTranslationsAsync(keys);
        return translations.Select(ToDto);
    }

    public async Task<TranslationDto?> GetTranslationByKeyAsync(Guid key, CancellationToken cancellationToken = default)
    {
        var translation = await _translationDomainService.GetTranslationAsync(key);
        return translation is not null ? ToDto(translation) : null;
    }

    private static TranslationDto ToDto(Domain.Entities.Translations t) => new()
    {
        Key = t.Key,
        Uk = t.Uk,
        En = t.En,
        De = t.De
    };
}
