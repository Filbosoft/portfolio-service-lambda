using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Domain.Models;

namespace Api.Repositories
{
    public interface IPortfolioRepository
    {
        Task<Portfolio> CreatePortfolio(Portfolio portfolio);
        Task<Portfolio> UpdatePortfolio(Portfolio portfolio);
        Task<IEnumerable<Portfolio>> GetPortfolios(long ownerId);
        Task<Portfolio> GetPortfolio(string id);
        Task<bool> DeletePortfolio(string id);
    }
    public class PortfoliosRepository : IPortfolioRepository
    {
        private readonly IDynamoDBContext _context;

        public PortfoliosRepository(IDynamoDBContext context)
        {
            _context = context;
        }

        public async Task<Portfolio> CreatePortfolio(Portfolio portfolio)
        {
            portfolio.Id = Guid.NewGuid().ToString();
            await _context.SaveAsync(portfolio);
            return portfolio;
        }

        public async Task<Portfolio> GetPortfolio(string id)
        {
            var portfolio = await _context.LoadAsync<Portfolio>(id);
            return portfolio;
        }

        public async Task<IEnumerable<Portfolio>> GetPortfolios(long ownerId)
        {
            var scanConditions = new List<ScanCondition>
            {
                new ScanCondition(nameof(Portfolio.Owner), ScanOperator.Equal, ownerId)
            };
            var portfolios = await _context.ScanAsync<Portfolio>(scanConditions).GetRemainingAsync();

            return portfolios;
        }

        public async Task<Portfolio> UpdatePortfolio(Portfolio portfolio)
        {
            await _context.SaveAsync(portfolio);
            return portfolio;
        }

        public async Task<bool> DeletePortfolio(string id)
        {
            await _context.DeleteAsync<Portfolio>(id);
            return true;
        }
    }
}