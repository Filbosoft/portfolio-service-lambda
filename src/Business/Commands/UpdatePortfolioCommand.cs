using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AutoMapper;
using Business.Commands.Common;
using Business.Extensions;
using Business.HelperMethods;
using Business.Wrappers;
using Conditus.DynamoDBMapper.Mappers;
using Conditus.Trader.Domain.Entities;
using Conditus.Trader.Domain.Models;
using Database.Indexes;
using MediatR;

namespace Business.Commands
{
    public class UpdatePortfolioCommand : UpdateCommand, IRequestWrapper<PortfolioDetail>
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
    }

    public class UpdatePortfolioCommandHandler : IHandlerWrapper<UpdatePortfolioCommand, PortfolioDetail>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IAmazonDynamoDB _db;

        public UpdatePortfolioCommandHandler(IMapper mapper, IAmazonDynamoDB db, IMediator mediator)
        {
            _mapper = mapper;
            _db = db;
            _mediator = mediator;
        }

        public async Task<BusinessResponse<PortfolioDetail>> Handle(UpdatePortfolioCommand request, CancellationToken cancellationToken)
        {
            var entity = await _db.LoadByLocalIndexAsync<PortfolioEntity>(
                request.RequestingUserId,
                nameof(PortfolioEntity.Id),
                request.Id,
                PortfolioLocalIndexes.PortfolioIdIndex
            );

            if (entity == null)
                return BusinessResponse.Fail<PortfolioDetail>($"No portfolio with the id of {request.Id} was found");

            var updateRequest = GetUpdateRequest(request, entity);
            
            var response = await _db.UpdateItemAsync(updateRequest);
            var updatedEntity = response.Attributes.ToEntity<PortfolioEntity>();
            var portfolioDetail = _mapper.Map<PortfolioDetail>(updatedEntity);

            return BusinessResponse.Ok<PortfolioDetail>(portfolioDetail, "Portfolio updated!");
        }

        /***
        * Expression attributes
        ***/

        private const string V_REQUESTING_USER_ID = ":v_requesting_user_id";
        private const string V_CREATED_AT = ":v_created_at";
        private const string V_NEW_NAME = ":v_new_name";
        
        

        private UpdateItemRequest GetUpdateRequest(UpdatePortfolioCommand request, PortfolioEntity entity)
        {
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
                    {V_NEW_NAME, request.Name.GetAttributeValue()}
                },
                UpdateExpression = $"SET {nameof(PortfolioEntity.PortfolioName)} = {V_NEW_NAME}",
                ReturnValues = "ALL_NEW"
            };

            return updateRequest;
        }
    }
}