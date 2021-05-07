using System;
using Domain.Enums;

namespace Domain.Models
{
    public class Order : BaseModel
    {
        public OrderType Type { get; set; }
        public string AssetId { get; set; }
        public AssetType AssetType { get; set; }
        public int Quantity { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}