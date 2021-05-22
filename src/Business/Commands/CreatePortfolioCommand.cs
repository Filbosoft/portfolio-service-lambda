using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Business.Wrappers;
using System.Collections.Generic;
using Conditus.Trader.Domain.Models;

namespace Business.Commands
{
    public class CreatePortfolioCommand : BusinessRequest, IRequestWrapper<PortfolioDetail>
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Currency { get; set; }
        [Required]
        public string Owner { get; set; }
    }

    public class CreatePortfolioCommandHandler : IHandlerWrapper<CreatePortfolioCommand, PortfolioDetail>
    {
        private readonly IMapper _mapper;
        private readonly IPortfolioRepository _portfolioRepository;
        public CreatePortfolioCommandHandler(IMapper mapper, IPortfolioRepository portfolioRepository)
        {
            _mapper = mapper;
            _portfolioRepository = portfolioRepository;
        }

        public async Task<BusinessResponse<PortfolioDetail>> Handle(CreatePortfolioCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Portfolio>(request);
            entity.Orders = new List<Order>();

            await _portfolioRepository.CreatePortfolioAsync(entity);

            return BusinessResponse.Ok<PortfolioDetail>(entity, "Portfolio created!");
        }
    }
}