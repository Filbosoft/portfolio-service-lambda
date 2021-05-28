using System;
using Conditus.Trader.Domain.Entities;

using static Integration.Tests.V1.TestConstants;
using static Integration.Seeds.V1.PortfolioSeeds;

namespace Integration.Seeds.V1
{
    public static class PortfolioGrowthPointSeeds
    {
        public static readonly PortfolioGrowthPoint DAY_OLD_GROWTH_POINT = new PortfolioGrowthPoint
        {
            PortfolioId = PORTFOLIO_WITH_GROWTH_POINTS.Id,
            OwnerId = TESTUSER_ID,
            CurrentGrowth = 100.1M,
            GrowthPointTimestamp = DateTime.UtcNow.AddDays(-1)
        };

        public static readonly PortfolioGrowthPoint MONTH_OLD_GROWTH_POINT = new PortfolioGrowthPoint
        {
            PortfolioId = PORTFOLIO_WITH_GROWTH_POINTS.Id,
            OwnerId = TESTUSER_ID,
            CurrentGrowth = 100.1M,
            GrowthPointTimestamp = DateTime.UtcNow.AddMonths(-1)
        };

        public static readonly PortfolioGrowthPoint YEAR_OLD_GROWTH_POINT = new PortfolioGrowthPoint
        {
            PortfolioId = PORTFOLIO_WITH_GROWTH_POINTS.Id,
            OwnerId = TESTUSER_ID,
            CurrentGrowth = 100.1M,
            GrowthPointTimestamp = DateTime.UtcNow.AddYears(-1)
        };

        public static readonly PortfolioGrowthPoint OLD_GROWTH_POINT = new PortfolioGrowthPoint
        {
            PortfolioId = PORTFOLIO_WITH_GROWTH_POINTS.Id,
            OwnerId = TESTUSER_ID,
            CurrentGrowth = 100.1M,
            GrowthPointTimestamp = Convert.ToDateTime("5/25/2001 08:00:04 AM").ToUniversalTime()
        };

        public static readonly PortfolioGrowthPoint NON_USER_GROWTH_POINT = new PortfolioGrowthPoint
        {
            PortfolioId = NON_USER_PORTFOLIO_WITH_GROWTH_POINTS.Id,
            OwnerId = NON_USER_PORTFOLIO_WITH_GROWTH_POINTS.OwnerId,
            CurrentGrowth = 100.1M,
            GrowthPointTimestamp = DateTime.UtcNow.AddDays(-1)
        };
    }
}