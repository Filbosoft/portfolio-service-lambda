using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Business.Wrappers;
using Domain.Repositories;

namespace Business.Commands
{
    public class DeletePortfolioCommand : IRequestWrapper<bool>
    {
        [Required]
        public string PortfolioId { get; set; }
    }

    public class DeletePortfolioCommandHandler : IHandlerWrapper<DeletePortfolioCommand, bool>
    {
        private readonly IPortfolioRepository _portfolioRepository;

        public DeletePortfolioCommandHandler(IPortfolioRepository portfolioRepository)
        {
            _portfolioRepository = portfolioRepository;
        }
        public async Task<BusinessResponse<bool>> Handle(DeletePortfolioCommand request, CancellationToken cancellationToken)
        {
            await _portfolioRepository.DeletePortfolioAsync(request.PortfolioId);

            return BusinessResponse.Ok<bool>(true, "Portfolio deleted!");
        }
    }
}