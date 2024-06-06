using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Payment : BaseEntity
{
    public int UserId { get; set; }
    public int ReservationId { get; set; }
    public decimal Sum { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentStatus { get; set; } = null!;

    public User? User { get; set; }
    public Reservation? Reservation { get; set; }
}
