using System.Threading.Tasks;
using Business.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/portfolios/{portfolioId}/transactions")]
    [Produces("application/json")]
    public class PortfolioTransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PortfolioTransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePortfolioTransaction([FromRoute] string portfolioId, [FromBody] CreatePortfolioTransactionCommand command)
        {
            command.PortfolioId = portfolioId;
            var response = await _mediator.Send(command);

            if (response.IsError)
                return NotFound(response.Message);
            
            return StatusCode(StatusCodes.Status201Created);
        }

        
    }
}
