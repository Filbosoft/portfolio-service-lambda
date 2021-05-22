using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Business.Wrappers;
using Conditus.Trader.Domain.Models;
using MediatR;

namespace Business.Queries
{
    public class GetRequestingUserPortfoliosQuery : BusinessRequest, IRequestWrapper<IEnumerable<PortfolioOverview>>
    { }

    public class GetRequestingUserPortfoliosQueryHandler : IHandlerWrapper<GetRequestingUserPortfoliosQuery, IEnumerable<PortfolioOverview>>
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IMediator _mediator;
        public GetRequestingUserPortfoliosQueryHandler(IPortfolioRepository portfolioRepository, IMediator mediator)
        {
            _portfolioRepository = portfolioRepository;
            _mediator = mediator;
        }
        public async Task<BusinessResponse<IEnumerable<PortfolioOverview>>> Handle(GetRequestingUserPortfoliosQuery request, CancellationToken cancellationToken)
        {
            var getPortfoliosRequest = new GetPortfoliosQuery { OwnerId = request.RequestingUserId };
            var queryResponse = await _mediator.Send(getPortfoliosRequest);

            if (queryResponse.IsError)
                return BusinessResponse.Fail<IEnumerable<PortfolioOverview>>(queryResponse.Message);
            
            var userPortfolios = queryResponse.Data;

            return BusinessResponse.Ok<IEnumerable<PortfolioOverview>>(userPortfolios);
        }
    }
}