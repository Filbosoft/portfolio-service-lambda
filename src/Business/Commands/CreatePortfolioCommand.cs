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
        private readonly IDynamoDBContext _dbContext;

        public CreatePortfolioCommandHandler(IMapper mapper, IDynamoDBContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<BusinessResponse<PortfolioDetail>> Handle(CreatePortfolioCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<PortfolioEntity>(request);
            entity.Id = Guid.NewGuid().ToString();
            entity.Assets = new List<PortfolioAsset>();

            var dbRequest = new PutItemRequest
            {
                TableName = "Portfolios",
                Item
            };
            await _dbContext.SaveAsync(entity);

            var portfolioDetail = _mapper.Map<PortfolioDetail>(entity);

            return BusinessResponse.Ok<PortfolioDetail>(portfolioDetail, "Portfolio created!");
        }
    }
}