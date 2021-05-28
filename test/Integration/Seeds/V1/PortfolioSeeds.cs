using System;
using System.Collections.Generic;
using Conditus.Trader.Domain.Entities;
using Conditus.Trader.Domain.Models;

using static Integration.Tests.V1.TestConstants;
using static Integration.Seeds.V1.AssetSeeds;

namespace Integration.Seeds.V1
{
    public static class PortfolioSeeds
    {
        public static readonly PortfolioEntity PORTFOLIO_WITH_ASSETS = new PortfolioEntity
        {
            Id = "161d4ed7-c66a-4907-97dd-6f088b8723a5",
            Capital = 100000.9M,
            PortfolioName = "ca60480e-2297-4145-90bc-d214f45a362f",
            OwnerId = TESTUSER_ID,
            CreatedAt = Convert.ToDateTime("5/25/2021 08:00:01 AM").ToUniversalTime(),
            Assets = new List<PortfolioAsset>
            {
                new PortfolioAsset{Name = DKK_STOCK.Name, Symbol = DKK_STOCK.Symbol, Quantity = 10},
                new PortfolioAsset{Name = USD_STOCK.Name, Symbol = USD_STOCK.Symbol, Quantity = 10}
            }
        };

        public static readonly PortfolioEntity OLD_PORTFOLIO = new PortfolioEntity
        {
            Id = "0b78b063-a53a-4266-8945-28eddc0b0c56",
            Capital = 100000.9M,
            PortfolioName = "9a302011-95e4-4b9e-a758-5f8d94fcf17b",
            OwnerId = TESTUSER_ID,
            CreatedAt = Convert.ToDateTime("5/25/2000 08:00:02 AM").ToUniversalTime()
        };

        public static readonly PortfolioEntity PORTFOLIO_WITH_PORTFOLIO_IN_NAME = new PortfolioEntity
        {
            Id = "205cb7c0-d1f0-40b9-94b4-1ad4a8c9b535",
            Capital = 100000.9M,
            PortfolioName = "PORTFOLIO#1",
            OwnerId = TESTUSER_ID,
            CreatedAt = Convert.ToDateTime("5/25/2021 08:00:03 AM").ToUniversalTime()
        };

        public static readonly PortfolioEntity NON_USER_PORTFOLIO = new PortfolioEntity
        {
            Id = "ec7ec42d-a6fd-476c-8bce-4a609ae67068",
            Capital = 100000.90M,
            PortfolioName = "Portfolio",
            OwnerId = "04fb0290-43f1-4ab7-baee-e8daa0e8b5af",
            CreatedAt = Convert.ToDateTime("5/25/2021 08:00:04 AM").ToUniversalTime()
        };

        public static readonly PortfolioEntity PORTFOLIO_WITH_GROWTH_POINTS = new PortfolioEntity
        {
            Id = "dd147826-2c37-4672-bc9f-3e613ebb9c5f",
            Capital = 100000.9M,
            PortfolioName = "7523aa58-74e3-490f-9c7e-3eed52f7f893",
            OwnerId = TESTUSER_ID,
            CreatedAt = Convert.ToDateTime("5/25/2011 08:00:03 AM").ToUniversalTime()
        };

        public static readonly PortfolioEntity NON_USER_PORTFOLIO_WITH_GROWTH_POINTS = new PortfolioEntity
        {
            Id = "99200409-2665-435e-ae84-d0ce9dbd680b",
            Capital = 100000.9M,
            PortfolioName = "89f5b800-3d3a-4fd8-b6ea-5fb11605252e",
            OwnerId = "04fb0290-43f1-4ab7-baee-e8daa0e8b5af",
            CreatedAt = Convert.ToDateTime("5/25/2011 08:00:04 AM").ToUniversalTime()
        };

        public static readonly PortfolioEntity PORTFOLIO_TO_UPDATE = new PortfolioEntity
        {
            Id = "9ca13b56-9fba-4951-918d-f04782d731ee",
            Capital = 100.9M,
            PortfolioName = "08036100-46f1-496f-aee3-4d7185e5fbb1",
            OwnerId = TESTUSER_ID,
            CreatedAt = Convert.ToDateTime("5/25/2021 08:00:04 AM").ToUniversalTime()
        };

        public static readonly PortfolioEntity PORTFOLIO_FOR_TRANSACTIONS = new PortfolioEntity
        {
            Id = "6076e5ef-830f-42cd-beef-4c1210531cf0",
            Capital = 100.9M,
            PortfolioName = "0a22a909-7732-400f-a532-2d67e8cd474f",
            OwnerId = TESTUSER_ID,
            CreatedAt = Convert.ToDateTime("5/25/2021 08:00:04 AM").ToUniversalTime()
        };
    }
}