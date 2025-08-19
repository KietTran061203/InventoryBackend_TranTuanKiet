using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Backend.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend.Controllers;

[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly MongoContext _db;
    // Khai báo biến _db để truy cập vào MongoDB
    public ProductsController(MongoContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var uid = User.GetUserId(); // Lấy ID người dùng từ token JWT Bearer Token

        var items = await _db.Products
            .Find(p => p.UserId == uid) // Lọc sản phẩm theo ID người dùng
            .SortByDescending(p => p.CreatedAt) // Sắp xếp theo ngày tạo giảm dần
            .ToListAsync(); // Chuyển đổi kết quả thành danh sách
        Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(items));
        // Trả về danh sách sản phẩm từ người dùng hiện tại
        return Ok(items); 
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var uid = User.GetUserId(); // Lấy ID người dùng từ token JWT Bearer Token

        var filter = Builders<Product>.Filter.Eq(p => p.Id, id) &
                     Builders<Product>.Filter.Eq(p => p.UserId, uid); // Tạo bộ lọc để tìm sản phẩm theo ID và ID người dùng
        // Tìm sản phẩm trong cơ sở dữ liệu MongoDB
        var product = await _db.Products.Find(filter).FirstOrDefaultAsync();
        //Nếu không tìm thấy sản phẩm, trả về trang 404 NotFound ngược lai sẽ trả về sản phẩm
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateUpdateDto dto)
    {
        var uid = User.GetUserId(); // Lấy ID người dùng từ token JWT Bearer Token

        var product = new Product
        {
            Id = ObjectId.GenerateNewId().ToString(), // Tạo ID mới cho sản phẩm
            UserId = uid, // Gán ID người dùng vào sản phẩm
            Name = dto.Name, // Gán tên sản phẩm từ DTO
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            CreatedAt = DateTime.UtcNow
        };
        // Sau khi tạo sản phẩm, thêm sản phẩm vào cơ sở dữ liệu MongoDB
        await _db.Products.InsertOneAsync(product);
        // Trả về sản phẩm vừa tạo
        return Ok(product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, ProductCreateUpdateDto dto)
    {
        var uid = User.GetUserId(); // Lấy ID người dùng từ token JWT Bearer Token

        var filter = Builders<Product>.Filter.Eq(p => p.Id, id) &
                     Builders<Product>.Filter.Eq(p => p.UserId, uid);

        var update = Builders<Product>.Update
            .Set(p => p.Name, dto.Name)
            .Set(p => p.Description, dto.Description)
            .Set(p => p.Price, dto.Price)
            .Set(p => p.Stock, dto.Stock); // Tạo đối tượng cập nhật để thay đổi thông tin sản phẩm
        // Cập nhật sản phẩm trong cơ sở dữ liệu MongoDB
        var res = await _db.Products.UpdateOneAsync(filter, update);
        // Nếu không tìm thấy sản phẩm, trả về trang 404 NotFound, ngược lại trả về trang OK
        return res.MatchedCount == 0 ? NotFound() : Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var uid = User.GetUserId(); // Lấy ID người dùng từ token JWT Bearer Token

        var filter = Builders<Product>.Filter.Eq(p => p.Id, id) &
                     Builders<Product>.Filter.Eq(p => p.UserId, uid); // Tạo bộ lọc để tìm sản phẩm theo ID và ID người dùng
        // Xóa sản phẩm khỏi cơ sở dữ liệu MongoDB
        var res = await _db.Products.DeleteOneAsync(filter);
        // Nếu không tìm thấy sản phẩm, trả về trang 404 NotFound, ngược lại trả về trang OK
        return res.DeletedCount == 0 ? NotFound() : Ok();
    }
    //[HttpGet]
    //public async Task<IActionResult> GetAll()
    //{
    //    var uid = User.GetUserId();
    //    var items = await _db.Products.Find(p => p.UserId == uid)
    //        .SortByDescending(p => p.CreatedAt).ToListAsync();
    //    return Ok(items);
    //}

    //[HttpGet("{id}")]
    //public async Task<IActionResult> GetById(string id)
    //{
    //    var uid = User.GetUserId();
    //    var pid = ObjectId.Parse(id);
    //    var p = await _db.Products.Find(x => x.Id == pid && x.UserId == uid).FirstOrDefaultAsync();
    //    return p is null ? NotFound() : Ok(p);
    //}

    //[HttpPost]
    //public async Task<IActionResult> Create(ProductCreateUpdateDto dto)
    //{
    //    var uid = User.GetUserId();
    //    var p = new Product
    //    {
    //        Id = ObjectId.GenerateNewId(),
    //        UserId = uid,
    //        Name = dto.Name.Trim(),
    //        Description = dto.Description,
    //        Price = dto.Price,
    //        Stock = dto.Stock,
    //        ImageUrl = dto.ImageUrl
    //    };
    //    await _db.Products.InsertOneAsync(p);
    //    return Ok(p);
    //}

    //[HttpPut("{id}")]
    //public async Task<IActionResult> Update(string id, ProductCreateUpdateDto dto)
    //{
    //    var uid = User.GetUserId();
    //    var pid = ObjectId.Parse(id);
    //    var upd = Builders<Product>.Update
    //        .Set(x => x.Name, dto.Name.Trim())
    //        .Set(x => x.Description, dto.Description)
    //        .Set(x => x.Price, dto.Price)
    //        .Set(x => x.Stock, dto.Stock)
    //        .Set(x => x.ImageUrl, dto.ImageUrl);

    //    var res = await _db.Products.UpdateOneAsync(
    //        x => x.Id == pid && x.UserId == uid, upd);

    //    return res.MatchedCount == 0 ? NotFound() : Ok();
    //}

    //[HttpDelete("{id}")]
    //public async Task<IActionResult> Delete(string id)
    //{
    //    var uid = User.GetUserId();
    //    var pid = ObjectId.Parse(id);
    //    var res = await _db.Products.DeleteOneAsync(x => x.Id == pid && x.UserId == uid);
    //    return res.DeletedCount == 0 ? NotFound() : Ok();
    //}
}
