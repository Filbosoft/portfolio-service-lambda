using System;
using System.Collections.Generic;
using Conditus.Trader.Domain.Entities;
using Conditus.Trader.Domain.Models;

using static Integration.Seeds.AssetSeeds;
using static Integration.Utilities.TestConstants;

namespace Integration.Seeds
{
    public static class PortfolioSeeds
    {
        public static readonly PortfolioEntity PORTFOLIO_WITH_ASSETS = new PortfolioEntity
        {
            Id = "161d4ed7-c66a-4907-97dd-6f088b8723a5",
            Capital = 100000.90M,
            Name = "Portfolio",
            OwnerId = TESTUSER_ID,
            CreatedAt = Convert.ToDateTime("2/5/2021 08:00:01 AM").ToUniversalTime(),
            Assets = new List<PortfolioAsset>
            {
                new PortfolioAsset{Name = DKK_STOCK.Name, Symbol = DKK_STOCK.Symbol, Quantity = 10},
                new PortfolioAsset{Name = USD_STOCK.Name, Symbol = USD_STOCK.Symbol, Quantity = 10}
            }
        };
    }
}