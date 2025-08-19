using MongoDB.Bson;

namespace Backend.DTOs;
// Tạo 1 Data Transfer Object (DTO) để tạo và cập nhật đơn hàng
public record OrderItemDto(string ProductId, int Quantity);
public record CreateOrderDto(List<OrderItemDto> Items);
public record UpdateOrderStatusDto(string Status); // pending/completed/cancelled
