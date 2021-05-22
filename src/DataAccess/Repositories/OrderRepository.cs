using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;

namespace DataAccess.Repositories
{
    // public class OrderRepository : IOrderRepository
    // {
    //     private readonly IDynamoDBContext _context;
    //     private readonly IAmazonDynamoDB _db;
    //     private const string PORTFOLIO_TABLE_NAME = "Portfolios";

    //     public OrderRepository(IDynamoDBContext context, IAmazonDynamoDB db)
    //     {
    //         _context = context;
    //         _db = db;
    //     }

    //     public async Task<Order> PlaceOrderAsync(string portfolioId, Order order)
    //     {
    //         order.Id = Guid.NewGuid().ToString();
    //         order.CreatedAt = DateTime.UtcNow;

    //         var orderMap = DynamoDBMapper.GetAttributeMap(order);
    //         var expressionValues = new Dictionary<string, AttributeValue>
    //         {
    //             {":order", new AttributeValue{M = orderMap}},
    //             {":portfolioId", new AttributeValue{S = portfolioId}}
    //         };
    //         UpdateItemRequest request = new UpdateItemRequest
    //         {
    //             TableName = PORTFOLIO_TABLE_NAME,
    //             Key = new Dictionary<string, AttributeValue>
    //                 {
    //                     {nameof(Portfolio.Id), new AttributeValue{S = portfolioId}}
    //                 },
    //             UpdateExpression = "SET Orders.#orderId = :order",
    //             ConditionExpression = "Id = :portfolioId",
    //             ExpressionAttributeNames = new Dictionary<string, string>
    //                 {
    //                     {"#orderId", order.Id}
    //                 },
    //             ExpressionAttributeValues = expressionValues,
    //             ReturnValues = "UPDATED_NEW"
    //         };
    //         UpdateItemResponse response;

    //         try
    //         {
    //             response = await _db.UpdateItemAsync(request);
    //         }
    //         catch (ConditionalCheckFailedException)
    //         {
    //             throw new KeyNotFoundException();
    //         }
    //         catch (AmazonDynamoDBException)
    //         {
    //             request.UpdateExpression = "SET Orders = :order";
    //             request.ExpressionAttributeNames = null;
    //             expressionValues.Remove(":order");
    //             expressionValues.Add(":order", new AttributeValue { M = new Dictionary<string, AttributeValue> { { order.Id, new AttributeValue { M = orderMap } } } });

    //             response = await _db.UpdateItemAsync(request);
    //         }

    //         return order;
    //     }

    //     public async Task<Order> UpdateOrderAsync(string portfolioId, Order order)
    //     {
    //         var orderMap = DynamoDBMapper.GetAttributeMap(order);

    //         UpdateItemRequest request = new UpdateItemRequest
    //         {
    //             TableName = PORTFOLIO_TABLE_NAME,
    //             Key = new Dictionary<string, AttributeValue>
    //                 {
    //                     {nameof(Portfolio.Id), new AttributeValue{S = portfolioId}}
    //                 },
    //             UpdateExpression = "SET Orders.#orderId = :order",
    //             ExpressionAttributeNames = new Dictionary<string, string>
    //                 {
    //                     {"#orderId", order.Id}
    //                 },
    //             ExpressionAttributeValues = new Dictionary<string, AttributeValue>
    //             {
    //                 {":order", new AttributeValue{M = orderMap}},
    //             },
    //             ReturnValues = "UPDATED_NEW"
    //         };
    //         UpdateItemResponse response = await _db.UpdateItemAsync(request);

    //         return order;
    //     }
    // }
}