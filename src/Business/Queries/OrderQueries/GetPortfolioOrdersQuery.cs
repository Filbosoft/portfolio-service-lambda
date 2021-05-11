using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Business.Wrappers;
using Domain.Repositories;
using Domain.Models;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;
using System.Linq;
using System;

namespace Business.Queries.OrderQueries
{
    public class GetPortfolioOrdersQuery : BusinessRequest, IRequestWrapper<IEnumerable<Order>>
    {
        [Required]
        public string PortfolioId { get; set; }
        public OrderType? Type { get; set; }
        public OrderStatus? Status { get; set; }
        public string AssetId { get; set; }
        public AssetType? AssetType { get; set; }
        public DateTime? CreatedFromDate { get; set; }
        public DateTime? CreatedToDate { get; set; }
        public DateTime? CompletedFromDate { get; set; }
        public DateTime? CompletedToDate { get; set; }
    }

    public class GetPortfolioOrdersQueryHandler : IHandlerWrapper<GetPortfolioOrdersQuery, IEnumerable<Order>>
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IMapper _mapper;

        public GetPortfolioOrdersQueryHandler(IPortfolioRepository portfolioRepository, IMapper mapper)
        {
            _portfolioRepository = portfolioRepository;
            _mapper = mapper;
        }

        public async Task<BusinessResponse<IEnumerable<Order>>> Handle(GetPortfolioOrdersQuery request, CancellationToken cancellationToken)
        {
            var portfolio = await _portfolioRepository.GetPortfolioAsync(request.PortfolioId, request.RequestingUserId);
            if (portfolio == null)
                return BusinessResponse.Fail<IEnumerable<Order>>($"No portfolio with the id of {request.PortfolioId} was found");

            var portfolioOrders = portfolio.Orders;
            var query = portfolioOrders.AsQueryable();

            if (request.Type != null)
                query = query.Where(o => o.Type.Equals(request.Type));
            if (request.Status != null)
                query = query.Where(o => o.Status.Equals(request.Status));
            if (request.AssetId != null)
                query = query.Where(o => o.AssetId.Equals(request.AssetId));
            if (request.AssetType != null)
                query = query.Where(o => o.AssetType.Equals(request.AssetType));

            if (request.CreatedFromDate != null)
                query = query.Where(o => o.CreatedAt >= request.CreatedFromDate);
            if (request.CreatedToDate != null)
                query = query.Where(o => o.CreatedAt <= request.CreatedToDate);

            if (request.CompletedFromDate != null)
                query = query.Where(o => o.Status.Equals(OrderStatus.Completed) && o.CompletedAt >= request.CompletedFromDate);
            if (request.CompletedToDate != null)
                query = query.Where(o => o.Status.Equals(OrderStatus.Completed) && o.CompletedAt <= request.CompletedToDate);

            var filteredOrders = query.ToList();

            return BusinessResponse.Ok<IEnumerable<Order>>(filteredOrders);
        }
    }
}