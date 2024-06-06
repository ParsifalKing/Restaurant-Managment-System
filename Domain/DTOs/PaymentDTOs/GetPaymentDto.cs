namespace Domain.DTOs.PaymentDTOs;

public class GetPaymentDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ReservationId { get; set; }
    public decimal Sum { get; set; }
    public DateTime PaymentDate { get; set; }
    public required string PaymentStatus { get; set; }
    public DateTimeOffset CreateAt { get; set; }
    public DateTimeOffset UpdateAt { get; set; }
}
