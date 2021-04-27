using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Business.Wrappers;
using Domain.Models;
using Domain.Repositories;

namespace Business.Commands
{
    public class UpdatePortfolioCommand : IRequestWrapper<Portfolio>
    {
        [Required]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Currency { get; set; }
        public long? Owner { get; set; }
    }

    public class UpdatePortfolioCommandHandler : IHandlerWrapper<UpdatePortfolioCommand, Portfolio>
    {
        private readonly IPortfolioRepository _portfolioRepository;        
        private readonly IMapper _mapper;

        public UpdatePortfolioCommandHandler(IPortfolioRepository portfolioRepository, IMapper mapper)
        {
            _portfolioRepository = portfolioRepository;
            _mapper = mapper;
        }
        public async Task<BusinessResponse<Portfolio>> Handle(UpdatePortfolioCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Portfolio>(request);
            
            await _portfolioRepository.UpdatePortfolioAsync(entity);

            return BusinessResponse.Ok<Portfolio>(entity, "Portfolio updated!");
        }
    }
}