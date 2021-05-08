using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Business.Wrappers;
using Domain.Repositories;
using Domain.Models;
using MediatR;
using System.Linq;

namespace Business.Queries.PortfolioQueries
{
    public class GetRequestingUserOrdersQuery : BusinessRequest, IRequestWrapper<IEnumerable<Order>>
    { }

    public class GetRequestingUserOrdersQueryHandler : IHandlerWrapper<GetRequestingUserOrdersQuery, IEnumerable<Order>>
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IMediator _mediator;
        public GetRequestingUserOrdersQueryHandler(IPortfolioRepository portfolioRepository, IMediator mediator)
        {
            _portfolioRepository = portfolioRepository;
            _mediator = mediator;
        }
        public async Task<BusinessResponse<IEnumerable<Order>>> Handle(GetRequestingUserOrdersQuery request, CancellationToken cancellationToken)
        {
            var getPortfoliosRequest = new GetPortfoliosQuery { OwnerId = request.RequestingUserId };
            var queryResponse = await _mediator.Send(getPortfoliosRequest);

            if (queryResponse.IsError)
                return BusinessResponse.Fail<IEnumerable<Order>>(queryResponse.Message);
            
            var userPortfolios = queryResponse.Data;
            var userOrders = userPortfolios.SelectMany(p => p.Orders);

            return BusinessResponse.Ok<IEnumerable<Order>>(userOrders);
        }
    }
}