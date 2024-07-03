using NewsAggregationPlatform.Models.Entities;

namespace NewsAggregationPlatform.WebApi.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
