using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Amazon.DynamoDBv2.DataModel;
using Domain.PropertyConverters;

namespace Domain.Models
{
    [DynamoDBTable("Portfolios")]
    public class Portfolio : BaseModel
    {
        [DynamoDBHashKey]
        public new string Id { get; set; }
        [Required]
        [DynamoDBProperty]
        public string Name { get; set; }
        [Required]
        [Range(1,long.MaxValue)]
        [DynamoDBProperty]
        public long Owner { get; set; }
        [Required]
        [DynamoDBProperty]
        public string Currency { get; set; }
        [DynamoDBProperty(typeof(ListMapPropertyConverter<Order>))]
        public IEnumerable<Order> Orders { get; set; } = new List<Order>();      
    }
}