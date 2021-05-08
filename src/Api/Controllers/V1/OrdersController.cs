using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Repositories;
using Business.Commands.OrderCommands;
using Business.Queries;
using Business.Queries.OrderQueries;
using Domain.Enums;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/portfolios/{portfolioId}/[controller]")]
    [Produces("application/json")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> PlaceOrder([FromRoute] string portfolioId, [FromBody] PlaceOrderCommand command)
        {
            command.PortfolioId = portfolioId;
            var response = await _mediator.Send(command);

            if (response.IsError)
                return BadRequest(response.Message);

            var newOrder = response.Data;
            return CreatedAtAction(
                nameof(GetPortfolioOrderById),
                new { portfolioId = portfolioId, Id = newOrder.Id },
                newOrder
            );
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetPortfolioOrders([FromRoute] string portfolioId,
            [FromQuery] OrderType? type,
            [FromQuery] string assetId,
            [FromQuery] AssetType? assetType,
            [FromQuery] OrderStatus? status,
            [FromQuery] DateTime? createdFromDate,
            [FromQuery] DateTime? createdToDate,
            [FromQuery] DateTime? completedFromDate,
            [FromQuery] DateTime? completedToDate
            )
        {
            var query = new GetPortfolioOrdersQuery
            {
                PortfolioId = portfolioId,
                Type = type,
                AssetId = assetId,
                AssetType = assetType,
                Status = status,
                CreatedFromDate = createdFromDate,
                CreatedToDate = createdToDate,
                CompletedFromDate = completedFromDate,
                CompletedToDate = completedToDate
            };
            var response = await _mediator.Send(query);

            if (response.IsError)
                return NotFound(response.Message);

            var orders = response.Data;
            return Ok(orders);
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<Order>> GetPortfolioOrderById([FromRoute] string portfolioId, [FromRoute] string orderId)
        {
            var query = new GetPortfolioOrderByIdQuery { PortfolioId = portfolioId, OrderId = orderId };
            var response = await _mediator.Send(query);

            if (response.IsError)
                return NotFound(response.Message);

            var order = response.Data;

            return Ok(order);
        }


        [HttpPut("{orderId}")]
        public async Task<ActionResult<Order>> UpdatePortfolioOrder(
            [FromRoute] string portfolioId,
            [FromRoute] string orderId,
            [FromBody] UpdateOrderCommand command)
        {
            command.PortfolioId = portfolioId;
            command.Id = orderId;
            var response = await _mediator.Send(command);

            if (response.IsError)
                return NotFound(response.Message);

            var updatedOrder = response.Data;

            return Accepted(updatedOrder);
        }

        [HttpDelete("{orderId}")]
        public async Task<ActionResult<bool>> CancelPortfolioOrder(
            [FromRoute] string portfolioId,
            [FromRoute] string orderId
        )
        {
            var command = new CancelOrderCommand
            {
                PortfolioId = portfolioId,
                OrderId = orderId
            };
            var response = await _mediator.Send(command);

            return Ok();
        }
    }
}
