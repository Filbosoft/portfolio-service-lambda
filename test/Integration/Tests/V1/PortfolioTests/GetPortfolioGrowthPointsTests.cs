using System;
using System.Collections.Generic;
using System.Net.Http;
using Api;
using Conditus.Trader.Domain.Entities;
using FluentAssertions;
using Integration.Utilities;
using Xunit;
using Amazon.DynamoDBv2.DataModel;
using System.Net;

using static Integration.Utilities.TestConstants;
using static Integration.Seeds.PortfolioSeeds;
using static Integration.Seeds.PortfolioGrowthPointSeeds;

namespace Integration.Tests.V1.PortfolioTests
{
    public class GetPortfolioGrowthPointsTests : IClassFixture<CustomWebApplicationFactory<Startup>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly IDynamoDBContext _dbContext;

        public GetPortfolioGrowthPointsTests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateAuthorizedClient();
            _dbContext = factory.GetDynamoDBContext();

            Setup();
        }

        public void Dispose()
        {
            _client.Dispose();
            _dbContext.Dispose();
        }

        private async void Setup()
        {
            var seedGrowthPoints = new List<PortfolioGrowthPoint>
            {
                DAY_OLD_GROWTH_POINT,
                MONTH_OLD_GROWTH_POINT,
                YEAR_OLD_GROWTH_POINT,
                OLD_GROWTH_POINT,
                NON_USER_GROWTH_POINT
            };

            var batchWriteRequest = _dbContext.CreateBatchWrite<PortfolioGrowthPoint>();
            batchWriteRequest.AddPutItems(seedGrowthPoints);

            await batchWriteRequest.ExecuteAsync();
        }

        [Fact]
        public async void GetPortfolioGrowthPoints_WithUserPortfolioId_ShouldReturnGrowthPointsForTheLastMonth()
        {
            //Given
            var uri = $"{BASE_URL}/{PORTFOLIO_WITH_GROWTH_POINTS.Id}/growthpoints";

            //When
            var httpResponse = await _client.GetAsync(uri);

            //Then
            httpResponse.EnsureSuccessStatusCode();
            var growthPoints = await httpResponse.GetDeserializedResponseBodyAsync<IEnumerable<PortfolioGrowthPoint>>();

            growthPoints.Should().NotBeNullOrEmpty()
                .And.OnlyContain(g => g.GrowthPointTimestamp > DateTime.UtcNow.AddMonths(-1));
        }

        [Fact]
        public async void GetPortfolioGrowthPoints_WithNonUserPortfolioId_ShouldReturnNoGrowthPoints()
        {
            //Given
            var uri = $"{BASE_URL}/{NON_USER_PORTFOLIO_WITH_GROWTH_POINTS.Id}/growthpoints";

            //When
            var httpResponse = await _client.GetAsync(uri);

            //Then
            httpResponse.EnsureSuccessStatusCode();
            var growthPoints = await httpResponse.GetDeserializedResponseBodyAsync<IEnumerable<PortfolioGrowthPoint>>();

            growthPoints.Should().BeNullOrEmpty();
        }

        [Fact]
        public async void GetPortfolioGrowthPoints_WithFromDateSpecified_ShouldReturnGrowthPointsFromTheSpecifiedDate()
        {
            //Given
            var fromDate = DateTime.UtcNow.AddYears(-2);
            var query = $"?fromDate={fromDate.ToString()}";
            var uri = $"{BASE_URL}/{PORTFOLIO_WITH_GROWTH_POINTS.Id}/growthpoints{query}";

            //When
            var httpResponse = await _client.GetAsync(uri);

            //Then
            httpResponse.EnsureSuccessStatusCode();
            var growthPoints = await httpResponse.GetDeserializedResponseBodyAsync<IEnumerable<PortfolioGrowthPoint>>();

            growthPoints.Should().NotBeNullOrEmpty()
                .And.OnlyContain(g => g.GrowthPointTimestamp > fromDate);
        }

        [Fact]
        public async void GetPortfolioGrowthPoints_WithToDateSpecified_ShouldReturnGrowthPointsOnlyToTheSpecifiedDate()
        {
            //Given
            var fromDate = DateTime.UtcNow.AddYears(-2);
            var toDate = DateTime.UtcNow.AddMonths(-1);
            var query = $"?fromDate={fromDate.ToString()}&toDate={toDate.ToString()}";
            var uri = $"{BASE_URL}/{PORTFOLIO_WITH_GROWTH_POINTS.Id}/growthpoints{query}";

            //When
            var httpResponse = await _client.GetAsync(uri);

            //Then
            httpResponse.EnsureSuccessStatusCode();
            var growthPoints = await httpResponse.GetDeserializedResponseBodyAsync<IEnumerable<PortfolioGrowthPoint>>();

            growthPoints.Should().NotBeNullOrEmpty()
                .And.OnlyContain(g => g.GrowthPointTimestamp > fromDate && g.GrowthPointTimestamp < toDate);
        }
        
        [Fact]
        public async void GetPortfolioGrowthPoints_WithToBigInterval_ShouldReturnBadRequest()
        {
            //Given
            var fromDate = DateTime.UtcNow.AddYears(-11);
            var query = $"?fromDate={fromDate.ToString()}";
            var uri = $"{BASE_URL}/{PORTFOLIO_WITH_GROWTH_POINTS.Id}/growthpoints{query}";

            //When
            var httpResponse = await _client.GetAsync(uri);

            //Then
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}