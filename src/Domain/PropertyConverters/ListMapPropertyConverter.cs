using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Domain.Models;

namespace Domain.PropertyConverters
{
    public class ListMapPropertyConverter<T> : IPropertyConverter
        where T : new()
    {
        public object FromEntry(DynamoDBEntry ordersEntry)
        {
            Document ordersEntryDocument = ordersEntry as Document;
            var ordersEntryMap = ordersEntryDocument.ToAttributeMap();
            var orderEntryMaps = ordersEntryMap
                .Select(x => x.Value.M)
                .ToList();
            
            var orders = new List<T>();

            foreach (var orderEntryMap in orderEntryMaps)
            {
                var newEntity = DynamoDBMapper.MapAttributeMapToEntity<T>(orderEntryMap);
                orders.Add(newEntity);
            }

            return orders;
        }

        public DynamoDBEntry ToEntry(object value)
        {
            List<Order> orders = value as List<Order>;
            if (orders == null) throw new ArgumentOutOfRangeException();

            var orderEntries = new Dictionary<string, AttributeValue>();

            foreach (var order in orders)
            {
                var orderEntry = DynamoDBMapper.GetAttributeMap(order);
                orderEntries.Add(order.Id, new AttributeValue { M = orderEntry});
            }

            DynamoDBEntry entry = new Primitive
            {
                Value = new AttributeValue{M = orderEntries} 
            };
            return entry;
        }
    }
}