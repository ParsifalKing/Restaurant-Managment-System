using Domain.Enums;

namespace Domain.DTOs.ReservationDTOs;

public class CreateReservationDto
{
    public int UserId { get; set; }
    public int TableId { get; set; }
    public DateTime ReservationDate { get; set; }
    public TimeSpan ReservationTime { get; set; }
    public ReservationStatus ReservationStatus { get; set; }
}
