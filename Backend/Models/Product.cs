using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Backend.Models;

public class Product
{
    //[BsonId] public ObjectId Id { get; set; }
    //public ObjectId UserId { get; set; }
    //[BsonId]
    //[BsonRepresentation(BsonType.ObjectId)]
    //public string Id { get; set; }
    //[BsonId]
    //[BsonRepresentation(BsonType.ObjectId)] // Tự chuyển đổi giữa string qua ObjectId
    //public string Id { get; set; }

    //[BsonRepresentation(BsonType.ObjectId)]
    //public string UserId { get; set; }

    //public string Name { get; set; } = null!;
    //public string? Description { get; set; }
    //[BsonRepresentation(BsonType.Decimal128)] public decimal Price { get; set; }
    //public int Stock { get; set; }
    //public string? ImageUrl { get; set; }
    //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    //Đánh dấu khóa chính cho MongoDB
    [BsonId]
    //Lưu dưới dạng String nhưng sẽ được chuyển đổi thành ObjectId
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    //Tương tự lưu UserId dưới dạng String nhưng chuyển đổi thành ObjectId

    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; }   // chỉnh là string

    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
