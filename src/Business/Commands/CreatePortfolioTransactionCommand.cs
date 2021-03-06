using System;
using System.ComponentModel.DataAnnotations;
using Business.Wrappers;

namespace Business.Commands
{
    public class CreatePortfolioTransactionCommand : BusinessRequest, IRequestWrapper<bool>
    {
        public string PortfolioId { get; set; }
        [Required]
        public decimal? Amount { get; set; }
    }

    public enum CreatePortfolioTransactionResponseCodes
    {
        Success,
        PortfolioNotFound,
        InsufficientCapital
    }
}