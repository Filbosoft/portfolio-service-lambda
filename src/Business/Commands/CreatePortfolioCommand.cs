using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Business.Wrappers;
using System.Collections.Generic;
using Conditus.Trader.Domain.Models;
using Conditus.Trader.Domain.Entities;
using Conditus.DynamoDBMapper.Mappers;
using Amazon.DynamoDBv2;
using Business.HelperMethods;

namespace Business.Commands
{
    public class CreatePortfolioCommand : BusinessRequest, IRequestWrapper<PortfolioDetail>
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [Range(1, double.MaxValue)]
        public decimal? Capital { get; set; }
    }

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

            var response = await _db.PutItemAsync(DynamoDBHelper.GetDynamoDBTableName<PortfolioEntity>(), entity.GetAttributeValueMap());
            
            var portfolioDetail = _mapper.Map<PortfolioDetail>(entity);

            return BusinessResponse.Ok<PortfolioDetail>(portfolioDetail, "Portfolio created!");
        }
    }
}