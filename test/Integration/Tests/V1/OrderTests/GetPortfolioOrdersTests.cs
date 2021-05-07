using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Api;
using Domain;
using Domain.Enums;
using Domain.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Integration.Utilities;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Integration.Tests.V1.OrderTests
{
    public class GetPortfolioOrdersTests : OrderTestsBase, IDisposable
    {
        protected readonly APIGatewayProxyRequest _request;

        public GetPortfolioOrdersTests(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
            _request = factory.CreateBaseRequest();

            Setup();
        }

        /**
        * * Seed values
        * Id prefix: 1007
        **/

        private const string ASSET1_ID = "ASSET#10070001";
        private const string ASSET2_ID = "ASSET#10070002";
        private const string ASSET3_ID = "ASSET#10070003";

        private static readonly Portfolio StockPortfolio1 = new Portfolio
        {
            Id = Guid.NewGuid().ToString(),
            Name = "SeedPortfolio#1",
            Currency = "DKK",
            Owner = 10070001,
            Orders = new List<Order>
            {
                new Order {Id = Guid.NewGuid().ToString(), Type = OrderType.Buy, Status = OrderStatus.Active, AssetId = ASSET1_ID, AssetType = AssetType.Stock, Quantity = 10, Price = 150.55M, Currency = "DKK", CreatedAt = DateTime.UtcNow.ToDbDateTime(), ExpiresAt = DateTime.UtcNow.AddDays(1).ToDbDateTime() },
                new Order {Id = Guid.NewGuid().ToString(), Type = OrderType.Sell, Status = OrderStatus.Active, AssetId = ASSET1_ID, AssetType = AssetType.Stock, Quantity = 10, Price = 180.6M, Currency = "DKK", CreatedAt = DateTime.UtcNow.ToDbDateTime(), ExpiresAt = DateTime.UtcNow.AddDays(1).ToDbDateTime() },
                new Order {Id = Guid.NewGuid().ToString(), Type = OrderType.Buy, Status = OrderStatus.Completed, AssetId = ASSET2_ID, AssetType = AssetType.Stock, Quantity = 50, Price = 7.4M, Currency = "DKK", CreatedAt = DateTime.UtcNow.AddDays(-10).ToDbDateTime(), ExpiresAt = DateTime.UtcNow.AddDays(-9).ToDbDateTime(), CompletedAt = DateTime.UtcNow.AddDays(-10).ToDbDateTime() },
                new Order {Id = Guid.NewGuid().ToString(), Type = OrderType.Sell, Status = OrderStatus.Active, AssetId = ASSET2_ID, AssetType = AssetType.Stock, Quantity = 40, Price = 15M, Currency = "DKK", CreatedAt = DateTime.UtcNow.AddDays(-5).ToDbDateTime(), ExpiresAt = DateTime.UtcNow.AddDays(-4).ToDbDateTime() },
                new Order {Id = Guid.NewGuid().ToString(), Type = OrderType.Buy, Status = OrderStatus.Completed, AssetId = ASSET2_ID, AssetType = AssetType.Stock, Quantity = 50, Price = 7.4M, Currency = "DKK", CreatedAt = DateTime.UtcNow.ToDbDateTime(), ExpiresAt = DateTime.UtcNow.ToDbDateTime(), CompletedAt = DateTime.UtcNow.ToDbDateTime() }
            }
        };

        private static readonly Portfolio FXPortfolio1 = new Portfolio
        {
            Id = Guid.NewGuid().ToString(),
            Name = "SeedPortfolio#1",
            Currency = "DKK",
            Owner = 10070001,
            Orders = new List<Order>
            {
                new Order {Id = Guid.NewGuid().ToString(), Type = OrderType.Buy, Status = OrderStatus.Cancelled, AssetId = ASSET3_ID, AssetType = AssetType.FX, Quantity = 1000, Price = 150.55M, Currency = "DKK", CreatedAt = DateTime.UtcNow.ToDbDateTime(), ExpiresAt = DateTime.UtcNow.AddDays(1).ToDbDateTime() },
                new Order {Id = Guid.NewGuid().ToString(), Type = OrderType.Buy, Status = OrderStatus.Active, AssetId = ASSET3_ID, AssetType = AssetType.FX, Quantity = 10000, Price = 150.55M, Currency = "DKK", CreatedAt = DateTime.UtcNow.ToDbDateTime(), ExpiresAt = DateTime.UtcNow.AddDays(1).ToDbDateTime() }
            }
        };

        /***
        * Batch operation:
        * Usually you would prefere using the DynamoDBContext.CreateBatchWrite() function
        * but as it only works for primitive types, meaning that nested objects won't be added
        * unless they are converted to a primitive type and back.
        * 
        * The reason it's using the raw DB is to implement the map structure with nested attributes.
        ***/
        private async void Setup()
        {
            var createStockPortfolioRequest = new PutRequest
            {
                Item = DynamoDBMapper.GetAttributeMap(StockPortfolio1)
            };
            var createFXPortfolioRequest = new PutRequest
            {
                Item = DynamoDBMapper.GetAttributeMap(FXPortfolio1)
            };

            var batchRequest = new Dictionary<string, List<WriteRequest>>
            {
                {"Portfolios", new List<WriteRequest>{
                    new WriteRequest {PutRequest = createStockPortfolioRequest},
                    new WriteRequest {PutRequest = createFXPortfolioRequest}
                }}
            };
            await _db.BatchWriteItemAsync(batchRequest);
        }

        [Fact]
        public async void GetPortfolioOrders_FromSeededPortfolio_ShouldReturnOkAndSeededOrders()
        {
            //Given
            var uri = $"{ORDER_URI}/{StockPortfolio1.Id}/orders";
            _request.Path = uri;
            _request.PathParameters = new Dictionary<string, string>
            {
                {"proxy", uri}
            };

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(_request, _context);

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status200OK);

            var portfolioOrders = httpResponse.GetDeserializedResponseBody<IEnumerable<Order>>();

            portfolioOrders.Should().NotBeEmpty()
                .And.BeEquivalentTo(StockPortfolio1.Orders);
        }

        [Fact]
        public async void GetPortfolioOrders_WithInvalidPortfolioId_ShouldReturnNotFound()
        {
            //Given
            var uri = $"{ORDER_URI}/{Guid.NewGuid()}/orders";

            _request.Path = uri;
            _request.PathParameters = new Dictionary<string, string>
            {
                {"proxy", uri}
            };

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(_request, _context);

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        public async void GetPortfolioOrders_WithOrderTypeQuery_ShouldReturnOkAndFilteredOrders()
        {
            //Given
            var uri = $"{ORDER_URI}/{StockPortfolio1.Id}/orders";
            var queryType = OrderType.Buy;

            _request.Path = uri;
            _request.QueryStringParameters = new Dictionary<string, string>
            {
                {nameof(Order.Type), queryType.ToString()}
            };
            _request.PathParameters = new Dictionary<string, string>
            {
                {"proxy", uri}
            };

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(_request, _context);

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status200OK);

            var portfolioOrders = httpResponse.GetDeserializedResponseBody<IEnumerable<Order>>();

            portfolioOrders.Should().NotBeEmpty()
                .And.OnlyContain(o => o.Type.Equals(queryType));
        }

        [Fact]
        public async void GetPortfolioOrders_WithOrderStatusQuery_ShouldReturnOkAndFilteredOrders()
        {
            //Given
            var uri = $"{ORDER_URI}/{StockPortfolio1.Id}/orders";
            var queryStatus = OrderStatus.Completed;

            _request.Path = uri;
            _request.QueryStringParameters = new Dictionary<string, string>
            {
                {nameof(Order.Status), queryStatus.ToString()}
            };
            _request.PathParameters = new Dictionary<string, string>
            {
                {"proxy", uri}
            };

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(_request, _context);

            //Then
            httpResponse.StatusCode.Should().Equals(StatusCodes.Status200OK);

            var portfolioOrders = httpResponse.GetDeserializedResponseBody<IEnumerable<Order>>();

            portfolioOrders.Should().NotBeEmpty()
                .And.OnlyContain(o => o.Status.Equals(queryStatus));
        }

        [Fact]
        public async void GetPortfolioOrders_WithAssetIdQuery_ShouldReturnOkAndFilteredOrders()
        {
            //Given
            var uri = $"{ORDER_URI}/{StockPortfolio1.Id}/orders";

            _request.Path = uri;
            _request.QueryStringParameters = new Dictionary<string, string>
            {
                {nameof(Order.AssetId), ASSET1_ID}
            };
            _request.PathParameters = new Dictionary<string, string>
            {
                {"proxy", uri}
            };

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(_request, _context);

            //Then
            httpResponse.StatusCode.Should().Equals(StatusCodes.Status200OK);

            var portfolioOrders = httpResponse.GetDeserializedResponseBody<IEnumerable<Order>>();

            portfolioOrders.Should().NotBeEmpty()
                .And.OnlyContain(o => o.AssetId.Equals(ASSET1_ID));
        }

        [Fact]
        public async void GetPortfolioOrders_WithAssetTypeQuery_ShouldReturnOkAndFilteredOrders()
        {
            //Given
            var uri = $"{ORDER_URI}/{StockPortfolio1.Id}/orders";
            var queryAssetType = AssetType.FX;

            _request.Path = uri;
            _request.QueryStringParameters = new Dictionary<string, string>
            {
                {nameof(Order.AssetType), queryAssetType.ToString()}
            };
            _request.PathParameters = new Dictionary<string, string>
            {
                {"proxy", uri}
            };

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(_request, _context);

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status200OK);

            var portfolioOrders = httpResponse.GetDeserializedResponseBody<IEnumerable<Order>>();

            portfolioOrders.Should().BeEmpty();
        }

        [Fact]
        public async void GetPortfolioOrders_WithCreatedFromDateQuery_ShouldReturnOkAndFilteredOrders()
        {
            //Given
            var uri = $"{ORDER_URI}/{StockPortfolio1.Id}/orders";
            var queryCreatedFromDate = DateTime.UtcNow.AddDays(-1);

            _request.Path = uri;
            _request.QueryStringParameters = new Dictionary<string, string>
            {
                {"createdFromDate", queryCreatedFromDate.ToString()}
            };
            _request.PathParameters = new Dictionary<string, string>
            {
                {"proxy", uri}
            };

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(_request, _context);

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status200OK);

            var portfolioOrders = httpResponse.GetDeserializedResponseBody<IEnumerable<Order>>();

            portfolioOrders.Should().NotBeEmpty()
                .And.OnlyContain(o => o.CreatedAt >= queryCreatedFromDate);
        }

        [Fact]
        public async void GetPortfolioOrders_WithCreatedToDateQuery_ShouldReturnOkAndFilteredOrders()
        {
            //Given
            var uri = $"{ORDER_URI}/{StockPortfolio1.Id}/orders";
            var queryCreatedToDate = DateTime.UtcNow.AddDays(-1);

            _request.Path = uri;
            _request.QueryStringParameters = new Dictionary<string, string>
            {
                {"createdToDate", queryCreatedToDate.ToString()}
            };
            _request.PathParameters = new Dictionary<string, string>
            {
                {"proxy", uri}
            };

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(_request, _context);

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status200OK);

            var portfolioOrders = httpResponse.GetDeserializedResponseBody<IEnumerable<Order>>();

            portfolioOrders.Should().NotBeEmpty()
                .And.OnlyContain(o => o.CreatedAt <= queryCreatedToDate);
        }

        [Fact]
        public async void GetPortfolioOrders_WithCompletedFromDateQuery_ShouldReturnOkAndFilteredOrders()
        {
            //Given
            var uri = $"{ORDER_URI}/{StockPortfolio1.Id}/orders";
            var queryCompletedFromDate = DateTime.UtcNow.AddDays(-1);

            _request.Path = uri;
            _request.QueryStringParameters = new Dictionary<string, string>
            {
                {"completedFromDate", queryCompletedFromDate.ToString()}
            };
            _request.PathParameters = new Dictionary<string, string>
            {
                {"proxy", uri}
            };

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(_request, _context);

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status200OK);

            var portfolioOrders = httpResponse.GetDeserializedResponseBody<IEnumerable<Order>>();

            portfolioOrders.Should().NotBeEmpty()
                .And.OnlyContain(o => o.CompletedAt >= queryCompletedFromDate);
        }

        [Fact]
        public async void GetPortfolioOrders_WithCompletedToDateQuery_ShouldReturnOkAndFilteredOrders()
        {
            //Given
            var uri = $"{ORDER_URI}/{StockPortfolio1.Id}/orders";
            var queryCompletedToDate = DateTime.UtcNow.AddDays(-1);

            _request.Path = uri;
            _request.QueryStringParameters = new Dictionary<string, string>
            {
                {"completedToDate", queryCompletedToDate.ToString()}
            };
            _request.PathParameters = new Dictionary<string, string>
            {
                {"proxy", uri}
            };

            //When
            var httpResponse = await _entryPoint.FunctionHandlerAsync(_request, _context);

            //Then
            httpResponse.StatusCode.Should().Be(StatusCodes.Status200OK);

            var portfolioOrders = httpResponse.GetDeserializedResponseBody<IEnumerable<Order>>();

            portfolioOrders.Should().NotBeEmpty()
                .And.OnlyContain(o => o.CompletedAt <= queryCompletedToDate);
        }
    }
}