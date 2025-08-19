using Backend.Data;
using Backend.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InventoryOrders.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    // Khai báo MongoContext để truy cập vào cơ sở dữ liệu
    private readonly MongoContext _db;
    public DashboardController(MongoContext db) => _db = db;

    [HttpGet("summary")]
    public async Task<IActionResult> Summary()
    {
        var uid = User.GetUserId(); // Lấy ID người dùng từ token JWT
        // Đếm số lượng sản phẩm, sản phẩm tồn kho, đơn hàng và doanh thu
        var totalProducts = await _db.Products.CountDocumentsAsync(p => p.UserId == uid);
        var lowStock = await _db.Products.CountDocumentsAsync(p => p.UserId == uid && p.Stock <= 5);
        //var totalProducts = await _db.Products.CountDocumentsAsync(p => p.UserId == id);
        //var lowStock = await _db.Products.CountDocumentsAsync(p => p.UserId == id && p.Stock <= 5);
        var orders = await _db.Orders.Find(o => o.UserId == uid).ToListAsync();
        var totalOrders = orders.Count;
        var revenue = orders.Where(o => o.Status == "completed").Sum(o => o.Total);

        return Ok(new { totalProducts, lowStock, totalOrders, revenue });
    }
}
