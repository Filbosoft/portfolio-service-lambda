using AutoMapper;
using Business.Commands.OrderCommands;
using Domain.Models;

namespace Business.MappingProfiles
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<PlaceOrderCommand, Order>();
            // CreateMap<UpdatePortfolioCommand, Portfolio>();
        }
    }
}