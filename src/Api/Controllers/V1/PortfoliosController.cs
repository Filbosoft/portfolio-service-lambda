using System;
using System.Threading.Tasks;
using Business.Commands;
using Business.Queries;
using Conditus.Trader.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class PortfoliosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PortfoliosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<PortfolioDetail>> CreatePortfolio([FromBody] CreatePortfolioCommand command)
        {
            var response = await _mediator.Send(command);

            if (response.IsError)
                return BadRequest(response.Message);

            var newPortfolio = response.Data;
            return CreatedAtAction(
                nameof(GetPortfolioById),
                new { Id = newPortfolio.Id },
                newPortfolio
            );
        }

        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<PortfolioOverview>>> Get([FromQuery] string ownerId)
        // {
        //     var query = new GetPortfoliosQuery 
        //     {
        //         OwnerId = ownerId
        //     };
        //     var response = await _mediator.Send(query);

        //     if (response.IsError)
        //         return BadRequest(response.Message);

        //     var portfolios = response.Data;
        //     return Ok(portfolios);
        // }

        [HttpGet("{id}")]
        public async Task<ActionResult<PortfolioDetail>> GetPortfolioById([FromRoute] string id)
        {
            var query = new GetPortfolioByIdQuery { PortfolioId = id };
            var response = await _mediator.Send(query);

            if (response.IsError) 
                return NotFound(response.Message);

            var portfolio = response.Data;

            return Ok(portfolio);
        }


        // [HttpPut("{id}")]
        // public async Task<ActionResult<PortfolioDetail>> Put([FromRoute] string id, [FromBody] UpdatePortfolioCommand command)
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
