using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Conditus.Trader.Domain.Entities;

namespace DataAccess.Repositories
{    
    // public class PortfolioRepository : IPortfolioRepository
    // {
    //     private readonly IDynamoDBContext _context;

    //     public PortfolioRepository(IDynamoDBContext context)
    //     {
    //         _context = context;
    //     }

    //     public async Task<PortfolioEntity> CreatePortfolioAsync(PortfolioEntity portfolio)
    //     {
    //         portfolio.Id = Guid.NewGuid().ToString();
    //         await _context.SaveAsync(portfolio);
    //         return portfolio;
    //     }

    //     public async Task<PortfolioEntity> GetPortfolioAsync(string id, string ownerId)
    //     {
    //         var portfolio = await _context.LoadAsync<PortfolioEntity>(id, ownerId);
    //         return portfolio;
    //     }

    //     public async Task<IEnumerable<PortfolioEntity>> GetPortfoliosAsync(IEnumerable<ScanCondition> conditions = default)
    //     {
    //         var portfolios = await _context.ScanAsync<PortfolioEntity>(conditions).GetRemainingAsync();

    //         return portfolios;
    //     }

    //     public async Task<PortfolioEntity> UpdatePortfolioAsync(PortfolioEntity portfolio)
    //     {
    //         await _context.SaveAsync(portfolio);
    //         return portfolio;
    //     }

    //     public async Task<bool> DeletePortfolioAsync(string id, string ownerId)
    //     {
    //         await _context.DeleteAsync<PortfolioEntity>(id, ownerId);
    //         return true;
    //     }
    // }
}