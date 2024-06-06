using Microsoft.AspNetCore.Http;

namespace Domain.DTOs.DishDTOs;

public class UpdateDishDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
    public IFormFile? PathPhoto { get; set; }
    public int MenuId { get; set; }
}
