using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Business.Commands.Common;
using Business.Wrappers;
using Domain.Models;
using Domain.Repositories;

namespace Business.Commands.PortfolioCommands
{
    public class UpdatePortfolioCommand : UpdateCommand, IRequestWrapper<Portfolio>
    {
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
            var entity = await _portfolioRepository.GetPortfolioAsync(request.Id);
            if (entity == null)
                return BusinessResponse.Fail<Portfolio>($"No portfolio with the id of {request.Id} was found");

            entity = request.TransferUpdatedValues<Portfolio>(entity);
            
            await _portfolioRepository.UpdatePortfolioAsync(entity);

            return BusinessResponse.Ok<Portfolio>(entity, "Portfolio updated!");
        }
    }
}