using Domain.Enums;

namespace Domain.Entities;

public class Table : BaseEntity
{
    public string TableNumber { get; set; } = null!;
    public int PlaceCount { get; set; }
    public string TableStatus { get; set; } = null!;

    public List<Reservation>? Reservations { get; set; }
}
