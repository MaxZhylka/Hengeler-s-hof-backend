using Hengeler.Application.DTOs.Slides;
using Hengeler.Domain.Entities;

namespace Hengeler.Application.Interfaces;
public interface ISlideService
{
  Task<Guid> CreateSlideAsync(SlideCreateDto dto, IFormFile? imageFile, CancellationToken cancellationToken = default);
  Task UpdateSlideAsync(SlideUpdateDto dto, IFormFile? newImageFile, CancellationToken cancellationToken = default);
  Task<IEnumerable<Slide>> GetAllSlidesAsync(CancellationToken cancellationToken = default);
  Task DeleteSlideAsync(Guid id, CancellationToken cancellationToken = default);
}
