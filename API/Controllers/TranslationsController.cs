using Hengeler.Application.DTOs.Translation;
using Hengeler.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hengeler.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TranslationsController(ITranslationService translationService) : ControllerBase
{
    private readonly ITranslationService _translationService = translationService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TranslationDto>>> GetAllTranslations(CancellationToken cancellationToken)
    {
        var result = await _translationService.GetAllTranslationsAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{key:guid}")]
    public async Task<ActionResult<TranslationDto>> GetTranslationByKey(Guid key, CancellationToken cancellationToken)
    {
        var translation = await _translationService.GetTranslationByKeyAsync(key, cancellationToken);
        return translation is not null ? Ok(translation) : NotFound();
    }

    [HttpPost("batch")]
    public async Task<ActionResult<IEnumerable<TranslationDto>>> GetTranslationsByKeys(
        [FromBody] IEnumerable<Guid> keys,
        CancellationToken cancellationToken)
    {
        if (!keys.Any())
            return BadRequest("Key list must not be empty.");

        var translations = await _translationService.GetTranslationsByKeysAsync(keys, cancellationToken);
        return Ok(translations);
    }
}
