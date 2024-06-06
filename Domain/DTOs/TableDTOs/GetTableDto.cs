using Domain.Enums;

namespace Domain.DTOs.TableDTOs;

public class GetTableDto
{
    public int Id { get; set; }
    public required string TableNumber { get; set; }
    public int PlaceCount { get; set; }
    public required string TableStatus { get; set; }
    public DateTimeOffset CreateAt { get; set; }
    public DateTimeOffset UpdateAt { get; set; }
}
