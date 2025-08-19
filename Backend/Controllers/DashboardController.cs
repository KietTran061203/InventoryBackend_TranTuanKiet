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
    private readonly MongoContext _db;
    public DashboardController(MongoContext db) => _db = db;

    [HttpGet("summary")]
    public async Task<IActionResult> Summary()
    {
        var uid = User.GetUserId();

        var totalProducts = await _db.Products.CountDocumentsAsync(p => p.UserId == uid);
        var lowStock = await _db.Products.CountDocumentsAsync(p => p.UserId == uid && p.Stock <= 5);

        var orders = await _db.Orders.Find(o => o.UserId == uid).ToListAsync();
        var totalOrders = orders.Count;
        var revenue = orders.Where(o => o.Status == "completed").Sum(o => o.Total);

        return Ok(new { totalProducts, lowStock, totalOrders, revenue });
    }
}
