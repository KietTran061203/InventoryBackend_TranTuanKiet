using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backends.Models;

public class OrderItem
{
    //[BsonRepresentation(BsonType.ObjectId)]
    //public string ProductId { get; set; }
    //public string Name { get; set; } = null!;
    //[BsonRepresentation(BsonType.Decimal128)] public decimal Price { get; set; }
    //public int Quantity { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string ProductId { get; set; }   // chỉnh lại thành string

    public string? Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public class Order
{
    //[BsonId] public ObjectId Id { get; set; }
    //public ObjectId UserId { get; set; }
    //[BsonId]
    //[BsonRepresentation(BsonType.ObjectId)] // Tự convert giữa string <-> ObjectId
    //public string Id { get; set; }

    //[BsonRepresentation(BsonType.ObjectId)]
    //public string UserId { get; set; }
    //public List<OrderItem> Items { get; set; } = new();
    //public string Status { get; set; } = "pending"; // pending|completed|cancelled
    //[BsonRepresentation(BsonType.Decimal128)] public decimal Total { get; set; }
    //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; }   // phải là string

    public List<OrderItem> Items { get; set; } = new();
    public decimal Total { get; set; }
    public string Status { get; set; } = "pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
