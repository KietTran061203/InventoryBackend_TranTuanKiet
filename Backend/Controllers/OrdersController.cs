using Backend.Data;
using Backend.DTOs;
using Backend.Extensions;
using Backend.Models;
using Backends.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    // Khai báo MongoContext để truy cập vào cơ sở dữ liệu MongoDB
    private readonly MongoContext _db;
    // Constructor để khởi tạo MongoContext
    public OrdersController(MongoContext db) => _db = db;

    //[HttpGet]
    //public async Task<IActionResult> List()
    //{
    //    var uid = User.GetUserId();
    //    var items = await _db.Orders.Find(o => o.UserId == uid)
    //        .SortByDescending(o => o.CreatedAt).ToListAsync();
    //    return Ok(items);
    //}
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var uid = User.GetUserId(); // Lấy ID người dùng từ token xác thực
        // Tạo bộ lọc để tìm các đơn hàng của người dùng hiện tại
        var filter = Builders<Order>.Filter.Eq(o => o.UserId, uid);
        //var items = await _db.Orders.Find(o => o.UserId == uid)
        //    .SortByDescending(o => o.CreatedAt)
        //    .ToListAsync();

        //return Ok(items);
        var items = await _db.Orders
          .Find(filter)
          .SortByDescending(o => o.CreatedAt) // Sắp xếp theo ngày tạo giảm dần
          .ToListAsync(); // Lấy danh sách các đơn hàng
        // Trả về danh sách đơn hàng cho người dùng
        return Ok(items);
    }
    //[HttpPost]
    //public async Task<IActionResult> Create(CreateOrderDto dto)
    //{
    //    var uid = User.GetUserId();
    //    if (dto.Items is null || dto.Items.Count == 0)
    //        return BadRequest("No items");

    //    using var session = await _db.Client.StartSessionAsync();
    //    session.StartTransaction();

    //    try
    //    {
    //        var orderItems = new List<OrderItem>();
    //        decimal total = 0;

    //        foreach (var i in dto.Items)
    //        {
    //            //Chuyển string qua ObjectId 
    //            if (!ObjectId.TryParse(i.ProductId, out var pid))
    //            {
    //                await session.AbortTransactionAsync();
    //                return BadRequest($"Invalid productId: {i.ProductId}");
    //            }

    //            //var product = await _db.Products
    //            //    .Find(p => p.Id == pid && p.UserId == uid)
    //            //    .FirstOrDefaultAsync();
    //            var filter = Builders<Product>.Filter.Eq(p => p.Id, pid) &
    //         Builders<Product>.Filter.Eq(p => p.UserId, uid);

    //            var product = await _db.Products.Find(filter).FirstOrDefaultAsync();

    //            if (product is null)
    //            {
    //                await session.AbortTransactionAsync();
    //                return NotFound($"Product {i.ProductId} not found");
    //            }
    //            if (product.Stock < i.Quantity)
    //            {
    //                await session.AbortTransactionAsync();
    //                return BadRequest($"Not enough stock for {product.Name}");
    //            }

    //            //Giảm stock
    //            //var filter = Builders<Product>.Filter.Where(
    //            //    p => p.Id == pid && p.UserId == uid && p.Stock >= i.Quantity
    //            //);
    //            var filter = Builders<Product>.Filter.Eq(p => p.Id, pid) &
    //         Builders<Product>.Filter.Eq(p => p.UserId, uid) &
    //         Builders<Product>.Filter.Gte(p => p.Stock, i.Quantity);
    //            var update = Builders<Product>.Update.Inc(p => p.Stock, -i.Quantity);
    //            var updRes = await _db.Products.UpdateOneAsync(session, filter, update);
    //            if (updRes.ModifiedCount == 0)
    //            {
    //                await session.AbortTransactionAsync();
    //                return BadRequest($"Stock changed, please retry for {product.Name}");
    //            }

    //            orderItems.Add(new OrderItem
    //            {
    //                ProductId = product.Id,
    //                Name = product.Name,
    //                Price = product.Price,
    //                Quantity = i.Quantity
    //            });
    //            total += product.Price * i.Quantity;
    //        }

    //        var order = new Order
    //        {
    //            Id = ObjectId.GenerateNewId(),
    //            UserId = uid,
    //            Items = orderItems,
    //            Total = total,
    //            Status = "pending",
    //            CreatedAt = DateTime.UtcNow
    //        };

    //        await _db.Orders.InsertOneAsync(session, order);
    //        await session.CommitTransactionAsync();
    //        return Ok(order);
    //    }
    //    catch
    //    {
    //        await session.AbortTransactionAsync();
    //        throw;
    //    }
    //}
    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderDto dto)
    {
        var uid = User.GetUserId(); // Lấy ID người dùng từ token xác thực
        // Kiểm tra xem danh sách sản phẩm có rỗng hay không
        if (dto.Items is null || dto.Items.Count == 0)
            return BadRequest("Không có sản phẩm");
        // Bắt đầu một phiên truy cập MongoDB
        using var session = await _db.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var orderItems = new List<OrderItem>(); // Danh sách các sản phẩm trong đơn hàng
            decimal total = 0; // Tổng giá trị của đơn hàng

            foreach (var i in dto.Items) // Duyệt qua từng sản phẩm trong danh sách
            {
                var pid = i.ProductId;

                // Tìm sản phẩm của user
                //var filterProduct = Builders<Product>.Filter.Eq(p => p.Id, pid) &
                //                    Builders<Product>.Filter.Eq(p => p.UserId, uid);
                var productFilter = Builders<Product>.Filter.And(
                  Builders<Product>.Filter.Eq(p => p.Id, pid),
                  Builders<Product>.Filter.Eq(p => p.UserId, uid)
              );

                //var product = await _db.Products.Find(filterProduct).FirstOrDefaultAsync();
                //if (product is null)
                //{
                //    await session.AbortTransactionAsync();
                //    return NotFound($"Product {pid} not found");
                //}

                //if (product.Stock < i.Quantity)
                //{
                //    await session.AbortTransactionAsync();
                //    return BadRequest($"Not enough stock for {product.Name}");
                //}
                // Tìm sản phẩm trong cơ sở dữ liệu
                var product = await _db.Products.Find(productFilter).FirstOrDefaultAsync();
                // Kiểm tra xem sản phẩm có tồn tại hay không
                if (product is null)
                {
                    await session.AbortTransactionAsync();
                    return NotFound($"Sản phẩm {pid} không tìm thấy");
                }
                // Kiểm tra xem số lượng sản phẩm có đủ hay không
                if (product.Stock < i.Quantity)
                {
                    await session.AbortTransactionAsync();
                    return BadRequest($"Không đủ hàng từ sản phẩm {product.Name}");
                }

                // giảm sản lượng
                //var filter = filterProduct & Builders<Product>.Filter.Gte(p => p.Stock, i.Quantity);
                //var update = Builders<Product>.Update.Inc(p => p.Stock, -i.Quantity);
                // Tạo bộ lọc để giảm số lượng sản phẩm
                var decStockFilter = Builders<Product>.Filter.And(
                   Builders<Product>.Filter.Eq(p => p.Id, pid),
                   Builders<Product>.Filter.Eq(p => p.UserId, uid),
                   Builders<Product>.Filter.Gte(p => p.Stock, i.Quantity)
               );
                var decUpdate = Builders<Product>.Update.Inc(p => p.Stock, -i.Quantity);

                //var updRes = await _db.Products.UpdateOneAsync(session, filter, update);
                //if (updRes.ModifiedCount == 0)
                //{
                //    await session.AbortTransactionAsync();
                //    return BadRequest($"Stock changed, please retry for {product.Name}");
                //}
                // Cập nhật số lượng sản phẩm trong cơ sở dữ liệu
                var updRes = await _db.Products.UpdateOneAsync(session, decStockFilter, decUpdate);
                if (updRes.ModifiedCount == 0)
                {
                    await session.AbortTransactionAsync();
                    return BadRequest($"Số lượng đã thay đổi {product.Name}");
                }
                // Thêm sản phẩm vào danh sách đơn hàng
                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = i.Quantity
                });
                // Tính tổng giá trị của đơn hàng
                total += product.Price * i.Quantity;
            }
            // Tạo một đơn hàng mới
            var order = new Order
            {
                // Tạo một Id mới cho đơn hàng
                Id = ObjectId.GenerateNewId().ToString(), // string Id
                UserId = uid,
                Items = orderItems,
                Total = total,
                Status = "pending", // Trạng thái ban đầu là "pending"
                CreatedAt = DateTime.UtcNow
            };
            // Thêm đơn hàng vào cơ sở dữ liệu
            await _db.Orders.InsertOneAsync(session, order);
            // Cam kết hoàn thành transaction
            await session.CommitTransactionAsync();
            // Trả về đơn hàng đã tạo
            return Ok(order);
        }
        catch
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }

    //[HttpPatch("{id}/status")]
    //public async Task<IActionResult> UpdateStatus(string id, UpdateOrderStatusDto dto)
    //{
    //    var uid = User.GetUserId();
    //    var oid = ObjectId.Parse(id);

    //    if (dto.Status is not ("pending" or "completed" or "cancelled"))
    //        return BadRequest("Invalid status");

    //    var res = await _db.Orders.UpdateOneAsync(
    //        o => o.Id == oid && o.UserId == uid,
    //        Builders<Order>.Update.Set(o => o.Status, dto.Status));

    //    return res.MatchedCount == 0 ? NotFound() : Ok();
    //}
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(string id, UpdateOrderStatusDto dto)
    {
        var uid = User.GetUserId(); // Lấy ID người dùng từ token xác thực
        // Kiểm tra xem ID đơn hàng có hợp lệ hay không
        if (dto.Status is not ("pending" or "completed" or "cancelled"))
            return BadRequest("Invalid status");

        //var filter = Builders<Order>.Filter.Eq(o => o.Id, id) &
        //             Builders<Order>.Filter.Eq(o => o.UserId, uid);
        // Tạo bộ lọc để tìm đơn hàng theo ID và UserId
        var filter = Builders<Order>.Filter.And(
           Builders<Order>.Filter.Eq(o => o.Id, id),
           Builders<Order>.Filter.Eq(o => o.UserId, uid)
       );
        // Cập nhật trạng thái đơn hàng
        var update = Builders<Order>.Update.Set(o => o.Status, dto.Status);
        // Thực hiện cập nhật đơn hàng trong cơ sở dữ liệu
        var res = await _db.Orders.UpdateOneAsync(filter, update);
        // Trả về kết quả cập nhật
        return res.MatchedCount == 0 ? NotFound() : Ok();
    }
}
