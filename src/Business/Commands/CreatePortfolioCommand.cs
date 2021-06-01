using System;
using System.ComponentModel.DataAnnotations;
using Business.Wrappers;
using Conditus.Trader.Domain.Models;

namespace Business.Commands
{
    public class CreatePortfolioCommand : BusinessRequest, IRequestWrapper<PortfolioDetail>
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [Range(1, double.MaxValue)]
        public decimal? Capital { get; set; }
    }

    public enum CreatePortfolioResponseCodes
    {
        Success
    }
}