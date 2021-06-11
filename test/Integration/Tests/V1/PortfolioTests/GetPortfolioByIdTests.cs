using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Api;
using Conditus.Trader.Domain.Entities;
using FluentAssertions;
using Integration.Utilities;
using Microsoft.AspNetCore.Http;
using Xunit;
using Conditus.Trader.Domain.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Api.Responses.V1;
using Business.Queries;
using Microsoft.AspNetCore.Mvc;
using Conditus.DynamoDB.MappingExtensions.Mappers;
using Conditus.DynamoDB.QueryExtensions.Extensions;

using static Integration.Tests.V1.TestConstants;
using static Integration.Seeds.V1.PortfolioSeeds;

namespace Integration.Tests.V1.PortfolioTests
{
    public class GetPortfolioByIdTests : IClassFixture<CustomWebApplicationFactory<Startup>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly IAmazonDynamoDB _db;

        public GetPortfolioByIdTests(CustomWebApplicationFactory<Startup> factory)
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
        public async void GetPortfolioById_WithExistingId_ShouldReturnSeededPortfolio()
        {
            //Given
            var uri = $"{BASE_URL}/{PORTFOLIO_WITH_ASSETS.Id}";

            //When
            var httpResponse = await _client.GetAsync(uri);

            //Then
            httpResponse.EnsureSuccessStatusCode();
            var apiResponse = await httpResponse.GetDeserializedResponseBodyAsync<ApiResponse<PortfolioDetail>>();
            var portfolio = apiResponse.Data;

            portfolio.Should().NotBeNull()
                .And.BeEquivalentTo(PORTFOLIO_WITH_ASSETS, options => options.ExcludingMissingMembers());
        }

        [Fact]
        public async void GetPortfolioById_WithNonExistingId_ShouldReturnNotFound()
        {
            //Given
            var uri = $"{BASE_URL}/{Guid.NewGuid()}";

            //When
            var httpResponse = await _client.GetAsync(uri);

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            var problem = await httpResponse.GetDeserializedResponseBodyAsync<ProblemDetails>();

            problem.Title.Should().Be(GetPortfolioByIdResponseCodes.PortfolioNotFound.ToString());
        }

        [Fact]
        public async void GetPortfolioById_WithNonAuthorizedPortfolioId_ShouldReturnNotFound()
        {
            //Given
            var uri = $"{BASE_URL}/{NON_USER_PORTFOLIO.Id}";

            //When
            var httpResponse = await _client.GetAsync(uri);

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            var problem = await httpResponse.GetDeserializedResponseBodyAsync<ProblemDetails>();

            problem.Title.Should().Be(GetPortfolioByIdResponseCodes.PortfolioNotFound.ToString());
        }
    }
}