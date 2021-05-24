using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Business.Wrappers;
using System.Collections.Generic;
using Conditus.Trader.Domain.Models;
using Conditus.Trader.Domain.Entities;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Conditus.DynamoDBMapper.Mappers;
using Amazon.DynamoDBv2;

namespace Business.Commands
{
    public class CreatePortfolioCommand : BusinessRequest, IRequestWrapper<PortfolioDetail>
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Capital { get; set; }
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

        public async Task<BusinessResponse<PortfolioDetail>> Handle(CreatePortfolioCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<PortfolioEntity>(request);
            entity.Id = Guid.NewGuid().ToString();
            entity.Assets = new List<PortfolioAsset>();

            var response = await _db.PutItemAsync("Portfolios", entity.GetAttributeValueMap());
            
            var portfolioDetail = _mapper.Map<PortfolioDetail>(entity);

            return BusinessResponse.Ok<PortfolioDetail>(portfolioDetail, "Portfolio created!");
        }
    }
}