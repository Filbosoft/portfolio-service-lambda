using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using AutoMapper;
using Business.Extensions;
using Business.Wrappers;
using Conditus.Trader.Domain.Entities;
using Conditus.Trader.Domain.Models;
using Database.Indexes;

namespace Business.Queries
{
    public class GetPortfolioByIdQuery : BusinessRequest, IRequestWrapper<PortfolioDetail>
    { 
        public string PortfolioId { get; set; }
    }

    public class GetPortfolioByIdQueryHandler : IHandlerWrapper<GetPortfolioByIdQuery, PortfolioDetail>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly IMapper _mapper;

        public GetPortfolioByIdQueryHandler(IAmazonDynamoDB db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<BusinessResponse<PortfolioDetail>> Handle(GetPortfolioByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _db.LoadByLocalIndexAsync<PortfolioEntity>(
                request.RequestingUserId,
                nameof(PortfolioEntity.Id),
                request.PortfolioId,
                PortfolioLocalIndexes.PortfolioIdIndex);

            if (entity == null)
                return BusinessResponse.Fail<PortfolioDetail>($"No portfolio with the id of {request.PortfolioId} was found");

            var portfolioDetail = _mapper.Map<PortfolioDetail>(entity);
            
            return BusinessResponse.Ok<PortfolioDetail>(portfolioDetail);
        }
    }
}