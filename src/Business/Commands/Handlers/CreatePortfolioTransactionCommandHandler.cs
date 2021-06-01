using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Business.Wrappers;
using System.Collections.Generic;
using Conditus.Trader.Domain.Entities;
using Conditus.DynamoDBMapper.Mappers;
using Amazon.DynamoDBv2;
using Business.HelperMethods;
using Database.Indexes;
using Amazon.DynamoDBv2.Model;
using Business.Extensions;

namespace Business.Commands.Handlers
{

    public class CreatePortfolioTransactionCommandHandler : IHandlerWrapper<CreatePortfolioTransactionCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IAmazonDynamoDB _db;

        public CreatePortfolioTransactionCommandHandler(IMapper mapper, IAmazonDynamoDB db)
        {
            _mapper = mapper;
            _db = db;
        }

        public async Task<BusinessResponse<bool>> Handle(CreatePortfolioTransactionCommand request, CancellationToken cancellationToken)
        {
            var entity = await _db.LoadByLocalIndexAsync<PortfolioEntity>(
                request.RequestingUserId,
                nameof(PortfolioEntity.Id),
                request.PortfolioId,
                PortfolioLocalIndexes.PortfolioIdIndex
            );

            if (entity == null)
                return BusinessResponse.Fail<bool>(
                    CreatePortfolioTransactionResponseCodes.PortfolioNotFound,
                    $"No portfolio with the id of {request.PortfolioId} was found");

            var updateRequest = GetUpdateRequest(request, entity);
            await _db.UpdateItemAsync(updateRequest);

            return BusinessResponse.Ok<bool>(true, "Portfolio transaction created!");
        }

        /***
        * Expression attributes
        ***/

        private const string V_REQUESTING_USER_ID = ":v_requesting_user_id";
        private const string V_CREATED_AT = ":v_created_at";
        private const string V_NEW_CAPITAL = ":v_new_capital";        

        private UpdateItemRequest GetUpdateRequest(CreatePortfolioTransactionCommand request, PortfolioEntity entity)
        {
            var newCapital = entity.Capital + request.Amount;
            var updateRequest = new UpdateItemRequest
            {
                TableName = DynamoDBHelper.GetDynamoDBTableName<PortfolioEntity>(),
                Key = new Dictionary<string, AttributeValue>
                {
                    {nameof(PortfolioEntity.OwnerId), request.RequestingUserId.GetAttributeValue()},
                    {nameof(PortfolioEntity.CreatedAt), entity.CreatedAt.GetAttributeValue()}
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {V_NEW_CAPITAL, newCapital.GetAttributeValue()}
                },
                UpdateExpression = $"SET {nameof(PortfolioEntity.Capital)} = {V_NEW_CAPITAL}",
                ReturnValues = "NONE"
            };

            return updateRequest;
        }
    }
}