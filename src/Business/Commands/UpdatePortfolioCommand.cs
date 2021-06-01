using System.ComponentModel.DataAnnotations;
using Business.Commands.Common;
using Business.Wrappers;
using Conditus.Trader.Domain.Models;

namespace Business.Commands
{
    public class UpdatePortfolioCommand : UpdateCommand, IRequestWrapper<PortfolioDetail>
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
    }

    public enum UpdatePortfolioResponseCodes
    {
        Success,
        PortfolioNotFound
    }
}