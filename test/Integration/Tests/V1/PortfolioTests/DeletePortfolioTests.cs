using System;
using System.Net.Http;
using Api;
using Conditus.Trader.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;
using Amazon.DynamoDBv2;
using Conditus.DynamoDB.MappingExtensions.Mappers;
using Conditus.DynamoDB.QueryExtensions.Extensions;

using static Integration.Tests.V1.TestConstants;
using static Integration.Seeds.V1.PortfolioSeeds;

namespace Integration.Tests.V1.PortfolioTests
{
    public class DeletePortfolioTests : IClassFixture<CustomWebApplicationFactory<Startup>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly IAmazonDynamoDB _db;

        public DeletePortfolioTests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateAuthorizedClient();
            _db = factory.GetDynamoDB();
        }

        public void Dispose()
        {
            _client.Dispose();
            _db.Dispose();
        }

        public async void SeedPortfolio(PortfolioEntity seedPortfolio)
        {
            await _db.PutItemAsync(typeof(PortfolioEntity).GetDynamoDBTableName(), seedPortfolio.GetAttributeValueMap());
        }

        [Fact]
        public async void DeletePortfolio_WithEmptyPortfolio_ShouldReturnOk()
        {
            //Given
            SeedPortfolio(PORTFOLIO_TO_DELETE);
            var uri = $"{BASE_URL}/{PORTFOLIO_TO_DELETE.Id}";

            //When
            var httpResponse = await _client.DeleteAsync(uri);

            //Then
            httpResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public async void DeletePortfolio_WithPortfolioWithAssets_ShouldReturnBadRequest()
        {
            //Given
            SeedPortfolio(PORTFOLIO_WITH_ASSETS);
            var uri = $"{BASE_URL}/{PORTFOLIO_WITH_ASSETS.Id}";

            //When
            var httpResponse = await _client.DeleteAsync(uri);

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async void DeletePortfolio_WithNonUserPortfolio_ShouldReturnOk()
        {
            //Given
            SeedPortfolio(NON_USER_PORTFOLIO);
            var uri = $"{BASE_URL}/{NON_USER_PORTFOLIO.Id}";

            //When
            var httpResponse = await _client.DeleteAsync(uri);

            //Then
            httpResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public async void DeletePortfolio_WithNonExistingPortfolio_ShouldReturnOk()
        {
            //Given
            var uri = $"{BASE_URL}/{Guid.NewGuid()}";

            //When
            var httpResponse = await _client.DeleteAsync(uri);

            //Then
            httpResponse.EnsureSuccessStatusCode();
        }
    }
}