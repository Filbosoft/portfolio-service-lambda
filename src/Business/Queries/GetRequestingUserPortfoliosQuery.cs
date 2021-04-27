using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Business.Wrappers;
using Domain.Repositories;
using Domain.Models;
using MediatR;

namespace Business.Queries
{
    public class GetRequestingUserPortfoliosQuery : BusinessRequest, IRequestWrapper<IEnumerable<Portfolio>>
    { }

    public class GetRequestingUserPortfoliosQueryHandler : IHandlerWrapper<GetRequestingUserPortfoliosQuery, IEnumerable<Portfolio>>
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IMediator _mediator;
        public GetRequestingUserPortfoliosQueryHandler(IPortfolioRepository portfolioRepository, IMediator mediator)
        {
            _portfolioRepository = portfolioRepository;
            _mediator = mediator;
        }
        public async Task<BusinessResponse<IEnumerable<Portfolio>>> Handle(GetRequestingUserPortfoliosQuery request, CancellationToken cancellationToken)
        {
            var getPortfoliosRequest = new GetPortfoliosQuery { OwnerId = request.RequestingUserId };
            var queryResponse = await _mediator.Send(getPortfoliosRequest);

            if (queryResponse.IsError)
                return BusinessResponse.Fail<IEnumerable<Portfolio>>(queryResponse.Message);
            
            var userPortfolios = queryResponse.Data;

            return BusinessResponse.Ok<IEnumerable<Portfolio>>(userPortfolios);
        }
    }
}