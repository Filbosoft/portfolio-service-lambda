using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using Api;
using Business.Commands;
using Domain.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Configuration;
using Test.Integration.Utilities;
using Xunit;

namespace Integration.Tests.V1.PortfolioTests
{
    public class CreatePortfolioTests : IClassFixture<CustomWebApplicationFactory<Startup>>, IDisposable
    {
        private readonly LambdaEntryPoint _entryPoint;
        private readonly TestLambdaContext _context;
        private readonly APIGatewayProxyRequest _request;
        private readonly IDynamoDBContext _db;
        
        private const string PORTFOLIO_URI = "api/v1";
        public CreatePortfolioTests(CustomWebApplicationFactory<Startup> factory)
        {
            factory.CreateClient();
            _entryPoint = new LambdaEntryPoint();
            _context = new TestLambdaContext();
            _request = factory.CreateBaseRequest();
            _db = factory.GetDbContext();
        }

        public void Dispose()
        {
        }

        [Fact]
        public async void CreatePortfolio_withValidValues_ShouldReturnOkAndTheNewPortfolio()
        {
            //Given
            var createPortfolioCommand = new CreatePortfolioCommand
            {
                Name = Guid.NewGuid().ToString(),
                Currency = "DKK",
                Owner = 1
            };

            _request.HttpMethod = HttpMethod.Post.ToString();
            _request.Path = PORTFOLIO_URI;
            _request.PathParameters = new Dictionary<string, string>
            {
                {"proxy", PORTFOLIO_URI}
            };
            _request.Body = JsonSerializer.Serialize(createPortfolioCommand);


            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(_request, _context);

            //Then
            httpResponse.StatusCode.Should().Equals(HttpStatusCode.Created);

            var newPortfolio = httpResponse.GetDeserializedResponseBody<Portfolio>();

            using (new AssertionScope())
            {
                newPortfolio.Should().NotBeNull();
                newPortfolio.Should().BeEquivalentTo(createPortfolioCommand, options => options
                    .ExcludingMissingMembers());
                newPortfolio.Id.Should().NotBeNullOrEmpty();
            }

            var dbPortfolio = await _db.LoadAsync<Portfolio>(newPortfolio.Id);
            dbPortfolio.Should().NotBeNull()
                .And.BeEquivalentTo(createPortfolioCommand, options => options
                    .ExcludingMissingMembers());
        }
    }
}