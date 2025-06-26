
using Hengeler.Domain.Entities;

namespace Hengeler.Domain.Interfaces;
public interface ISliderDomainService
{
  Task<Slider> GetSliderByIdAsync(string id);
  Task<Slider> CreateOrUpdateSliderAsync(Slider slider);
}
