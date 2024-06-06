using Microsoft.AspNetCore.Http;

namespace Domain.Entities;

public class Dish : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public string? PathPhoto { get; set; }
    public int MenuId { get; set; }

    public Menu? Menu { get; set; }
}
