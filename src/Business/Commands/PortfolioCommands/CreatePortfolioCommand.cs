using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Business.Wrappers;
using Domain.Repositories;
using Domain.Models;
using System.Collections.Generic;

namespace Business.Commands.PortfolioCommands
{
    public class CreatePortfolioCommand : BusinessRequest, IRequestWrapper<Portfolio>
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Currency { get; set; }
        [Required]
        public string Owner { get; set; }
    }

    public class CreatePortfolioCommandHandler : IHandlerWrapper<CreatePortfolioCommand, Portfolio>
    {
        private readonly IMapper _mapper;
        private readonly IPortfolioRepository _portfolioRepository;
        public CreatePortfolioCommandHandler(IMapper mapper, IPortfolioRepository portfolioRepository)
        {
            _mapper = mapper;
            _portfolioRepository = portfolioRepository;
        }

        public async Task<BusinessResponse<Portfolio>> Handle(CreatePortfolioCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Portfolio>(request);
            entity.Orders = new List<Order>();

            await _portfolioRepository.CreatePortfolioAsync(entity);

            return BusinessResponse.Ok<Portfolio>(entity, "Portfolio created!");
        }
    }
}