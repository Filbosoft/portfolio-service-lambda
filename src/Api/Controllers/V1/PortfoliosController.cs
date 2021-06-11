using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Responses.V1;
using Business.Commands;
using Business.Queries;
using Conditus.Trader.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("v{v:apiVersion}/[controller]")]
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
                default:
                    var newPortfolio = response.Data;
                    var apiResponse = new ApiResponse<PortfolioDetail>
                    {
                        Data = newPortfolio,
                        Status = StatusCodes.Status201Created
                    };
                    return CreatedAtAction(
                        nameof(GetPortfolioById),
                        new { Id = newPortfolio.Id },
                        apiResponse
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
                default:
                    var portfolios = response.Data;
                    var apiResponse = new ApiResponse<IEnumerable<PortfolioOverview>>
                    {
                        Data = portfolios,
                        Status = StatusCodes.Status200OK
                    };
                    return Ok(apiResponse);
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
                    var notFoundProblem = new ProblemDetails
                    {
                        Title = response.ResponseCode.ToString(),
                        Detail = response.Message,
                        Status = StatusCodes.Status404NotFound
                    };
                    return NotFound(notFoundProblem);

                default:
                    var portfolio = response.Data;
                    var apiResponse = new ApiResponse<PortfolioDetail>
                    {
                        Data = portfolio,
                        Status = StatusCodes.Status200OK
                    };

                    return Ok(apiResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PortfolioDetail>> UpdatePortfolio([FromRoute] string id, [FromBody] UpdatePortfolioCommand command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);

            switch (response.ResponseCode)
            {
                case UpdatePortfolioResponseCodes.PortfolioNotFound:
                    var notFoundProblem = new ProblemDetails
                    {
                        Title = response.ResponseCode.ToString(),
                        Detail = response.Message,
                        Status = StatusCodes.Status404NotFound
                    };
                    return NotFound(notFoundProblem);

                default:
                    var updatedPortfolio = response.Data;
                    var apiResponse = new ApiResponse<PortfolioDetail>
                    {
                        Data = updatedPortfolio,
                        Status = StatusCodes.Status202Accepted
                    };

                    return Accepted(apiResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePortfolio([FromRoute] string id)
        {
            var command = new DeletePortfolioCommand { PortfolioId = id };
            var response = await _mediator.Send(command);

            switch (response.ResponseCode)
            {
                case DeletePortfolioResponseCodes.PortfolioContainsAssets:
                    var portfolioContainsAssetsProblem = new ProblemDetails
                    {
                        Title = response.ResponseCode.ToString(),
                        Detail = response.Message,
                        Status = StatusCodes.Status400BadRequest
                    };
                    return BadRequest(portfolioContainsAssetsProblem);

                case DeletePortfolioResponseCodes.PortfolioNotFound:
                default:
                    return Ok();
            }
        }
    }
}
