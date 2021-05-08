using System.Threading;
using System.Threading.Tasks;
using Business.Wrappers;
using Domain.Repositories;
using Domain.Models;
using System.Linq;

namespace Business.Queries.OrderQueries
{
    public class GetPortfolioOrderByIdQuery : BusinessRequest, IRequestWrapper<Order>
    { 
        public string PortfolioId { get; set; }
        public string OrderId { get; set; }
    }

    public class GetPortfolioOrderByIdQueryHandler : IHandlerWrapper<GetPortfolioOrderByIdQuery, Order>
    {
        private readonly IPortfolioRepository _portfolioRepository;

        public GetPortfolioOrderByIdQueryHandler(IPortfolioRepository portfolioRepository)
        {
            _portfolioRepository = portfolioRepository;
        }

        public async Task<BusinessResponse<Order>> Handle(GetPortfolioOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var portfolio = await _portfolioRepository.GetPortfolioAsync(request.PortfolioId);

            if (portfolio == null)
                return BusinessResponse.Fail<Order>($"No portfolio with the id of {request.PortfolioId} was found");
            
            var order = portfolio.Orders
                .FirstOrDefault(o => o.Id.Equals(request.OrderId));
            if (order == null)
                return BusinessResponse.Fail<Order>($"No order with the id of {request.OrderId} was found");

            return BusinessResponse.Ok<Order>(order);
        }
    }
}