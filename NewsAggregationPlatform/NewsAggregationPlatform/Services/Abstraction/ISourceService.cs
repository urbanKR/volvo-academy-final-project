using NewsAggregationPlatform.Models.Entities;

namespace NewsAggregationPlatform.Services.Abstraction
{
    public interface ISourceService
    {
        Task<IEnumerable<Source>> GetSourcesAsync();
        Task<Source> GetSourceByIdAsync(Guid id);
        bool AddSource(Source source);
        bool UpdateSource(Source source);
        bool DeleteSource(Source source);
        bool Save();
        Task<Source> GetSourceByNameAsync(string name);
    }
}
