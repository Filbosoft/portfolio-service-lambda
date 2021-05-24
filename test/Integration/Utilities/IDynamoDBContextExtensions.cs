using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Conditus.DynamoDBMapper.Mappers;

namespace Integration.Utilities
{
    public static class IAmazonDynamoDBExtensions
    {
        public async static Task<T> LoadByLocalIndexAsync<T>(this IAmazonDynamoDB dynamoDB, string hashKey, string rangeKeyName, string rangeKey, string index)
            where T : new()
        {
            string tableName = GetTableName<T>(),
                hashKeyName = GetHashKeyName<T>();

            var query = new QueryRequest
            {
                TableName = GetTableName<T>(),
                Select = "ALL_ATTRIBUTES",
                IndexName = index,
                KeyConditionExpression = $"{hashKeyName} = :v_hash_key AND {rangeKeyName} = :v_range_key",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":v_hash_key", new AttributeValue{S = hashKey}},
                    {":v_range_key", new AttributeValue{S = rangeKey}}
                }
            };

            var response = await dynamoDB.QueryAsync(query);
            var item = response.Items
                .FirstOrDefault();
            var mappedItem = item.GetEntity<T>();
            
            return mappedItem;
        }

        private static string GetTableName<T>()
        {
            var type = typeof(T);
            var dynamoDBTableAttribute = type.GetCustomAttributes(typeof(DynamoDBTableAttribute), true)
                .FirstOrDefault() as DynamoDBTableAttribute;
            
            if (dynamoDBTableAttribute == null)
                return null;
            
            return dynamoDBTableAttribute.TableName;
        }

        private static string GetHashKeyName<T>()
        {
            var type = typeof(T);
            var hashProperty = type.GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(DynamoDBHashKeyAttribute), false) != null)
                .FirstOrDefault();
            
            if (hashProperty == null)
                throw new ArgumentOutOfRangeException("T","No hashkey defined on type");
            
            return hashProperty.Name;
        }
    }
}