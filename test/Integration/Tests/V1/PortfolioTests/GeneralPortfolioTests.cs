using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using Api;
using Business.Commands;
using Business.Commands.PortfolioCommands;
using DataAccess;
using Domain.Models;
using FluentAssertions;
using Integration.Utilities;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Integration.Tests.V1.PortfolioTests
{
    public class GeneralPortfolioTests : IClassFixture<CustomWebApplicationFactory<Startup>>, IDisposable
    {
        private readonly LambdaEntryPoint _entryPoint;
        private readonly TestLambdaContext _context;
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly IDynamoDBContext _db;
        private const string PORTFOLIO_URI = "api/v1/portfolios";

        public GeneralPortfolioTests(CustomWebApplicationFactory<Startup> factory)
        {
            _entryPoint = new LambdaEntryPoint();
            _context = new TestLambdaContext();
            _factory = factory;
            _db = factory.GetDbContext();
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        /**
        * * Seed values
        * Id prefix: 1005
        **/

        [Fact]
        public async void CRUD_ShouldBeSuccessful()
        {
            var newPortfolio = await CreatePortfolio();
            await ReadPortfolio(newPortfolio);
            var updatedPortfolio = await UpdatePortfolio(newPortfolio);
            DeletePortfolio(updatedPortfolio);
        }

        private async Task<Portfolio> CreatePortfolio()
        {
            //Given
            var portfolioCreator = new CreatePortfolioCommand
            {
                Name = "portfolioName",
                Currency = "DKK",
                Owner = "2812e65f-e43f-4fc4-8358-06095c8da1db"
            };
            var request = _factory.CreateBaseRequest();
            request.HttpMethod = HttpMethod.Post.ToString();
            request.Path = PORTFOLIO_URI;
            request.PathParameters = new Dictionary<string, string>
            {
                {"proxy", PORTFOLIO_URI}
            };
            request.Body = JsonSerializer.Serialize(portfolioCreator);

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(request, _context);

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status201Created);

            var newPortfolio = httpResponse.GetDeserializedResponseBody<Portfolio>();
            newPortfolio.Should().NotBeNull();

            return newPortfolio;
        }

        private async Task<Portfolio> ReadPortfolio(Portfolio portfolio)
        {
            //Given
            var portfolioUri = $"{PORTFOLIO_URI}/{portfolio.Id}";
            var request = _factory.CreateBaseRequest();
            request.Path = portfolioUri;
            request.PathParameters = new Dictionary<string, string>
            {
                {"proxy", portfolioUri}
            };

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(request, _context);

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status200OK);

            var readPortfolio = httpResponse.GetDeserializedResponseBody<Portfolio>();

            readPortfolio.Should()
                .NotBeNull()
                .And.BeEquivalentTo(portfolio, options => 
                    options.ExcludingMissingMembers());

            return readPortfolio;
        }

        private async Task<Portfolio> UpdatePortfolio(Portfolio portfolio)
        {
            //Given
            var portfolioUri = $"{PORTFOLIO_URI}/{portfolio.Id}";
            portfolio.Name = "UpdatedName";
            var request = _factory.CreateBaseRequest();
            request.HttpMethod = HttpMethod.Put.ToString();
            request.Path = portfolioUri;
            request.PathParameters = new Dictionary<string, string>
            {
                {"proxy", portfolioUri}
            };
            request.Body = JsonSerializer.Serialize(portfolio);

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(request, _context);

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status202Accepted);

            var updatedPortfolio = httpResponse.GetDeserializedResponseBody<Portfolio>();

            updatedPortfolio.Should().BeEquivalentTo(portfolio);

            return updatedPortfolio;
        }

        private async void DeletePortfolio(Portfolio portfolio)
        {
            //Given
            var portfolioUri = $"{PORTFOLIO_URI}/{portfolio.Id}";
            var request = _factory.CreateBaseRequest();
            request.HttpMethod = HttpMethod.Delete.ToString();
            request.Path = portfolioUri;
            request.PathParameters = new Dictionary<string, string>
            {
                {"proxy", portfolioUri}
            };

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(request, _context);

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status200OK);
        }
    }
}