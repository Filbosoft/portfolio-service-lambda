using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Queries;
using Conditus.Trader.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/portfolios/{portfolioId}/growthpoints")]
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

            if (response.IsError)
                return BadRequest(response.Message);

            var portfolios = response.Data;
            return Ok(portfolios);
        }
    }
}
