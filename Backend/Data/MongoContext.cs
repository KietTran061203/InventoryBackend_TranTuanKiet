using Backend.Models;
using Backends.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Backend.Data;
// Tạo một lớp MongoContext để quản lý kết nối MongoDB và các collection
public class MongoOptions
{
    // Chuỗi kết nối MongoDB và tên cơ sở dữ liệu
    public string ConnectionString { get; set; } = null!;
    public string Database { get; set; } = null!;
}

public class MongoContext
{
    // Các collection trong cơ sở dữ liệu MongoDB
    public readonly IMongoDatabase Db;
    public readonly IMongoCollection<User> Users;
    public readonly IMongoCollection<Product> Products;
    public readonly IMongoCollection<Order> Orders;
    public readonly IMongoClient Client;

    // Constructor nhận vào các tùy chọn cấu hình MongoDB
    public MongoContext(IOptions<MongoOptions> opt)
    {
        Client = new MongoClient(opt.Value.ConnectionString); // Khởi tạo client MongoDB với chuỗi kết nối
        Db = Client.GetDatabase(opt.Value.Database); // Lấy cơ sở dữ liệu theo tên đã cấu hình
        Users = Db.GetCollection<User>("users"); // Lấy collection "users"
        Products = Db.GetCollection<Product>("products"); // Lấy collection "products"
        Orders = Db.GetCollection<Order>("orders"); // Lấy collection "orders"
    }
}
