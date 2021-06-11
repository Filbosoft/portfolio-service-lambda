using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Business.Wrappers;
using Conditus.DynamoDB.MappingExtensions.Mappers;
using Conditus.DynamoDB.QueryExtensions.Extensions;
using Conditus.Trader.Domain.Entities;
using Conditus.Trader.Domain.Entities.Indexes;

namespace Business.Commands.Handlers
{

    public class DeletePortfolioCommandHandler : IHandlerWrapper<DeletePortfolioCommand, bool>
    {
        private readonly IAmazonDynamoDB _db;

        public DeletePortfolioCommandHandler(IAmazonDynamoDB db)
        {
            _db = db;
        }

        public async Task<BusinessResponse<bool>> Handle(DeletePortfolioCommand request, CancellationToken cancellationToken)
        {
            var entity = await _db.LoadByLocalSecondaryIndexAsync<PortfolioEntity>(
                request.RequestingUserId.GetAttributeValue(),
                request.PortfolioId.GetAttributeValue(),
                PortfolioLocalSecondaryIndexes.PortfolioIdIndex
            );

            if (entity == null)
                return BusinessResponse.Fail<bool>(
                    DeletePortfolioResponseCodes.PortfolioNotFound,
                    $"No portfolio with the id of {request.PortfolioId} was found");

            if (entity.Assets.Count > 0)
                return BusinessResponse.Fail<bool>(
                    DeletePortfolioResponseCodes.PortfolioContainsAssets,
                    $"Portfolio contains assets. Sell the portfolio assets to be able to delete the portfolio");

            await _db.DeleteItemAsync(
                typeof(PortfolioEntity).GetDynamoDBTableName(),
                entity.GetDynamoDBKey());

            return BusinessResponse.Ok<bool>(true, "Portfolio deleted!");
        }
    }
}