using System.Net;
using Domain.DTOs.PaymentDTOs;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.PaymentService;

public class PaymentService(ILogger<PaymentService> logger, DataContext context) : IPaymentService
{
    #region GetPaymentsAsync

    public async Task<PagedResponse<List<GetPaymentDto>>> GetPaymentsAsync(PaymentFilter filter)
    {
        try
        {
            logger.LogInformation("Starting method {GetPaymentsAsync} in time:{DateTime} ", "GetPaymentsAsync",
                DateTimeOffset.UtcNow);
            var Payments = context.Payments.AsQueryable();

            if (!string.IsNullOrEmpty(filter.PaymentStatus))
                Payments = Payments.Where(x => x.PaymentStatus.ToLower().Contains(filter.PaymentStatus.ToLower()));

            var response = await Payments.Select(x => new GetPaymentDto()
            {
                PaymentStatus = x.PaymentStatus,
                ReservationId = x.ReservationId,
                PaymentDate = x.PaymentDate,
                Sum = x.Sum,
                UserId = x.UserId,
                CreateAt = x.CreateAt,
                UpdateAt = x.UpdateAt,
                Id = x.Id,
            }).Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();

            var totalRecord = await Payments.CountAsync();

            logger.LogInformation("Finished method {GetPaymentsAsync} in time:{DateTime} ", "GetPaymentsAsync",
                DateTimeOffset.UtcNow);
            return new PagedResponse<List<GetPaymentDto>>(response, filter.PageNumber, filter.PageSize, totalRecord);
        }
        catch (Exception e)
        {
            logger.LogError("Exception {Exception}, time={DateTimeNow}", e.Message, DateTimeOffset.UtcNow);
            return new PagedResponse<List<GetPaymentDto>>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region GetPaymentByIdAsync

    public async Task<Response<GetPaymentDto>> GetPaymentByIdAsync(int PaymentId)
    {
        try
        {
            logger.LogInformation("Starting method {GetPaymentByIdAsync} in time:{DateTime} ", "GetPaymentByIdAsync",
                DateTimeOffset.UtcNow);

            var existing = await context.Payments.Select(x => new GetPaymentDto()
            {
                PaymentStatus = x.PaymentStatus,
                ReservationId = x.ReservationId,
                PaymentDate = x.PaymentDate,
                Sum = x.Sum,
                UserId = x.UserId,
                CreateAt = x.CreateAt,
                UpdateAt = x.UpdateAt,
                Id = x.Id,
            }).FirstOrDefaultAsync(x => x.Id == PaymentId);

            if (existing is null)
            {
                logger.LogWarning("Could not find Payment with Id:{Id},time:{DateTimeNow}", PaymentId, DateTimeOffset.UtcNow);
                return new Response<GetPaymentDto>(HttpStatusCode.BadRequest, $"Not found Payment by id:{PaymentId}");
            }


            logger.LogInformation("Finished method {GetPaymentByIdAsync} in time:{DateTime} ", "GetPaymentByIdAsync",
                DateTimeOffset.UtcNow);
            return new Response<GetPaymentDto>(existing);
        }
        catch (Exception e)
        {
            logger.LogError("Exception {Exception}, time={DateTimeNow}", e.Message, DateTimeOffset.UtcNow);
            return new Response<GetPaymentDto>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region CreatePaymentAsync

    public async Task<Response<string>> CreatePaymentAsync(CreatePaymentDto createPayment)
    {
        try
        {
            logger.LogInformation("Starting method {CreatePaymentAsync} in time:{DateTime} ", "CreatePaymentAsync",
                DateTimeOffset.UtcNow);

            var newPayment = new Payment()
            {
                PaymentDate = createPayment.PaymentDate,
                ReservationId = createPayment.ReservationId,
                Sum = createPayment.Sum,
                UserId = createPayment.UserId,
                CreateAt = DateTimeOffset.UtcNow,
                UpdateAt = DateTimeOffset.UtcNow,
                PaymentStatus = "Completed",
            };

            await context.Payments.AddAsync(newPayment);
            await context.SaveChangesAsync();

            logger.LogInformation("Finished method {CreatePaymentAsync} in time:{DateTime} ", "CreatePaymentAsync",
                DateTimeOffset.UtcNow);
            return new Response<string>($"Successfully created Payment by Id:{newPayment.Id}");
        }
        catch (Exception e)
        {
            logger.LogError("Exception {Exception}, time={DateTimeNow}", e.Message, DateTimeOffset.UtcNow);
            return new Response<string>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region UpdatePaymentAsync

    public async Task<Response<string>> UpdatePaymentAsync(UpdatePaymentDto updatePayment)
    {
        try
        {
            logger.LogInformation("Starting method {UpdatePaymentAsync} in time:{DateTime} ", "UpdatePaymentAsync",
                DateTimeOffset.UtcNow);

            var existing = await context.Payments.FirstOrDefaultAsync(x => x.Id == updatePayment.Id);
            if (existing == null)
            {
                logger.LogWarning("Payment not found by id:{Id},time:{Time}", updatePayment.Id, DateTimeOffset.UtcNow);
                new Response<string>(HttpStatusCode.BadRequest, $"Not found Booking by id:{updatePayment.Id}");
            }

            existing!.Sum = updatePayment.Sum;
            existing.PaymentDate = DateTime.UtcNow;
            existing.ReservationId = updatePayment.ReservationId;
            existing.UserId = updatePayment.UserId;
            existing.UpdateAt = DateTimeOffset.UtcNow;
            if (updatePayment.PaymentStatus == Domain.Enums.PaymentStatus.Completed) existing.PaymentStatus = "Completed";
            if (updatePayment.PaymentStatus == Domain.Enums.PaymentStatus.Cancelled) existing.PaymentStatus = "Cancelled";

            logger.LogInformation("Finished method {UpdatePaymentAsync} in time:{DateTime} ", "UpdatePaymentAsync",
                DateTimeOffset.UtcNow);
            return new Response<string>($"Successfully updated Payment by id:{updatePayment.Id}");
        }
        catch (Exception e)
        {
            logger.LogError("Exception {Exception}, time={DateTimeNow}", e.Message, DateTimeOffset.UtcNow);
            return new Response<string>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region DeletePaymentAsync

    public async Task<Response<bool>> DeletePaymentAsync(int PaymentId)
    {
        try
        {
            logger.LogInformation("Starting method {DeletePaymentAsync} in time:{DateTime} ", "DeletePaymentAsync",
                DateTimeOffset.UtcNow);

            var Payment = await context.Payments.Where(x => x.Id == PaymentId).ExecuteDeleteAsync();

            logger.LogInformation("Finished method {DeletePaymentAsync} in time:{DateTime} ", "DeletePaymentAsync",
                DateTimeOffset.UtcNow);
            return Payment == 0
                ? new Response<bool>(HttpStatusCode.BadRequest, $"Payment not found by id:{PaymentId}")
                : new Response<bool>(true);
        }
        catch (Exception e)
        {
            logger.LogError("Exception {Exception}, time={DateTimeNow}", e.Message, DateTimeOffset.UtcNow);
            return new Response<bool>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion
}
