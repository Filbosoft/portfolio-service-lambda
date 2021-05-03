using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using Api;
using Business.Commands;
using Domain.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Integration.Utilities;
using Xunit;


namespace Integration.Tests.V1.PortfolioTests
{
    public class UpdatePortfolioTests : IClassFixture<CustomWebApplicationFactory<Startup>>, IDisposable
    {
        private readonly LambdaEntryPoint _entryPoint;
        private readonly TestLambdaContext _context;
        private readonly APIGatewayProxyRequest _request;
        private readonly IDynamoDBContext _db;

        private const string PORTFOLIO_URI = "api/v1";

        public UpdatePortfolioTests(CustomWebApplicationFactory<Startup> factory)
        {
            _entryPoint = new LambdaEntryPoint();
            _context = new TestLambdaContext();

            _request = factory.CreateBaseRequest();
            _request.HttpMethod = HttpMethod.Put.ToString();

            _db = factory.GetDbContext();

            Setup();
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        /**
        * * Seed values
        * Id prefix: 1004
        **/

        public readonly static Portfolio Portfolio1 = new Portfolio
        {
            Id = Guid.NewGuid().ToString(),
            Name = "TestPortfolio1",
            Currency = "DKK",
            Owner = 10040001
        };

        private async void Setup()
        {
            await _db.SaveAsync<Portfolio>(Portfolio1);
        }

        [Fact]
        public async void UpdatePortfolio_withAllValues_ShouldReturnAcceptedAndTheUpdatedPortfolio()
        {
            //Given
            var uri = $"{PORTFOLIO_URI}/{Portfolio1.Id}";
            var portfolioUpdator = new UpdatePortfolioCommand
            {
                Id = Portfolio1.Id,
                Name = "portfolioName_updated",
                Currency = "USD",
                Owner = 1004002
            };

            _request.Path = uri;
            _request.PathParameters = new Dictionary<string, string>
            {
                {"proxy", uri}
            };
            _request.Body = JsonSerializer.Serialize(portfolioUpdator);

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(_request, _context);

            //Then
            httpResponse.Should().Equals(HttpStatusCode.Accepted);
            var updatedPortfolio = httpResponse.GetDeserializedResponseBody<Portfolio>();

            updatedPortfolio.Should().NotBeNull()
                .And.BeEquivalentTo(portfolioUpdator, options => options.ExcludingMissingMembers());

            var dbPortfolio = await _db.LoadAsync<Portfolio>(Portfolio1.Id);

            dbPortfolio.Should().BeEquivalentTo(portfolioUpdator, options => options
                .ExcludingMissingMembers());
        }

        [Fact]
        public async void UpdatePortfolio_withOnlyName_ShouldReturnAcceptedAndOnlyHaveUpdatedName()
        {
            //Given
            var uri = $"{PORTFOLIO_URI}/{Portfolio1.Id}";
            var portfolioUpdator = new UpdatePortfolioCommand
            {
                Id = Portfolio1.Id,
                Name = "UpdatedName"
            };

            _request.Path = uri;
            _request.PathParameters = new Dictionary<string, string>
            {
                {"proxy", uri}
            };
            _request.Body = JsonSerializer.Serialize(portfolioUpdator);

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(_request, _context);

            //Then
            httpResponse.Should().Equals(HttpStatusCode.Accepted);
            var updatedPortfolio = httpResponse.GetDeserializedResponseBody<Portfolio>();

            using (new AssertionScope())
            {
                updatedPortfolio.Should().NotBeNull();
                updatedPortfolio.Name.Should().Be(portfolioUpdator.Name);
                updatedPortfolio.Should().BeEquivalentTo(Portfolio1, options => options
                    .Excluding(p => p.Name)
                    .ExcludingMissingMembers());
            }

            var dbPortfolio = await _db.LoadAsync<Portfolio>(Portfolio1.Id);
            using (new AssertionScope())
            {
                dbPortfolio.Should().BeEquivalentTo(Portfolio1, options => options
                    .Excluding(p => p.Name)
                    .ExcludingMissingMembers());
                dbPortfolio.Name.Should().Equals(portfolioUpdator.Name);
            }
        }

        [Fact]
        public async void UpdatePortfolio_withOnlyCurrency_ShouldReturnAcceptedAndOnlyHaveUpdatedCurrency()
        {
            //Given
            var uri = $"{PORTFOLIO_URI}/{Portfolio1.Id}";
            var portfolioUpdator = new UpdatePortfolioCommand
            {
                Id = Portfolio1.Id,
                Currency = "USD"
            };

            _request.Path = uri;
            _request.PathParameters = new Dictionary<string, string>
            {
                {"proxy", uri}
            };
            _request.Body = JsonSerializer.Serialize(portfolioUpdator);

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(_request, _context);

            //Then
            httpResponse.Should().Equals(HttpStatusCode.Accepted);
            var updatedPortfolio = httpResponse.GetDeserializedResponseBody<Portfolio>();

            using (new AssertionScope())
            {
                updatedPortfolio.Should().NotBeNull();
                updatedPortfolio.Currency.Should().Be(portfolioUpdator.Currency);
                updatedPortfolio.Should().BeEquivalentTo(Portfolio1, options => options
                    .Excluding(p => p.Currency)
                    .ExcludingMissingMembers());
            }

            var dbPortfolio = await _db.LoadAsync<Portfolio>(Portfolio1.Id);
            using (new AssertionScope())
            {
                dbPortfolio.Should().BeEquivalentTo(Portfolio1, options => options
                    .Excluding(p => p.Currency)
                    .ExcludingMissingMembers());
                dbPortfolio.Currency.Should().Equals(portfolioUpdator.Currency);
            }
        }

        [Fact]
        public async void UpdatePortfolio_withOnlyUserId_ShouldReturnAcceptedAndOnlyHaveUpdatedUserId()
        {
            //Given
            var uri = $"{PORTFOLIO_URI}/{Portfolio1.Id}";
            var portfolioUpdator = new UpdatePortfolioCommand
            {
                Id = Portfolio1.Id,
                Owner = 10040002
            };

            _request.Path = uri;
            _request.PathParameters = new Dictionary<string, string>
            {
                {"proxy", uri}
            };
            _request.Body = JsonSerializer.Serialize(portfolioUpdator);

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(_request, _context);

            //Then
            httpResponse.Should().Equals(HttpStatusCode.Accepted);
            var updatedPortfolio = httpResponse.GetDeserializedResponseBody<Portfolio>();

            using (new AssertionScope())
            {
                updatedPortfolio.Should().NotBeNull();
                updatedPortfolio.Owner.Should().Be(portfolioUpdator.Owner);
                updatedPortfolio.Should().BeEquivalentTo(Portfolio1, options => options
                    .Excluding(p => p.Owner)
                    .ExcludingMissingMembers());
            }

            var dbPortfolio = await _db.LoadAsync<Portfolio>(Portfolio1.Id);
            using (new AssertionScope())
            {
                dbPortfolio.Should().BeEquivalentTo(Portfolio1, options => options
                    .Excluding(p => p.Owner)
                    .ExcludingMissingMembers());
                dbPortfolio.Owner.Should().Equals(portfolioUpdator.Owner);
            }
        }

        [Fact]
        public async void UpdatePortfolio_withNonExistingId_ShouldReturnNotFound()
        {
            //Given
            var uri = $"{PORTFOLIO_URI}/{Guid.NewGuid()}";
            var portfolioUpdator = new UpdatePortfolioCommand
            {
                Name = "Updated"
            };

            _request.Path = uri;
            _request.PathParameters = new Dictionary<string, string>
            {
                {"proxy", uri}
            };
            _request.Body = JsonSerializer.Serialize(portfolioUpdator);

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(_request, _context);

            //Then
            httpResponse.StatusCode.Should().Equals(HttpStatusCode.NotFound);
        }
    }
}