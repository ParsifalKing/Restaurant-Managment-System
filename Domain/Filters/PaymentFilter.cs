namespace Domain.Filters;

public class PaymentFilter : PaginationFilter
{
    public string? PaymentStatus { get; set; }
}
