using AutoMapper;
using Business.Commands;
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