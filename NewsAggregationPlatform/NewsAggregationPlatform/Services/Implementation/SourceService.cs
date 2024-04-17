using Microsoft.EntityFrameworkCore;
using NewsAggregationPlatform.Data;
using NewsAggregationPlatform.Models.Entities;
using NewsAggregationPlatform.Services.Abstraction;

namespace NewsAggregationPlatform.Services.Implementation
{
    public class SourceService : ISourceService
    {
        private readonly AppDbContext _dbContext;

        public SourceService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Source>> GetSourcesAsync()
        {
            return await _dbContext.Sources.ToListAsync();
        }
        public async Task<Source> GetSourceByIdAsync(Guid id)
        {
            return await _dbContext.Sources.Include(c => c.Articles).FirstOrDefaultAsync(c => c.Id == id);
        }
        public bool AddSource(Source source)
        {
            _dbContext.Sources.Add(source);
            return Save();
        }
        public bool UpdateSource(Source source)
        {
            _dbContext.Sources.Update(source);
            return Save();
        }
        public bool DeleteSource(Source source)
        {
            _dbContext.Remove(source);
            return Save();
        }
        public bool Save()
        {
            var changes = _dbContext.SaveChanges();
            return changes > 0;
        }
    }
}
