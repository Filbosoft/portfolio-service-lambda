using AutoMapper;
using Business.Commands;
using Conditus.Trader.Domain.Entities;

namespace Business.MappingProfiles
{
    public class PortfolioMappingProfile : Profile
    {
        public PortfolioMappingProfile()
        {
            CreateMap<CreatePortfolioCommand, PortfolioEntity>();
            // CreateMap<UpdatePortfolioCommand, PortfolioEntity>();
        }
    }
}