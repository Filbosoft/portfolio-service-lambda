using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using Api;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Integration.Tests.V1.PortfolioTests
{
    // public class DeletePortfolioTests : IClassFixture<CustomWebApplicationFactory<Startup>>, IDisposable
    // {
    //     private readonly LambdaEntryPoint _entryPoint;
    //     private readonly TestLambdaContext _context;
    //     private readonly APIGatewayProxyRequest _request;
    //     private readonly IDynamoDBContext _db;

    //     private const string PORTFOLIO_URI = "api/v1/portfolios";

    //     public DeletePortfolioTests(CustomWebApplicationFactory<Startup> factory)
    //     {
    //         _entryPoint = new LambdaEntryPoint();
    //         _context = new TestLambdaContext();

    //         _request = factory.CreateBaseRequest();
    //         _request.HttpMethod = HttpMethod.Delete.ToString();

    //         _db = factory.GetDbContext();

    //         Setup();
    //     }

    //     public void Dispose()
    //     {
    //         _db.Dispose();
    //     }

    //     /**
    //     * * Seed values
    //     * Id prefix: 1000
    //     **/

    //     private readonly Portfolio Portfolio1 = new Portfolio
    //     {
    //         Id = Guid.NewGuid().ToString(),
    //         Name = "SeedPortfolio#1",
    //         Currency = "DKK",
    //         Owner = "edb3f202-2712-4455-80d1-79f27f1d6bdd"
    //     };

    //     private async void Setup()
    //     {
    //         await _db.SaveAsync<Portfolio>(Portfolio1);
    //     }

    //     [Fact]
    //     public async void DeletePortfolio_withValidId_ShouldReturnSuccessfully()
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
    //     }

    //     [Fact]
    //     public async void DeletePortfolio_withInvalidId_ShouldReturnSuccessfully()
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
    //         httpResponse.StatusCode.Should().Be(StatusCodes.Status200OK);
    //     }
    // }
}