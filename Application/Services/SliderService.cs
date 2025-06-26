using Hengeler.Application.DTOs.Slider;
using Hengeler.Application.Interfaces;
using Hengeler.Domain.Entities;
using Hengeler.Domain.Interfaces;

namespace Hengeler.Application.Services;

public class SliderService(ISliderDomainService sliderDomainService) : ISliderService
{
  private readonly ISliderDomainService _sliderDomainService = sliderDomainService;

  public async Task<SliderDto> GetSliderByIdAsync(string sliderId)
  {
    var slider = await _sliderDomainService.GetSliderByIdAsync(sliderId);
    return MapToDto(slider);
  }

  public async Task<SliderDto> CreateOrUpdateSliderAsync(CreateSliderDto dto)
  {
    var sliderEntity = MapToEntity(dto);
    var updatedSlider = await _sliderDomainService.CreateOrUpdateSliderAsync(sliderEntity);
    return MapToDto(updatedSlider);
  }

  private static SliderDto MapToDto(Slider slider) => new()
  {
    Id = slider.Id,
    SliderId = slider.SliderId,
    Slides = slider.Slides?.Select(s => new Slide
    {
      Id = s.Id,
      ImageUrl = s.ImageUrl,
      TitleKey = s.TitleKey,
      DescriptionKey = s.DescriptionKey,
      Price = s.Price
    }).ToList() ?? []
  };

  private static Slider MapToEntity(CreateSliderDto dto) => new()
  {
    Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
    SliderId = dto.SliderId,
    SlideIds = dto.SlideIds ?? []
  };
}
