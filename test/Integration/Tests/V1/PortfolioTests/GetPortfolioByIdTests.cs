using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using Api;
using FluentAssertions;
using Integration.Utilities;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Integration.Tests.V1.PortfolioTests
{
    // public class GetPortfolioByIdTests : IClassFixture<CustomWebApplicationFactory<Startup>>, IDisposable
    // {
    //     private readonly LambdaEntryPoint _entryPoint;
    //     private readonly TestLambdaContext _context;
    //     private readonly APIGatewayProxyRequest _request;
    //     private readonly IDynamoDBContext _db;

    //     private const string PORTFOLIO_URI = "api/v1/portfolios";

    //     public GetPortfolioByIdTests(CustomWebApplicationFactory<Startup> factory)
    //     {
    //         _entryPoint = new LambdaEntryPoint();
    //         _context = new TestLambdaContext();
    //         _request = factory.CreateBaseRequest();
    //         _db = factory.GetDbContext();

    //         Setup();
    //     }

    //     public void Dispose()
    //     {
    //         _db.Dispose();
    //     }

    //     /**
    //     * * Seed values
    //     * Id prefix: 1001
    //     **/

    //     private readonly Portfolio Portfolio1 = new Portfolio
    //     {
    //         Id = Guid.NewGuid().ToString(),
    //         Name = "SeedPortfolio#1",
    //         Currency = "DKK",
    //         Owner = "2a42b10c-21ba-4e9e-968b-c723342c5ceb"
    //     };

    //     private async void Setup()
    //     {
    //         await _db.SaveAsync<Portfolio>(Portfolio1);
    //     }

    //     [Fact]
    //     public async void GetPortfolioById_WithExistingId_ShouldReturnSeededPortfolio()
    //     {
    //         //Given
    //         var uri = $"{PORTFOLIO_URI}/{Portfolio1.Id}";

    //         _request.Path = uri;
    //         _request.PathParameters = new Dictionary<string, string>
    //         {
    //             {"proxy", uri}
    //         };

    //         //When
    //         var httpResponse = await _entryPoint.FunctionHandlerAsync(_request, _context);

    //         //Then
    //         httpResponse.StatusCode.Should().Be(StatusCodes.Status200OK);
    //         var portfolio = httpResponse.GetDeserializedResponseBody<Portfolio>();

    //         portfolio.Should().NotBeNull()
    //             .And.BeEquivalentTo(Portfolio1, options => options.ExcludingMissingMembers());

    //         // portfolio.Orders.Should().NotBeEmpty()
    //         //     .And.Equals(PortfolioOrders.Where(p => p.PortfolioId.Equals(Portfolio1.Id)));
    //     }

    //     [Fact]
    //     public async void GetPortfolioById_WithNonExistingId_ShouldReturnNotFound()
    //     {
    //         //Given
    //         var uri = $"{PORTFOLIO_URI}/{Guid.NewGuid()}";
            
    //         _request.Path = uri;
    //         _request.PathParameters = new Dictionary<string, string>
    //         {
    //             {"proxy", uri}
    //         };

    //         //When
    //         var httpResponse = await _entryPoint.FunctionHandlerAsync(_request, _context);

    //         //Then
    //         httpResponse.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    //     }
    // }
}