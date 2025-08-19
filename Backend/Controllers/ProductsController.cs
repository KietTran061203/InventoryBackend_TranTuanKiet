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
    public ProductsController(MongoContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var uid = User.GetUserId();

        var items = await _db.Products
            .Find(p => p.UserId == uid)
            .SortByDescending(p => p.CreatedAt)
            .ToListAsync();
        Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(items));
        return Ok(items);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var uid = User.GetUserId();

        var filter = Builders<Product>.Filter.Eq(p => p.Id, id) &
                     Builders<Product>.Filter.Eq(p => p.UserId, uid);

        var product = await _db.Products.Find(filter).FirstOrDefaultAsync();

        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateUpdateDto dto)
    {
        var uid = User.GetUserId();

        var product = new Product
        {
            Id = ObjectId.GenerateNewId().ToString(),
            UserId = uid,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            CreatedAt = DateTime.UtcNow
        };

        await _db.Products.InsertOneAsync(product);

        return Ok(product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, ProductCreateUpdateDto dto)
    {
        var uid = User.GetUserId();

        var filter = Builders<Product>.Filter.Eq(p => p.Id, id) &
                     Builders<Product>.Filter.Eq(p => p.UserId, uid);

        var update = Builders<Product>.Update
            .Set(p => p.Name, dto.Name)
            .Set(p => p.Description, dto.Description)
            .Set(p => p.Price, dto.Price)
            .Set(p => p.Stock, dto.Stock);

        var res = await _db.Products.UpdateOneAsync(filter, update);

        return res.MatchedCount == 0 ? NotFound() : Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var uid = User.GetUserId();

        var filter = Builders<Product>.Filter.Eq(p => p.Id, id) &
                     Builders<Product>.Filter.Eq(p => p.UserId, uid);

        var res = await _db.Products.DeleteOneAsync(filter);

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
