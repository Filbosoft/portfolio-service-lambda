using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Business.Wrappers;
using Domain.Repositories;
using Domain.Models;
using Domain.Enums;
using System.Collections.Generic;

namespace Business.Commands.OrderCommands
{
    public class PlaceOrderCommand : BusinessRequest, IRequestWrapper<Order>
    {
        public string PortfolioId { get; set; }
        [Required]
        public OrderType? Type { get; set; }
        [Required]
        public string AssetId { get; set; }
        [Required]
        public AssetType? AssetType { get; set; }
        [Required]
        [Range(1,int.MaxValue)]
        public int Quantity { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string Currency { get; set; }
        [Required]
        public DateTime? ExpiresAt { get; set; }
    }

    public class PlaceOrderCommandHandler : IHandlerWrapper<PlaceOrderCommand, Order>
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        public PlaceOrderCommandHandler(IMapper mapper, IOrderRepository orderRepository)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
        }

        public async Task<BusinessResponse<Order>> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Order>(request);
            Order placedOrder;

            try
            {
                placedOrder = await _orderRepository.PlaceOrderAsync(request.PortfolioId, entity);
            } 
            catch (KeyNotFoundException)
            {
                return BusinessResponse.Fail<Order>($"No portfolio with the id of {request.PortfolioId} found");
            }
            
            return BusinessResponse.Ok<Order>(placedOrder, "Order created!");
        }
    }
}