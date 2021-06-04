using System;
using System.Net.Http;
using Amazon.DynamoDBv2;
using Api;
using Business.Commands;
using Conditus.DynamoDB.MappingExtensions.Mappers;
using Conditus.DynamoDB.QueryExtensions.Extensions;
using Conditus.Trader.Domain.Entities;
using Conditus.Trader.Domain.Entities.Indexes;
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

        [Fact]
        public async void CreatePortfolioTransaction_WithPositiveAmount_ShouldReturnCreatedAndPortfolioCapitalShouldHaveBeenIncreased()
        {
            //Given
            await _db.PutItemAsync(
                typeof(PortfolioEntity).GetDynamoDBTableName(),
                PORTFOLIO_FOR_TRANSACTIONS.GetAttributeValueMap());

            var uri = $"{BASE_URL}/{PORTFOLIO_FOR_TRANSACTIONS.Id}/transactions";
            var transactionCreateCommand = new CreatePortfolioTransactionCommand
            {
                Amount = 100
            };

            //When
            var httpResponse = await _client.PostAsync(uri, HttpSerializer.GetStringContent(transactionCreateCommand));

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status201Created);

            var dbPortfolio = await _db.LoadByLocalSecondaryIndexAsync<PortfolioEntity>(
                TESTUSER_ID.GetAttributeValue(),
                PORTFOLIO_FOR_TRANSACTIONS.Id.GetAttributeValue(),
                PortfolioLocalSecondaryIndexes.PortfolioIdIndex);

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
        public async void CreatePortfolioTransaction_WithNegativeAmount_ShouldReturnCreatedAndPortfolioCapitalShouldHaveBeenDecreased()
        {
            //Given
            await _db.PutItemAsync(
                typeof(PortfolioEntity).GetDynamoDBTableName(),
                PORTFOLIO_FOR_TRANSACTIONS.GetAttributeValueMap());

            var uri = $"{BASE_URL}/{PORTFOLIO_FOR_TRANSACTIONS.Id}/transactions";
            var transactionCreateCommand = new CreatePortfolioTransactionCommand
            {
                Amount = (PORTFOLIO_FOR_TRANSACTIONS.Capital / 10) * -1
            };

            //When
            var httpResponse = await _client.PostAsync(uri, HttpSerializer.GetStringContent(transactionCreateCommand));

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status201Created);

            var dbPortfolio = await _db.LoadByLocalSecondaryIndexAsync<PortfolioEntity>(
                TESTUSER_ID.GetAttributeValue(),
                PORTFOLIO_FOR_TRANSACTIONS.Id.GetAttributeValue(),
                PortfolioLocalSecondaryIndexes.PortfolioIdIndex);

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
        public async void CreatePortfolioTransaction_WithNegativeAmountLargerThanPortfolioCapital_ShouldReturnBadRequest()
        {
            //Given
            await _db.PutItemAsync(
                typeof(PortfolioEntity).GetDynamoDBTableName(),
                PORTFOLIO_FOR_TRANSACTIONS.GetAttributeValueMap());

            var uri = $"{BASE_URL}/{PORTFOLIO_FOR_TRANSACTIONS.Id}/transactions";
            var transactionCreateCommand = new CreatePortfolioTransactionCommand
            {
                Amount = PORTFOLIO_FOR_TRANSACTIONS.Capital * -1.1M
            };

            //When
            var httpResponse = await _client.PostAsync(uri, HttpSerializer.GetStringContent(transactionCreateCommand));

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async void CreatePortfolioTransaction_WithNonExistingId_ShouldReturnNotFound()
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