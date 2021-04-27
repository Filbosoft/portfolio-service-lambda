using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Business.Wrappers;
using Domain.Repositories;
using Domain.Models;

namespace Business.Queries
{
    public class GetPortfolioByIdQuery : BusinessRequest, IRequestWrapper<Portfolio>
    { 
        public string PortfolioId { get; set; }
    }

    public class GetPortfolioByIdQueryHandler : IHandlerWrapper<GetPortfolioByIdQuery, Portfolio>
    {
        private readonly IPortfolioRepository _portfolioRepository;

        public GetPortfolioByIdQueryHandler(IPortfolioRepository portfolioRepository)
        {
            _portfolioRepository = portfolioRepository;
        }

        public async Task<BusinessResponse<Portfolio>> Handle(GetPortfolioByIdQuery request, CancellationToken cancellationToken)
        {
            var portfolio = await _portfolioRepository.GetPortfolioAsync(request.PortfolioId);

            return BusinessResponse.Ok<Portfolio>(portfolio);
        }
    }
}