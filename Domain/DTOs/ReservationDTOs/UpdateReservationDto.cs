using Domain.Enums;

namespace Domain.DTOs.ReservationDTOs;

public class UpdateReservationDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int TableId { get; set; }
    public DateTime ReservationDate { get; set; }
    public TimeSpan ReservationTime { get; set; }
    public ReservationStatus ReservationStatus { get; set; }
}
