namespace Backend.DTOs;

public record ProductCreateUpdateDto(string Name, string? Description, decimal Price, int Stock, string? ImageUrl);
