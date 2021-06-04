using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Business.Wrappers;
using System.Collections.Generic;
using Conditus.Trader.Domain.Models;
using Conditus.Trader.Domain.Entities;
using Amazon.DynamoDBv2;
using Conditus.DynamoDB.MappingExtensions.Mappers;
using Conditus.DynamoDB.QueryExtensions.Extensions;

namespace Business.Commands.Handlers
{
    public class CreatePortfolioCommandHandler : IHandlerWrapper<CreatePortfolioCommand, PortfolioDetail>
    {
        private readonly IMapper _mapper;
        private readonly IAmazonDynamoDB _db;

        public CreatePortfolioCommandHandler(IMapper mapper, IAmazonDynamoDB db)
        {
            _mapper = mapper;
            _db = db;
        }

        public const string DEFAULT_CURRENCY_CODE = "DKK";        

        public async Task<BusinessResponse<PortfolioDetail>> Handle(CreatePortfolioCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<PortfolioEntity>(request);
            entity.Id = Guid.NewGuid().ToString();
            entity.Assets = new List<PortfolioAsset>();
            entity.CurrencyCode = DEFAULT_CURRENCY_CODE;

            var response = await _db.PutItemAsync(typeof(PortfolioEntity).GetDynamoDBTableName(), entity.GetAttributeValueMap());
            
            var portfolioDetail = _mapper.Map<PortfolioDetail>(entity);

            return BusinessResponse.Ok<PortfolioDetail>(portfolioDetail, "Portfolio created!");
        }
    }
}