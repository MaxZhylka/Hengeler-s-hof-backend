namespace Hengeler.Application.DTOs.Translation;

public class TranslationDto
{
    public Guid Key { get; set; }
    public string Uk { get; set; } = string.Empty;
    public string En { get; set; } = string.Empty;
    public string De { get; set; } = string.Empty;
}