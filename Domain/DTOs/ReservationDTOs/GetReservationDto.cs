using Domain.Enums;

namespace Domain.DTOs.ReservationDTOs;

public class GetReservationDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int TableId { get; set; }
    public DateTime ReservationDate { get; set; }
    public TimeSpan ReservationTime { get; set; }
    public required string ReservationStatus { get; set; }
    public DateTimeOffset CreateAt { get; set; }
    public DateTimeOffset UpdateAt { get; set; }
}
