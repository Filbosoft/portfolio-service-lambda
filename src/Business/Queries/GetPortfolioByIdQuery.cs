using Business.Wrappers;
using Conditus.Trader.Domain.Models;

namespace Business.Queries
{
    public class GetPortfolioByIdQuery : BusinessRequest, IRequestWrapper<PortfolioDetail>
    { 
        public string PortfolioId { get; set; }
    }

    public enum GetPortfolioByIdResponseCodes
    {
        Success,
        PortfolioNotFound
    }
}