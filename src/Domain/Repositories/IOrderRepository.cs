using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> PlaceOrderAsync(string portfolioId, Order order);
        Task<Order> UpdateOrderAsync(string portfolioId, Order order);
    }
}