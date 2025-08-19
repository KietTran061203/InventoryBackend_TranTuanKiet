namespace Backend.DTOs;
// Tạo 1 Data Transfer Object (DTO) để tạo và cập nhật sản phẩm
public record ProductCreateUpdateDto(string Name, string? Description, decimal Price, int Stock, string? ImageUrl);
