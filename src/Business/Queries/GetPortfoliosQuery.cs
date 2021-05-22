using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using AutoMapper;
using Business.Wrappers;
using Conditus.Trader.Domain.Models;
using Conditus.Trader.Domain.Entities;

namespace Business.Queries
{
    public class GetPortfoliosQuery : BusinessRequest, IRequestWrapper<IEnumerable<PortfolioOverview>>
    { 
        public string OwnerId { get; set; }
    }

    public class GetPortfoliosQueryHandler : IHandlerWrapper<GetPortfoliosQuery, IEnumerable<PortfolioOverview>>
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IMapper _mapper;

        public GetPortfoliosQueryHandler(IPortfolioRepository portfolioRepository, IMapper mapper)
        {
            _portfolioRepository = portfolioRepository;
            _mapper = mapper;
        }

        public async Task<BusinessResponse<IEnumerable<PortfolioOverview>>> Handle(GetPortfoliosQuery request, CancellationToken cancellationToken)
        {
            var scanConditions = new List<ScanCondition>();

            if (request.OwnerId != null)
                scanConditions.Add(new ScanCondition(nameof(PortfolioEntity.Owner), ScanOperator.Equal, request.OwnerId));

            var portfolios = await _portfolioRepository.GetPortfoliosAsync(scanConditions);

            return BusinessResponse.Ok<IEnumerable<PortfolioOverview>>(portfolios);
        }
    }
}