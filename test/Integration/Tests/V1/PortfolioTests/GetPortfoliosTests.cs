using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using Api;
using Domain.Models;
using FluentAssertions;
using Integration.Utilities;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Integration.Tests.V1.PortfolioTests
{
    public class GetPortfoliosTests : IClassFixture<CustomWebApplicationFactory<Startup>>, IDisposable
    {
        private readonly LambdaEntryPoint _entryPoint;
        private readonly TestLambdaContext _context;
        private readonly APIGatewayProxyRequest _request;
        private readonly IDynamoDBContext _dbContext;

        private const string PORTFOLIO_URI = "api/v1/portfolios";

        public GetPortfoliosTests(CustomWebApplicationFactory<Startup> factory)
        {
            _entryPoint = new LambdaEntryPoint();
            _context = new TestLambdaContext();
            
            _request = CustomWebApplicationFactory<Startup>.CreateBaseRequestV2();
            _request.Path = PORTFOLIO_URI;
            _request.PathParameters = new Dictionary<string, string>
            {
                {"proxy", PORTFOLIO_URI}
            };

            _dbContext = factory.GetDbContext();
            factory.Dispose();

            Setup();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        /**
        * * Seed values
        * Id prefix: 1003
        **/

        public const string UserWithContentId = "2fc366cd-5616-4503-b065-a228b50b60e5";
        public const string UserWithoutContentId = "457f4738-60fc-4ac6-a701-3fa89900e2ce";
        public static Portfolio Portfolio1 = new Portfolio
        {
            Id = Guid.NewGuid().ToString(),
            Name = "TestPortfolio1",
            Currency = "DKK",
            Owner = UserWithContentId
        };

        public static Portfolio Portfolio2 = new Portfolio
        {
            Id = Guid.NewGuid().ToString(),
            Name = "TestPortfolio2",
            Currency = "DKK",
            Owner = UserWithContentId
        };

        public static Portfolio Portfolio3 = new Portfolio
        {
            Id = Guid.NewGuid().ToString(),
            Name = "TestPortfolio3",
            Currency = "DKK",
            Owner = UserWithContentId
        };

        private async void Setup()
        {
            var seedPortfolios = new List<Portfolio>
            {
                Portfolio1,
                Portfolio2,
                Portfolio3
            };

            var batchWriteOperation = _dbContext.CreateBatchWrite<Portfolio>();
            batchWriteOperation.AddPutItems(seedPortfolios);

            await batchWriteOperation.ExecuteAsync();

        }

        [Fact]
        public async void GetPortfolios_ShouldReturnSeededPortfolios()
        {
            //Given
            //Portfolios has been seeded

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(_request, _context);

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status200OK);
            var portfolios = httpResponse.GetDeserializedResponseBody<IEnumerable<Portfolio>>();

            portfolios.Should().NotBeEmpty();
        }

        [Fact]
        public async void GetPortfolios_WithUserIdWithContent_ShouldReturnSeededUserPortfolios()
        {
            //Given
            //Portfolios has been seeded
            _request.QueryStringParameters = new Dictionary<string, string>
            {
                {"ownerId",UserWithContentId.ToString()}
            };

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(_request, _context);

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status200OK);
            var portfolios = httpResponse.GetDeserializedResponseBody<IEnumerable<Portfolio>>();

            portfolios.Should().NotBeEmpty();
        }

        [Fact]
        public async void GetPortfolios_WithUserIdWithoutContent_ShouldReturnEmpty()
        {
            //Given
            _request.QueryStringParameters = new Dictionary<string, string>
            {
                {"ownerId",UserWithoutContentId.ToString()}
            };

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(_request, _context);

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status200OK);
            var portfolios = httpResponse.GetDeserializedResponseBody<IEnumerable<Portfolio>>();

            portfolios.Should().BeEmpty();
        }

        [Fact]
        public async void GetPortfolios_WithNonExistingUserId_ShouldReturnEmpty()
        {
            //Given
            _request.QueryStringParameters = new Dictionary<string, string>
            {
                {"ownerId","99999999"}
            };

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(_request, _context);

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status200OK);
            var portfolios = httpResponse.GetDeserializedResponseBody<IEnumerable<Portfolio>>();

            portfolios.Should().BeEmpty();
        }
    }
}