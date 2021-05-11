using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Domain.Models;
using Domain.Repositories;

namespace DataAccess.Repositories
{    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly IDynamoDBContext _context;

        public PortfolioRepository(IDynamoDBContext context)
        {
            _context = context;
        }

        public async Task<Portfolio> CreatePortfolioAsync(Portfolio portfolio)
        {
            portfolio.Id = Guid.NewGuid().ToString();
            await _context.SaveAsync(portfolio);
            return portfolio;
        }

        public async Task<Portfolio> GetPortfolioAsync(string id, string ownerId)
        {
            var portfolio = await _context.LoadAsync<Portfolio>(id, ownerId);
            return portfolio;
        }

        public async Task<IEnumerable<Portfolio>> GetPortfoliosAsync(IEnumerable<ScanCondition> conditions = default)
        {
            var portfolios = await _context.ScanAsync<Portfolio>(conditions).GetRemainingAsync();

            return portfolios;
        }

        public async Task<Portfolio> UpdatePortfolioAsync(Portfolio portfolio)
        {
            await _context.SaveAsync(portfolio);
            return portfolio;
        }

        public async Task<bool> DeletePortfolioAsync(string id, string ownerId)
        {
            await _context.DeleteAsync<Portfolio>(id, ownerId);
            return true;
        }
    }
}