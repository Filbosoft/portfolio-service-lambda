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
using FluentAssertions;
using FluentAssertions.Execution;
using Integration.Utilities;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Integration.Tests.V1.PortfolioTests
{
    public class CreatePortfolioTests : IClassFixture<CustomWebApplicationFactory<Startup>>, IDisposable
    {
        private readonly IDynamoDBContext _db;
        
        private const string PORTFOLIO_URI = "api/v1/portfolios";
        public CreatePortfolioTests(CustomWebApplicationFactory<Startup> factory)
        {

            _request = factory.CreateBaseRequest();
            _request.HttpMethod = HttpMethod.Post.ToString();
            _request.Path = PORTFOLIO_URI;
            _request.PathParameters = new Dictionary<string, string>
            {
                {"proxy", PORTFOLIO_URI}
            };

            _db = factory.GetDbContext();
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        [Fact]
        public async void CreatePortfolio_withValidValues_ShouldReturnOkAndTheNewPortfolio()
        {
            //Given
            var createPortfolioCommand = new CreatePortfolioCommand
            {
                Name = Guid.NewGuid().ToString(),
                Currency = "DKK",
                Owner = "c90d634e-a9f9-460f-9f23-e595ab9f668a"
            };

            _request.Body = JsonSerializer.Serialize(createPortfolioCommand);

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(_request, _context);

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status201Created);

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

        [Theory]
        [MemberData(nameof(CreatePortfolioCommandsWithMissingValues))]
        public async void CreatePortfolio_withMissingValues_ShouldReturnBadRequest(CreatePortfolioCommand createPortfolioCommand)
        {
            //Given
            _request.Body = JsonSerializer.Serialize(createPortfolioCommand);

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(_request, _context);

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        public static IEnumerable<object[]> CreatePortfolioCommandsWithMissingValues
        {
            get
            {
                yield return new Object[] { new CreatePortfolioCommand { Currency = "DKK", Owner = "3932e81a-0417-4ee6-bc30-0b27d7f5e169" } };
                yield return new Object[] { new CreatePortfolioCommand { Name = "PortfolioName", Owner = "3932e81a-0417-4ee6-bc30-0b27d7f5e169" } };
                yield return new Object[] { new CreatePortfolioCommand { Name = "PortfolioName", Currency = "DKK" } };
            }
        }
    }
}