using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using AutoMapper;
using Business.Wrappers;
using Conditus.DynamoDB.MappingExtensions.Mappers;
using Conditus.DynamoDB.QueryExtensions.Extensions;
using Conditus.Trader.Domain.Entities;
using Conditus.Trader.Domain.Entities.Indexes;
using Conditus.Trader.Domain.Models;

namespace Business.Queries.Handlers
{

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
            var entity = await _db.LoadByLocalSecondaryIndexAsync<PortfolioEntity>(
                request.RequestingUserId.GetAttributeValue(),
                request.PortfolioId.GetAttributeValue(),
                PortfolioLocalSecondaryIndexes.PortfolioIdIndex);

            if (entity == null)
                return BusinessResponse.Fail<PortfolioDetail>(
                    GetPortfolioByIdResponseCodes.PortfolioNotFound,
                    $"No portfolio with the id of {request.PortfolioId} was found");

            var portfolioDetail = _mapper.Map<PortfolioDetail>(entity);
            
            return BusinessResponse.Ok<PortfolioDetail>(portfolioDetail);
        }
    }
}