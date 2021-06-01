using System.Collections.Generic;
using Business.Wrappers;
using Conditus.Trader.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Business.Queries
{
    public class GetPortfolioGrowthPointsQuery : BusinessRequest, IRequestWrapper<IEnumerable<PortfolioGrowthPoint>>
    {
        [Required]
        public string PortfolioId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public enum GetPortfolioGrowthPointsResponseCodes
    {
        Success,
        FromDateLaterThanToDate,
        TimespanToLong
    }
}