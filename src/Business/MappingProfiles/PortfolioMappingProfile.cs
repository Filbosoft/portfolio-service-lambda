using AutoMapper;
using Business.Commands.PortfolioCommands;
using Domain.Models;

namespace Business.MappingProfiles
{
    public class PortfolioMappingProfile : Profile
    {
        public PortfolioMappingProfile()
        {
            CreateMap<CreatePortfolioCommand, Portfolio>();
            CreateMap<UpdatePortfolioCommand, Portfolio>();
        }
    }
}