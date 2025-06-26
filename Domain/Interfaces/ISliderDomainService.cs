
namespace Hengeler.Domain.Entities.Interfaces;
public interface ISliderDomainService
{
  Task<Slider> GetSliderByIdAsync(string id);
  Task<Slider> CreateOrUpdateSliderAsync(Slider room);
}
