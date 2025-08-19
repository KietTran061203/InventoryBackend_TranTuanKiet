using Backend.Models;
using Backends.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Backend.Data;

public class MongoOptions
{
    public string ConnectionString { get; set; } = null!;
    public string Database { get; set; } = null!;
}

public class MongoContext
{
    public readonly IMongoDatabase Db;
    public readonly IMongoCollection<User> Users;
    public readonly IMongoCollection<Product> Products;
    public readonly IMongoCollection<Order> Orders;
    public readonly IMongoClient Client;

    public MongoContext(IOptions<MongoOptions> opt)
    {
        Client = new MongoClient(opt.Value.ConnectionString);
        Db = Client.GetDatabase(opt.Value.Database);
        Users = Db.GetCollection<User>("users");
        Products = Db.GetCollection<Product>("products");
        Orders = Db.GetCollection<Order>("orders");
    }
}
