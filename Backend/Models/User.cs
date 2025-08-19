using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models;

public class User
{
    [BsonId] public ObjectId Id { get; set; }
    [BsonElement("username")] public string Username { get; set; } = null!;
    [BsonElement("email")] public string Email { get; set; } = null!;
    [BsonElement("passwordHash")] public string PasswordHash { get; set; } = null!;
}
