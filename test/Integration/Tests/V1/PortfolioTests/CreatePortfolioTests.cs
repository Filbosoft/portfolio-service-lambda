using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using Api;
using Business.Commands;
using Domain.Models;
using FluentAssertions;
using Test.Integration.Utilities;
using Xunit;

namespace Integration.Tests.V1.PortfolioTests
{
    public class CreatePortfolioTests
    {
        private readonly LambdaEntryPoint _entryPoint;
        private const string PORTFOLIO_URI = "api/v1";
        public CreatePortfolioTests()
        {
            _entryPoint = new LambdaEntryPoint();
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
            var context = new TestLambdaContext();
            var requestStr = File.ReadAllText("../../../SampleRequests/RequestBase.json");
            var request = JsonSerializer.Deserialize<APIGatewayProxyRequest>(requestStr, new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
            
            request.HttpMethod = HttpMethod.Post.ToString();
            request.Path = PORTFOLIO_URI;
            request.PathParameters = new Dictionary<string,string>
            {
                {"proxy", PORTFOLIO_URI}
            };
            request.Body = JsonSerializer.Serialize(createPortfolioCommand);


            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(request, context);

            //Then
            httpResponse.StatusCode.Should().Equals(HttpStatusCode.Created);
        
            var newPortfolio = httpResponse.GetDeserializedResponseBody<Portfolio>();

            newPortfolio.Should().NotBeNull()
                .And.BeEquivalentTo(createPortfolioCommand, options => options
                    .ExcludingMissingMembers());
                
            // var dbPortfolio = await _db.Portfolios.FindAsync(newPortfolio.Id);
            // dbPortfolio.Should().NotBeNull()
            //     .And.BeEquivalentTo(createPortfolioCommand, options => options
            //         .Excluding(p => p.Id)
            //         .ExcludingTracking());
        }
    }
}