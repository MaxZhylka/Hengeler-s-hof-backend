using Hengeler.Domain.Entities;

namespace Hengeler.Domain.Services;

public interface ISlideDomainService
{
  Task<IEnumerable<Slide>> GetAllSlidesAsync();
  Task<IEnumerable<Slide>> GetSlidesByIdsAsync(IEnumerable<Guid> ids);
  Task<Slide> CreateSlideAsync(Slide slide);
  Task UpdateSlideAsync(Slide slide);
  Task DeleteSlideAsync(Guid slideId);
}
