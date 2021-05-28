using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Api;
using Business.Commands;
using Business.Extensions;
using Business.HelperMethods;
using Conditus.DynamoDBMapper.Mappers;
using Conditus.Trader.Domain.Entities;
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
    public class CreatePortfolioTransactionTests : IClassFixture<CustomWebApplicationFactory<Startup>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly IAmazonDynamoDB _db;

        public CreatePortfolioTransactionTests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateAuthorizedClient();
            _db = factory.GetDynamoDB();
        }

        public void Dispose()
        {
            _client.Dispose();
            _db.Dispose();
        }

        private async void Seed()
        {
            await _db.PutItemAsync(
                DynamoDBHelper.GetDynamoDBTableName<PortfolioEntity>(),
                PORTFOLIO_FOR_TRANSACTIONS.GetAttributeValueMap());
        }

        [Fact]
        public async void CreatePortfolioTransaction_WithValidAmount_ShouldReturnCreatedAndPortfolioCapitalShouldHaveBeenIncreased()
        {
            //Given
            Seed();
            var uri = $"{BASE_URL}/{PORTFOLIO_FOR_TRANSACTIONS.Id}/transactions";
            var transactionCreateCommand = new CreatePortfolioTransactionCommand
            {
                Amount = 100
            };

            //When
            var httpResponse = await _client.PostAsync(uri, HttpSerializer.GetStringContent(transactionCreateCommand));

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status201Created);

            var dbPortfolio = await _db.LoadByLocalIndexAsync<PortfolioEntity>(
                TESTUSER_ID, 
                nameof(PortfolioEntity.Id), 
                PORTFOLIO_FOR_TRANSACTIONS.Id, 
                PortfolioLocalIndexes.PortfolioIdIndex);

            using (new AssertionScope())
            {
                dbPortfolio.Should().NotBeNull()
                    .And.BeEquivalentTo(PORTFOLIO_FOR_TRANSACTIONS, options => options
                        .Excluding(p => p.PortfolioName)
                        .Excluding(p => p.Capital)
                        .ExcludingMissingMembers());
                dbPortfolio.Capital.Should().Be(PORTFOLIO_FOR_TRANSACTIONS.Capital + transactionCreateCommand.Amount);
            }
        }

        [Fact]
        public async void CreatePortfolioTransaction_WithNegativeAmount_ShouldReturnBadRequest()
        {
            //Given
            Seed();
            var uri = $"{BASE_URL}/{PORTFOLIO_FOR_TRANSACTIONS.Id}/transactions";
            var transactionCreateCommand = new CreatePortfolioTransactionCommand
            {
                Amount = -100
            };

            //When
            var httpResponse = await _client.PostAsync(uri, HttpSerializer.GetStringContent(transactionCreateCommand));

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async void UpdatePortfolio_WithNonExistingId_ShouldReturnNotFound()
        {
            //Given
            var uri = $"{BASE_URL}/{Guid.NewGuid()}/transactions";
            var portfolioTransactionCommand = new CreatePortfolioTransactionCommand
            {
                Amount = 100
            };

            //When
            var httpResponse = await _client.PostAsync(uri, HttpSerializer.GetStringContent(portfolioTransactionCommand));

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }
    }
}