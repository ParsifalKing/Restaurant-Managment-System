using Domain.Enums;

namespace Domain.Entities;

public class Reservation : BaseEntity
{
    public int UserId { get; set; }
    public int TableId { get; set; }
    public int PaymentId { get; set; }
    public DateTime ReservationDate { get; set; }
    public TimeSpan ReservationTime { get; set; }
    public string ReservationStatus { get; set; } = null!;

    public User? User { get; set; }
    public Table? Table { get; set; }
    public Payment? Payment { get; set; }
}