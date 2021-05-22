using AutoMapper;
using Business.Commands;
using Conditus.Trader.Domain.Entities;
using Conditus.Trader.Domain.Models;

namespace Business.MappingProfiles
{
    public class PortfolioMappingProfile : Profile
    {
        public PortfolioMappingProfile()
        {
            CreateMap<CreatePortfolioCommand, PortfolioEntity>()
                .ForMember(
                    dest => dest.OwnerId,
                    opt => opt.MapFrom(src => src.RequestingUserId)
                )
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.MapFrom(src => src.RequestedAt)
                );
            // CreateMap<UpdatePortfolioCommand, PortfolioEntity>();
            
            CreateMap<PortfolioEntity, PortfolioDetail>();
            CreateMap<PortfolioEntity, PortfolioOverview>();
        }
    }
}