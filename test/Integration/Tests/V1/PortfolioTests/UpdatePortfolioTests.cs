using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Api;
using Business.Commands;
using Business.Extensions;
using Business.HelperMethods;
using Conditus.DynamoDBMapper.Mappers;
using Conditus.Trader.Domain.Entities;
using Conditus.Trader.Domain.Models;
using Database.Indexes;
using FluentAssertions;
using FluentAssertions.Execution;
using Integration.Utilities;
using Microsoft.AspNetCore.Http;
using Xunit;

using static Integration.Seeds.V1.PortfolioSeeds;
using static Integration.Tests.V1.TestConstants;

namespace Integration.Tests.V1.PortfolioTests
{
    public class UpdatePortfolioTests : IClassFixture<CustomWebApplicationFactory<Startup>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly IAmazonDynamoDB _db;

        public UpdatePortfolioTests(CustomWebApplicationFactory<Startup> factory)
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
                PORTFOLIO_TO_UPDATE
            };

            var writeRequests = seedPortfolios
                .Select(p => new PutRequest { Item = p.GetAttributeValueMap() })
                .Select(p => new WriteRequest { PutRequest = p })
                .ToList();

            var batchWriteRequest = new BatchWriteItemRequest
            {
                RequestItems = new Dictionary<string, List<WriteRequest>>
                {
                    { DynamoDBHelper.GetDynamoDBTableName<PortfolioEntity>(), writeRequests }
                }
            };

            await _db.BatchWriteItemAsync(batchWriteRequest);
        }

        [Fact]
        public async void UpdatePortfolio_withName_ShouldReturnAcceptedAndTheUpdatedPortfolio()
        {
            //Given
            var uri = $"{BASE_URL}/{PORTFOLIO_TO_UPDATE.Id}";
            var portfolioUpdator = new UpdatePortfolioCommand
            {
                Name = "portfolioName_updated"
            };

            //When
            var httpResponse = await _client.PutAsync(uri, HttpSerializer.GetStringContent(portfolioUpdator));

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status202Accepted);
            var updatedPortfolio = await httpResponse.GetDeserializedResponseBodyAsync<PortfolioDetail>();

            using (new AssertionScope())
            {
                updatedPortfolio.Should().NotBeNull();
                updatedPortfolio.Name.Should().Be(portfolioUpdator.Name);
                updatedPortfolio.Should().BeEquivalentTo(PORTFOLIO_TO_UPDATE, options => options
                    .ExcludingMissingMembers());
            }

            var dbPortfolio = await _db.LoadByLocalIndexAsync<PortfolioEntity>(
                TESTUSER_ID, 
                nameof(PortfolioEntity.Id), 
                PORTFOLIO_TO_UPDATE.Id, 
                PortfolioLocalIndexes.PortfolioIdIndex);

            using (new AssertionScope())
            {
                dbPortfolio.Should().NotBeNull()
                    .And.BeEquivalentTo(PORTFOLIO_TO_UPDATE, options => options
                        .Excluding(p => p.PortfolioName)
                        .ExcludingMissingMembers());
                dbPortfolio.PortfolioName.Should().Equals(portfolioUpdator.Name);
            }
        }

        [Fact]
        public async void UpdatePortfolio_withNonExistingId_ShouldReturnNotFound()
        {
            //Given
            var uri = $"{BASE_URL}/{Guid.NewGuid()}";
            var portfolioUpdator = new UpdatePortfolioCommand
            {
                Name = "Updated"
            };

            //When
            var httpResponse = await _client.PutAsync(uri, HttpSerializer.GetStringContent(portfolioUpdator));

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }
    }
}