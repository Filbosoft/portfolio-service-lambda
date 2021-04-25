using System;
using System.ComponentModel.DataAnnotations;
using Amazon.DynamoDBv2.DataModel;

namespace Domain.Models
{
    [DynamoDBTable("Portfolios")]
    public class Portfolio
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
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
    }
}