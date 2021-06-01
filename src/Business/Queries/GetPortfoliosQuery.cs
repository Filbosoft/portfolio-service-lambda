using System.Collections.Generic;
using Business.Wrappers;
using Conditus.Trader.Domain.Models;
using System;

namespace Business.Queries
{
    public class GetPortfoliosQuery : BusinessRequest, IRequestWrapper<IEnumerable<PortfolioOverview>>
    {
        public string NameQuery { get; set; }
        public DateTime? CreatedFromDate { get; set; }
    }

    public enum GetPortfoliosResponseCodes
    {
        Success
    }
}