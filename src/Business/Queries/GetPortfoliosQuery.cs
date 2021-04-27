using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using AutoMapper;
using Business.Wrappers;
using Domain.Repositories;
using Domain.Models;

namespace Business.Queries
{
    public class GetPortfoliosQuery : BusinessRequest, IRequestWrapper<IEnumerable<Portfolio>>
    { 
        public long? OwnerId { get; set; }
    }

    public class GetPortfoliosQueryHandler : IHandlerWrapper<GetPortfoliosQuery, IEnumerable<Portfolio>>
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IMapper _mapper;

        public GetPortfoliosQueryHandler(IPortfolioRepository portfolioRepository, IMapper mapper)
        {
            _portfolioRepository = portfolioRepository;
            _mapper = mapper;
        }

        public async Task<BusinessResponse<IEnumerable<Portfolio>>> Handle(GetPortfoliosQuery request, CancellationToken cancellationToken)
        {
            var scanConditions = new List<ScanCondition>();

            if (request.OwnerId != null)
                scanConditions.Add(new ScanCondition(nameof(Portfolio.Owner), ScanOperator.Equal, request.OwnerId));

            var portfolios = await _portfolioRepository.GetPortfoliosAsync(scanConditions);

            return BusinessResponse.Ok<IEnumerable<Portfolio>>(portfolios);
        }
    }
}