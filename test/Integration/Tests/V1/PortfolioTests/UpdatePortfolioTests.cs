using System;
using System.Net.Http;
using Amazon.DynamoDBv2;
using Api;
using Api.Responses.V1;
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
using Microsoft.AspNetCore.Mvc;
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
        }

        public void Dispose()
        {
            _client.Dispose();
            _db.Dispose();
        }

        [Fact]
        public async void UpdatePortfolio_withName_ShouldReturnAcceptedAndTheUpdatedPortfolio()
        {
            //Given
            await _db.PutItemAsync(
                DynamoDBHelper.GetDynamoDBTableName<PortfolioEntity>(),
                PORTFOLIO_TO_UPDATE.GetAttributeValueMap());
                
            var uri = $"{BASE_URL}/{PORTFOLIO_TO_UPDATE.Id}";
            var portfolioUpdater = new UpdatePortfolioCommand
            {
                Name = "portfolioName_updated"
            };

            //When
            var httpResponse = await _client.PutAsync(uri, HttpSerializer.GetStringContent(portfolioUpdater));

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status202Accepted);
            var apiResponse = await httpResponse.GetDeserializedResponseBodyAsync<ApiResponse<PortfolioDetail>>();
            var updatedPortfolio = apiResponse.Data;

            using (new AssertionScope())
            {
                updatedPortfolio.Should().NotBeNull();
                updatedPortfolio.Name.Should().Be(portfolioUpdater.Name);
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
                dbPortfolio.PortfolioName.Should().Equals(portfolioUpdater.Name);
            }
        }

        [Fact]
        public async void UpdatePortfolio_withNonExistingId_ShouldReturnNotFound()
        {
            //Given
            var uri = $"{BASE_URL}/{Guid.NewGuid()}";
            var portfolioUpdater = new UpdatePortfolioCommand
            {
                Name = "Updated"
            };

            //When
            var httpResponse = await _client.PutAsync(uri, HttpSerializer.GetStringContent(portfolioUpdater));

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            var problem = await httpResponse.GetDeserializedResponseBodyAsync<ProblemDetails>();

            problem.Title.Should().Be(UpdatePortfolioResponseCodes.PortfolioNotFound.ToString());
        }
    }
}