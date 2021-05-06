using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Repositories;
using Business.Commands.OrderCommands;
using Business.Queries;
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

        //PlaceOrder
        //GetPortfolioOrder
        //GetPortfolioOrders
        //UpdateOrder
        //CancelOrder

        [HttpPost]
        public async Task<ActionResult<Order>> PlaceOrder([FromRoute] string portfolioId, [FromBody] PlaceOrderCommand command)
        {
            command.PortfolioId = portfolioId;
            var response = await _mediator.Send(command);

            if (response.IsError)
                return BadRequest(response.Message);

            var newOrder = response.Data;
            return CreatedAtAction(
                nameof(GetById),
                new { portfolioId = portfolioId,  Id = newOrder.Id },
                newOrder
            );
        }

        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<Order>>> GetPortfolioOrders([FromRoute] string portfolioId, 
        //     [FromQuery] OrderType? type,
        //     [FromQuery] string assetId,
        //     [FromQuery] AssetType? assetType,
        //     [FromQuery] OrderStatus? status
        //     )
        // {
        //     var query = new GetPortfolioOrdersQuery 
        //     {
        //         PortfolioId = portfolioId,
        //         OrderType = type,
        //         AssetId = assetId,
        //         AssetType = assetType,
        //         Status = status
        //     };
        //     var response = await _mediator.Send(query);

        //     if (response.IsError)
        //         return BadRequest(response.Message);

        //     var orders = response.Data;
        //     return Ok(orders);
        // }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetById([FromRoute] string portfolioId, [FromRoute] string orderId)
        {
            // var query = new GetPortfolioOrderByIdQuery { PortfolioId = portfolioId, OrderId = orderId };
            // var response = await _mediator.Send(query);

            // if (response.IsError) 
            //     return NotFound(response.Message);

            // var order = response.Data;

            // return Ok(order);
            return Ok();
        }


        // [HttpPut("{id}")]
        // public async Task<ActionResult<Portfolio>> Put([FromRoute] string id, [FromBody] UpdatePortfolioCommand command)
        // {
        //     command.Id = id;
        //     var response = await _mediator.Send(command);

        //     if (response.IsError) 
        //         return NotFound(response.Message);

        //     var updatedPortfolio = response.Data;

        //     return Accepted(updatedPortfolio);
        // }

        // [HttpDelete("{id}")]
        // public async Task<ActionResult<bool>> Delete([FromRoute] string id)
        // {
        //     var command = new DeletePortfolioCommand { PortfolioId = id };
        //     var response = await _mediator.Send(command);

        //     return Ok();
        // }
    }
}
