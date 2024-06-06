using Domain.Enums;

namespace Domain.Filters;

public class ReservationFilter : PaginationFilter
{
    public ReservationStatus? ReservationStatus { get; set; }
}
