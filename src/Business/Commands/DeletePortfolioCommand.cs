using System.ComponentModel.DataAnnotations;
using Business.Wrappers;

namespace Business.Commands
{
    public class DeletePortfolioCommand : BusinessRequest, IRequestWrapper<bool>
    {
        [Required]
        public string PortfolioId { get; set; }
    }

    public enum DeletePortfolioResponseCodes
    {
        Success,
        PortfolioNotFound,
        PortfolioContainsAssets
    }
}