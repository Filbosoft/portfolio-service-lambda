using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Business.Commands.Common;
using Business.Queries.OrderQueries;
using Business.Wrappers;
using Domain.Enums;
using Domain.Models;
using Domain.Repositories;
using MediatR;

namespace Business.Commands.OrderCommands
{
    public class UpdateOrderCommand : UpdateCommand, IRequestWrapper<Order>
    {
        public string PortfolioId { get; set; }
        [Required]
        public string Id { get; set; }
        public int? Quantity { get; set; }
        public OrderStatus? Status { get; set; }
        public decimal? Price { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    public class UpdateOrderCommandHandler : IHandlerWrapper<UpdateOrderCommand, Order>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMediator _mediator;

        public UpdateOrderCommandHandler(IOrderRepository orderRepository, IMediator mediator)
        {
            _orderRepository = orderRepository;
            _mediator = mediator;
        }
        public async Task<BusinessResponse<Order>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var getOrderQuery = new GetPortfolioOrderByIdQuery {PortfolioId = request.PortfolioId, OrderId = request.Id};
            var queryResponse = await _mediator.Send(getOrderQuery);

            if (queryResponse.IsError)
                return queryResponse;

            var entity = queryResponse.Data;

            entity = request.TransferUpdatedValues<Order>(entity);
            await _orderRepository.UpdateOrderAsync(request.PortfolioId, entity);

            return BusinessResponse.Ok<Order>(entity, "Order updated!");
        }
    }
}