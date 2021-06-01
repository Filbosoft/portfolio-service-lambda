using System;
using System.Collections.Generic;
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

            switch (response.ResponseCode)
            {
                case CreatePortfolioResponseCodes.Success:
                default:
                    var newPortfolio = response.Data;

                    return CreatedAtAction(
                        nameof(GetPortfolioById),
                        new { Id = newPortfolio.Id },
                        newPortfolio
                    );
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PortfolioOverview>>> GetPortfolios(
            [FromQuery] string nameQuery,
            [FromQuery] DateTime? createdFromDate)
        {
            var query = new GetPortfoliosQuery
            {
                NameQuery = nameQuery,
                CreatedFromDate = createdFromDate
            };
            var response = await _mediator.Send(query);

            switch (response.ResponseCode)
            {
                case GetPortfoliosResponseCodes.Success:
                default:
                    var portfolios = response.Data;
                    return Ok(portfolios);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PortfolioDetail>> GetPortfolioById([FromRoute] string id)
        {
            var query = new GetPortfolioByIdQuery { PortfolioId = id };
            var response = await _mediator.Send(query);

            switch (response.ResponseCode)
            {
                case GetPortfolioByIdResponseCodes.PortfolioNotFound:
                    return NotFound(response.Message);

                case GetPortfolioByIdResponseCodes.Success:
                default:
                    var portfolio = response.Data;

                    return Ok(portfolio);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PortfolioDetail>> Put([FromRoute] string id, [FromBody] UpdatePortfolioCommand command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);

            switch (response.ResponseCode)
            {
                case UpdatePortfolioResponseCodes.PortfolioNotFound:
                    return NotFound(response.Message);

                case UpdatePortfolioResponseCodes.Success:
                default:
                    var updatedPortfolio = response.Data;

                    return Accepted(updatedPortfolio);
            }
        }
    }
}
