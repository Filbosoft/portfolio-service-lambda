using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Api;
using Conditus.Trader.Domain.Entities;
using Conditus.Trader.Domain.Models;
using FluentAssertions;
using Integration.Utilities;
using Xunit;
using Api.Responses.V1;
using Conditus.DynamoDB.MappingExtensions.Mappers;
using Conditus.DynamoDB.QueryExtensions.Extensions;

using static Integration.Tests.V1.TestConstants;
using static Integration.Seeds.V1.PortfolioSeeds;

namespace Integration.Tests.V1.PortfolioTests
{
    public class GetPortfoliosTests : IClassFixture<CustomWebApplicationFactory<Startup>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly IAmazonDynamoDB _db;

        public GetPortfoliosTests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateAuthorizedClient();
            _db = factory.GetDynamoDB();

            Setup();
        }

        public void Dispose()
        {
            _client.Dispose();
            _db.Dispose();
        }

        private async void Setup()
        {

            var seedPortfolios = new List<PortfolioEntity>
            {
                PORTFOLIO_WITH_ASSETS,
                PORTFOLIO_WITH_PORTFOLIO_IN_NAME,
                OLD_PORTFOLIO,
                NON_USER_PORTFOLIO
            };

            var writeRequests = seedPortfolios
                .Select(p => new PutRequest { Item = p.GetAttributeValueMap() })
                .Select(p => new WriteRequest { PutRequest = p })
                .ToList();

            var batchWriteRequest = new BatchWriteItemRequest
            {
                RequestItems = new Dictionary<string, List<WriteRequest>>
                {
                    { typeof(PortfolioEntity).GetDynamoDBTableName(), writeRequests }
                }
            };

            await _db.BatchWriteItemAsync(batchWriteRequest);
        }

        [Fact]
        public async void GetPortfolios_WithoutQueryParameters_ShouldReturnSeededUserPortfolios()
        {
            //Given
            //Portfolios has been seeded

            //When
            var httpResponse = await _client.GetAsync(BASE_URL);

            //Then
            httpResponse.EnsureSuccessStatusCode();
            var apiResponse = await httpResponse.GetDeserializedResponseBodyAsync<ApiResponse<IEnumerable<PortfolioOverview>>>();
            var portfolios = apiResponse.Data;

            portfolios.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async void GetPortfolios_WithNameQuery_ShouldReturnSeededUserPortfoliosWhereNameContainsNameQuery()
        {
            //Given
            var query = $"?nameQuery={PORTFOLIO_WITH_PORTFOLIO_IN_NAME.PortfolioName}";
            var uri = BASE_URL + query;

            //When
            var httpResponse = await _client.GetAsync(uri);

            //Then
            httpResponse.EnsureSuccessStatusCode();
            var apiResponse = await httpResponse.GetDeserializedResponseBodyAsync<ApiResponse<IEnumerable<PortfolioOverview>>>();
            var portfolios = apiResponse.Data;

            portfolios.Should().NotBeNullOrEmpty()
                .And.OnlyContain(p => p.Name.Contains(PORTFOLIO_WITH_PORTFOLIO_IN_NAME.PortfolioName));
        }

        [Fact]
        public async void GetPortfolios_WithCreatedFromDate_ShouldReturnSeededUserPortfoliosFilteredByCreatedFromDate()
        {
            //Given
            var query = $"?createdFromDate={PORTFOLIO_WITH_ASSETS.CreatedAt}";
            var uri = BASE_URL + query;

            //When
            var httpResponse = await _client.GetAsync(uri);

            //Then
            httpResponse.EnsureSuccessStatusCode();
            var apiResponse = await httpResponse.GetDeserializedResponseBodyAsync<ApiResponse<IEnumerable<PortfolioOverview>>>();
            var portfolios = apiResponse.Data;

            portfolios.Should().NotBeNullOrEmpty()
                .And.NotContain(p => p.Id.Equals(OLD_PORTFOLIO.Id));

            var portfolioWithAssets = portfolios.FirstOrDefault(p => p.Id.Equals(PORTFOLIO_WITH_ASSETS.Id));

            portfolioWithAssets.Should().NotBeNull();
        }
    }
}