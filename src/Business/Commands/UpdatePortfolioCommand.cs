using System.Threading;
using System.Threading.Tasks;
using Business.Commands.Common;
using Business.Wrappers;
using Conditus.Trader.Domain.Models;

namespace Business.Commands
{
    // public class UpdatePortfolioCommand : UpdateCommand, IRequestWrapper<PortfolioDetail>
    // {
    //     public string Id { get; set; }
    //     public string Name { get; set; }
    //     public string Currency { get; set; }
    //     public string Owner { get; set; }
    // }

    // public class UpdatePortfolioCommandHandler : IHandlerWrapper<UpdatePortfolioCommand, PortfolioDetail>
    // {
    //     private readonly IPortfolioRepository _portfolioRepository;        

    //     public UpdatePortfolioCommandHandler(IPortfolioRepository portfolioRepository)
    //     {
    //         _portfolioRepository = portfolioRepository;
    //     }
    //     public async Task<BusinessResponse<PortfolioDetail>> Handle(UpdatePortfolioCommand request, CancellationToken cancellationToken)
    //     {
    //         var entity = await _portfolioRepository.GetPortfolioAsync(request.Id, request.RequestingUserId);
    //         if (entity == null)
    //             return BusinessResponse.Fail<PortfolioDetail>($"No portfolio with the id of {request.Id} was found");

    //         entity = request.TransferUpdatedValues<PortfolioDetail>(entity);
            
    //         await _portfolioRepository.UpdatePortfolioAsync(entity);

    //         return BusinessResponse.Ok<PortfolioDetail>(entity, "Portfolio updated!");
    //     }
    // }
}