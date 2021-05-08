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
    public class CancelOrderCommand : UpdateCommand, IRequestWrapper<Order>
    {
        [Required]
        public string PortfolioId { get; set; }
        [Required]
        public string OrderId { get; set; }
    }

    public class CancelOrderCommandHandler : IHandlerWrapper<CancelOrderCommand, Order>
    {
        private readonly IMediator _mediator;

        public CancelOrderCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<BusinessResponse<Order>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var updateOrderCommand = new UpdateOrderCommand
            {
                PortfolioId = request.PortfolioId,
                Id = request.OrderId,
                Status = OrderStatus.Cancelled
            };
            var response = await _mediator.Send(updateOrderCommand);

            return response;
        }
    }
}