using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Responses.V1;
using Business.Queries;
using Conditus.Trader.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("v{v:apiVersion}/portfolios/{portfolioId}/growthpoints")]
    [Produces("application/json")]
    public class PortfolioGrowthPointsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PortfolioGrowthPointsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PortfolioGrowthPoint>>> GetPortfolioGrowthPoints(
            [FromRoute] string portfolioId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var query = new GetPortfolioGrowthPointsQuery
            {
                PortfolioId = portfolioId,
                FromDate = fromDate,
                ToDate = toDate
            };
            var response = await _mediator.Send(query);

            switch (response.ResponseCode)
            {
                case GetPortfolioGrowthPointsResponseCodes.FromDateLaterThanToDate:
                case GetPortfolioGrowthPointsResponseCodes.TimespanToLong:
                    var badRequestProblem = new ProblemDetails
                    {
                        Title = response.ResponseCode.ToString(),
                        Detail = response.Message,
                        Status = StatusCodes.Status400BadRequest
                    };
                    return BadRequest(badRequestProblem);

                case GetPortfolioGrowthPointsResponseCodes.Success:
                default:
                    var growthPoints = response.Data;
                    var apiResponse = new ApiResponse<IEnumerable<PortfolioGrowthPoint>>
                    {
                        Data = growthPoints,
                        Status = StatusCodes.Status200OK
                    };
                    return Ok(apiResponse);
            }
        }
    }
}
