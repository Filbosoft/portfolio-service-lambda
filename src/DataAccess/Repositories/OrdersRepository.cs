using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Domain;
using Domain.Models;
using Domain.Repositories;

namespace DataAccess.Repositories
{
    public class OrdersRepository : IOrderRepository
    {
        private readonly IDynamoDBContext _context;
        private readonly IAmazonDynamoDB _db;
        private const string PORTFOLIO_TABLE_NAME = "Portfolios";



        public OrdersRepository(IDynamoDBContext context, IAmazonDynamoDB db)
        {
            _context = context;
            _db = db;
        }

        public async Task<Portfolio> GetPortfolioAsync(string id)
        {
            var portfolio = await _context.LoadAsync<Portfolio>(id);
            return portfolio;
        }

        public async Task<IEnumerable<Portfolio>> GetPortfoliosAsync(IEnumerable<ScanCondition> conditions = default)
        {
            var portfolios = await _context.ScanAsync<Portfolio>(conditions).GetRemainingAsync();

            return portfolios;
        }

        public async Task<Portfolio> UpdatePortfolioAsync(Portfolio portfolio)
        {
            await _context.SaveAsync(portfolio);
            return portfolio;
        }

        public async Task<Order> PlaceOrderAsync(string portfolioId, Order order)
        {
            order.Id = Guid.NewGuid().ToString();
            var orderMap = DynamoDBMapper.GetAttributeMap(order);
            UpdateItemRequest request = new UpdateItemRequest
            {
                TableName = PORTFOLIO_TABLE_NAME,
                Key = new Dictionary<string, AttributeValue>
                    {
                        {nameof(Portfolio.Id), new AttributeValue{S = portfolioId}}
                    },
                UpdateExpression = "SET Orders.#orderId = :order",
                ExpressionAttributeNames = new Dictionary<string, string>
                    {
                        {"#orderId", order.Id}
                    },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        {":order", new AttributeValue{M = orderMap}}
                    },
                ReturnValues = "UPDATED_NEW"
            };
            UpdateItemResponse response;

            try
            {
                response = await _db.UpdateItemAsync(request);
            }
            catch (AmazonDynamoDBException)
            {
                request.UpdateExpression = "SET Orders = :order";
                request.ExpressionAttributeNames = null;
                request.ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":order", new AttributeValue{M = new Dictionary<string, AttributeValue>{{order.Id, new AttributeValue{M = orderMap}}}}}
                };

                response = await _db.UpdateItemAsync(request);
            }

            return order;
        }

        public Task<Order> UpdateOrderAsync(string portfolioId, Order order)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Order>> GetPortfolioOrdersAsync(string portfolioId, IEnumerable<ScanCondition> conditions)
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetPortfolioOrderAsync(string portfolioId, string orderId)
        {
            throw new NotImplementedException();
        }
    }
}