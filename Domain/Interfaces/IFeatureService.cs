using Hengeler.Domain.Entities;

namespace Hengeler.Domain.Interfaces;

public interface IFeatureService
{
  Task<List<Feature>> GetAllAsync();
  Task<bool> UpdateAsync(Feature dto);
}