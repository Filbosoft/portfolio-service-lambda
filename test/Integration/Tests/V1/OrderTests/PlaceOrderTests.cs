using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Api;
using Business.Commands.OrderCommands;
using Domain.Enums;
using Domain.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Integration.Utilities;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Integration.Tests.V1.OrderTests
{
    public class PlaceOrderTests : OrderTestsBase
    {
        protected readonly APIGatewayProxyRequest _request;

        private readonly string PORTFOLIO1_ORDER_URI = $"{ORDER_URI}/{Portfolio1.Id}/orders";

        public PlaceOrderTests(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
            _request = factory.CreateBaseRequest();
            _request.HttpMethod = HttpMethod.Post.ToString();
            _request.Path = PORTFOLIO1_ORDER_URI;
            _request.PathParameters = new Dictionary<string, string>
            {
                {"proxy", PORTFOLIO1_ORDER_URI}
            };

            Setup();
        }

        /**
        * * Seed values
        * Id prefix: 1006
        **/

        private static readonly Portfolio Portfolio1 = new Portfolio
        {
            Id = Guid.NewGuid().ToString(),
            Name = "SeedPortfolio#1",
            Currency = "DKK",
            Owner = 10060001
        };

        private async void Setup()
        {
            await _db.SaveAsync<Portfolio>(Portfolio1);
        }

        [Fact]
        public async void PlaceOrder_withValidValues_ShouldReturnOkAndTheNewOrder()
        {
            //Given
            var placeOrderCommand = new PlaceOrderCommand
            {
                Type = OrderType.Buy,
                AssetId = "Asset#1",
                AssetType = AssetType.Stock,
                Quantity = 10,
                Price = 150,
                Currency = "DKK",
                ExpiresAt = Convert.ToDateTime(DateTime.UtcNow.AddDays(2).ToString())
            };

            _request.Body = JsonSerializer.Serialize(placeOrderCommand);

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(_request, _context);

            //Then
            httpResponse.StatusCode.Should().Equals(HttpStatusCode.Created);

            var newOrder = httpResponse.GetDeserializedResponseBody<Order>();

            using (new AssertionScope())
            {
                newOrder.Should().NotBeNull();
                newOrder.Should().BeEquivalentTo(placeOrderCommand, options => options
                    .ExcludingMissingMembers());
                newOrder.Id.Should().NotBeNullOrEmpty();
            }

            var dbPortfolio = await _db.LoadAsync<Portfolio>(Portfolio1.Id);
            dbPortfolio.Orders.Should().NotBeEmpty()
                .And.ContainEquivalentOf(placeOrderCommand, options => options
                    .ExcludingMissingMembers());
        }

        [Fact]
        public async void PlaceOrder_withInvalidPortfolioId_ShouldReturnBadRequest()
        {
            //Given
            var placeOrderCommand = new PlaceOrderCommand
            {
                Type = OrderType.Buy,
                AssetId = "Asset#1",
                AssetType = AssetType.Stock,
                Quantity = 10,
                Price = 150,
                Currency = "DKK",
                ExpiresAt = Convert.ToDateTime(DateTime.UtcNow.AddDays(2).ToString())
            };
            var uri = $"{ORDER_URI}/{Guid.NewGuid()}/orders";

            _request.Path = uri;
            _request.PathParameters = new Dictionary<string, string>
            {
                {"proxy", uri}
            };
            _request.Body = JsonSerializer.Serialize(placeOrderCommand);

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(_request, _context);

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Theory]
        [MemberData(nameof(PlaceOrderCommandsWithMissingValues))]
        public async void PlaceOrder_withMissingValues_ShouldReturnBadRequest(PlaceOrderCommand placeOrderCommand)
        {
            //Given
            _request.Body = JsonSerializer.Serialize(placeOrderCommand);

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(_request, _context);

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        public static IEnumerable<object[]> PlaceOrderCommandsWithMissingValues
        {
            get
            {
                yield return new Object[] { new PlaceOrderCommand { AssetId = "Asset#1", Quantity = 10, Price = 150.5M, Currency = "DKK", ExpiresAt = DateTime.UtcNow.AddMinutes(1)} };
                yield return new Object[] { new PlaceOrderCommand { Type = OrderType.Buy, Quantity = 10, Price = 150.5M, Currency = "DKK", ExpiresAt = DateTime.UtcNow.AddMinutes(1)} };
                yield return new Object[] { new PlaceOrderCommand { Type = OrderType.Buy, AssetId = "Asset#1", Price = 150.5M, Currency = "DKK", ExpiresAt = DateTime.UtcNow.AddMinutes(1)} };
                yield return new Object[] { new PlaceOrderCommand { Type = OrderType.Buy, AssetId = "Asset#1", Quantity = 10, Currency = "DKK", ExpiresAt = DateTime.UtcNow.AddMinutes(1)} };
                yield return new Object[] { new PlaceOrderCommand { Type = OrderType.Buy, AssetId = "Asset#1", Quantity = 10, Price = 150.5M, ExpiresAt = DateTime.UtcNow.AddMinutes(1)} };
                yield return new Object[] { new PlaceOrderCommand { Type = OrderType.Buy, AssetId = "Asset#1", Quantity = 10, Price = 150.5M, Currency = "DKK"} };
            }
        }
    }
}