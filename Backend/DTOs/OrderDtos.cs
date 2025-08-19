using MongoDB.Bson;

namespace Backend.DTOs;

public record OrderItemDto(string ProductId, int Quantity);
public record CreateOrderDto(List<OrderItemDto> Items);
public record UpdateOrderStatusDto(string Status); // pending/completed/cancelled
