using Hengeler.Application.DTOs.Slider;

namespace Hengeler.Application.Interfaces;

public interface ISliderService
{
  Task<SliderDto> GetSliderByIdAsync(string sliderId);
  Task<SliderDto> CreateOrUpdateSliderAsync(CreateSliderDto dto);
}