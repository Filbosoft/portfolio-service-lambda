using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Api.Repositories;
using Business.Commands;
using Business.Queries;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class PortfoliosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PortfoliosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Portfolio>> CreatePortfolio([FromBody] CreatePortfolioCommand command)
        {
            var response = await _mediator.Send(command);

            if (response.IsError)
                return BadRequest(response.Message);

            var newPortfolio = response.Data;
            return CreatedAtAction(
                nameof(GetById),
                new { Id = newPortfolio.Id },
                newPortfolio
            );
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Portfolio>>> Get([FromQuery] long? ownerId)
        {
            var query = new GetPortfoliosQuery 
            {
                OwnerId = ownerId
            };
            var response = await _mediator.Send(query);

            if (response.IsError)
                return BadRequest(response.Message);

            var portfolios = response.Data;
            return Ok(portfolios);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Portfolio>> GetById([FromRoute] string id)
        {
            var query = new GetPortfolioByIdQuery { PortfolioId = id };
            var response = await _mediator.Send(query);

            if (response.IsError) 
                return NotFound(response.Message);

            var portfolio = response.Data;

            return Ok(portfolio);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<Portfolio>> Put([FromRoute] string id, [FromBody] UpdatePortfolioCommand command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);

            if (response.IsError) 
                return NotFound(response.Message);

            var updatedPortfolio = response.Data;

            return Accepted(updatedPortfolio);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete([FromRoute] string id)
        {
            var command = new DeletePortfolioCommand { PortfolioId = id };
            var response = await _mediator.Send(command);

            return Ok();
        }
    }
}
