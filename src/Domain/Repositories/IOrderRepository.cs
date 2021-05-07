using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;
using Amazon.DynamoDBv2.DataModel;

namespace Domain.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> PlaceOrderAsync(string portfolioId, Order order);
        Task<Order> UpdateOrderAsync(string portfolioId, Order order);
        Task<Order> GetPortfolioOrderAsync(string portfolioId, string orderId);
    }
}