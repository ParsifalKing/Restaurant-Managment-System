using Domain.Enums;

namespace Domain.DTOs.TableDTOs;

public class UpdateTableDto
{
    public int Id { get; set; }
    public required string TableNumber { get; set; }
    public int PlaceCount { get; set; }
    public TableStatus TableStatus { get; set; }
}
