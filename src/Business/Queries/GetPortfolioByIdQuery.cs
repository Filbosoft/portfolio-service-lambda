using System.Threading;
using System.Threading.Tasks;
using Business.Wrappers;
using Domain.Repositories;
using Conditus.Trader.Domain.Models;

namespace Business.Queries
{
    public class GetPortfolioByIdQuery : BusinessRequest, IRequestWrapper<PortfolioDetail>
    { 
        public string PortfolioId { get; set; }
    }

    public class GetPortfolioByIdQueryHandler : IHandlerWrapper<GetPortfolioByIdQuery, PortfolioDetail>
    {
        private readonly IPortfolioRepository _portfolioRepository;

        public GetPortfolioByIdQueryHandler(IPortfolioRepository portfolioRepository)
        {
            _portfolioRepository = portfolioRepository;
        }

        public async Task<BusinessResponse<PortfolioDetail>> Handle(GetPortfolioByIdQuery request, CancellationToken cancellationToken)
        {
            var portfolio = await _portfolioRepository.GetPortfolioAsync(request.PortfolioId, request.RequestingUserId);

            if (portfolio == null)
                return BusinessResponse.Fail<PortfolioDetail>($"No portfolio with the id of {request.PortfolioId} was found");

            return BusinessResponse.Ok<PortfolioDetail>(portfolio);
        }
    }
}