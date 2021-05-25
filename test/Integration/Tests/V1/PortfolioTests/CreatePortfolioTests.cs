using System;
using System.Collections.Generic;
using System.Net.Http;
using Amazon.DynamoDBv2;
using Api;
using Business.Commands;
using Business.Extensions;
using Conditus.Trader.Domain.Entities;
using Conditus.Trader.Domain.Models;
using Database.Indexes;
using FluentAssertions;
using FluentAssertions.Execution;
using Integration.Utilities;
using Microsoft.AspNetCore.Http;
using Xunit;

using static Integration.Utilities.TestConstants;

namespace Integration.Tests.V1.PortfolioTests
{
    public class CreatePortfolioTests : IClassFixture<CustomWebApplicationFactory<Startup>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly IAmazonDynamoDB _db;
        
        public CreatePortfolioTests(CustomWebApplicationFactory<Startup> factory)
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
        public async void CreatePortfolio_withValidValues_ShouldReturnOkAndTheNewPortfolio()
        {
            //Given
            var createPortfolioCommand = new CreatePortfolioCommand
            {
                Name = Guid.NewGuid().ToString(),
                Capital = 1000
            };

            //When
            var httpResponse = await _client.PostAsync(BASE_URL, HttpSerializer.GetStringContent(createPortfolioCommand));

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status201Created);

            var newPortfolio = await httpResponse.GetDeserializedResponseBodyAsync<PortfolioDetail>();

            using (new AssertionScope())
            {
                newPortfolio.Should().NotBeNull();
                newPortfolio.Should().BeEquivalentTo(createPortfolioCommand, options => options
                    .ExcludingMissingMembers());
                newPortfolio.Id.Should().NotBeNullOrEmpty();
            }

            var dbPortfolio = await _db.LoadByLocalIndexAsync<PortfolioEntity>(
                TESTUSER_ID, 
                nameof(PortfolioEntity.Id), 
                newPortfolio.Id, 
                LocalIndexes.PortfolioIdIndex);
                
            dbPortfolio.Should().NotBeNull()
                .And.BeEquivalentTo(createPortfolioCommand, options => options
                    .ExcludingMissingMembers());
        }

        [Theory]
        [MemberData(nameof(CreatePortfolioCommandsWithMissingValues))]
        public async void CreatePortfolio_withMissingValues_ShouldReturnBadRequest(CreatePortfolioCommand createPortfolioCommand)
        {
            //When
            var httpResponse = await _client.PostAsync(BASE_URL, HttpSerializer.GetStringContent(createPortfolioCommand));

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        public static IEnumerable<object[]> CreatePortfolioCommandsWithMissingValues
        {
            get
            {
                yield return new Object[] { new CreatePortfolioCommand { Name = "PortfolioName" } };
                yield return new Object[] { new CreatePortfolioCommand { Capital = 100 } };
            }
        }
    }
}