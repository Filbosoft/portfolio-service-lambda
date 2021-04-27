using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;
using Amazon.DynamoDBv2.DataModel;

namespace Domain.Repositories
{
    public interface IPortfolioRepository
    {
        Task<Portfolio> CreatePortfolioAsync(Portfolio portfolio);
        Task<Portfolio> UpdatePortfolioAsync(Portfolio portfolio);
        Task<IEnumerable<Portfolio>> GetPortfoliosAsync(IEnumerable<ScanCondition> conditions);
        Task<Portfolio> GetPortfolioAsync(string id);
        Task<bool> DeletePortfolioAsync(string id);
    }
}