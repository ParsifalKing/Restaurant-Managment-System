using System.Net;
using Domain.DTOs.ReservationDTOs;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.ReservationService;

public class ReservationService(ILogger<ReservationService> logger, DataContext context) : IReservationService
{
    #region GetReservationsAsync

    public async Task<PagedResponse<List<GetReservationDto>>> GetReservationsAsync(ReservationFilter filter)
    {
        try
        {
            logger.LogInformation("Starting method {GetReservationsAsync} in time:{DateTime} ", "GetReservationsAsync",
                DateTimeOffset.UtcNow);
            var Reservations = context.Reservations.AsQueryable();

            if (filter.ReservationStatus != null)
            {
                if (filter.ReservationStatus == Domain.Enums.ReservationStatus.Confirmed) Reservations = Reservations.Where(x => x.ReservationStatus == "Confirmed");
                if (filter.ReservationStatus == Domain.Enums.ReservationStatus.Cancelled) Reservations = Reservations.Where(x => x.ReservationStatus == "Cancelled");
            }

            var response = await Reservations.Select(x => new GetReservationDto()
            {
                ReservationStatus = x.ReservationStatus,
                ReservationDate = x.ReservationDate,
                ReservationTime = x.ReservationTime,
                TableId = x.TableId,
                UserId = x.UserId,
                CreateAt = x.CreateAt,
                UpdateAt = x.UpdateAt,
                Id = x.Id,
            }).Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();

            var totalRecord = await Reservations.CountAsync();

            logger.LogInformation("Finished method {GetReservationsAsync} in time:{DateTime} ", "GetReservationsAsync",
                DateTimeOffset.UtcNow);
            return new PagedResponse<List<GetReservationDto>>(response, filter.PageNumber, filter.PageSize, totalRecord);
        }
        catch (Exception e)
        {
            logger.LogError("Exception {Exception}, time={DateTimeNow}", e.Message, DateTimeOffset.UtcNow);
            return new PagedResponse<List<GetReservationDto>>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region GetReservationByIdAsync

    public async Task<Response<GetReservationDto>> GetReservationByIdAsync(int ReservationId)
    {
        try
        {
            logger.LogInformation("Starting method {GetReservationByIdAsync} in time:{DateTime} ", "GetReservationByIdAsync",
                DateTimeOffset.UtcNow);

            var existing = await context.Reservations.Select(x => new GetReservationDto()
            {
                ReservationStatus = x.ReservationStatus,
                ReservationDate = x.ReservationDate,
                ReservationTime = x.ReservationTime,
                TableId = x.TableId,
                UserId = x.UserId,
                CreateAt = x.CreateAt,
                UpdateAt = x.UpdateAt,
                Id = x.Id,
            }).FirstOrDefaultAsync(x => x.Id == ReservationId);

            if (existing is null)
            {
                logger.LogWarning("Could not find Reservation with Id:{Id},time:{DateTimeNow}", ReservationId, DateTimeOffset.UtcNow);
                return new Response<GetReservationDto>(HttpStatusCode.BadRequest, $"Not found Reservation by id:{ReservationId}");
            }


            logger.LogInformation("Finished method {GetReservationByIdAsync} in time:{DateTime} ", "GetReservationByIdAsync",
                DateTimeOffset.UtcNow);
            return new Response<GetReservationDto>(existing);
        }
        catch (Exception e)
        {
            logger.LogError("Exception {Exception}, time={DateTimeNow}", e.Message, DateTimeOffset.UtcNow);
            return new Response<GetReservationDto>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region CreateReservationAsync

    public async Task<Response<string>> CreateReservationAsync(CreateReservationDto createReservation)
    {
        try
        {
            logger.LogInformation("Starting method {CreateReservationAsync} in time:{DateTime} ", "CreateReservationAsync",
                DateTimeOffset.UtcNow);
            var existingReservation = await context.Reservations.AnyAsync(x => x.TableId == createReservation.TableId && x.ReservationDate.Hour == createReservation.ReservationDate.Hour);
            if (existingReservation)
            {
                logger.LogWarning("Reservation already exists by id:{RoomId},time:{Time}", createReservation.TableId,
                    DateTimeOffset.UtcNow);
                return new Response<string>(HttpStatusCode.BadRequest,
                    $"Already exists Reservation by name:{createReservation.TableId}");
            }

            var newReservation = new Reservation()
            {
                ReservationDate = createReservation.ReservationDate,
                ReservationTime = createReservation.ReservationTime,
                TableId = createReservation.TableId,
                UserId = createReservation.UserId,
                CreateAt = DateTimeOffset.UtcNow,
                UpdateAt = DateTimeOffset.UtcNow,
            };
            if (createReservation.ReservationStatus == Domain.Enums.ReservationStatus.Confirmed) newReservation.ReservationStatus = "Confirmed";
            if (createReservation.ReservationStatus == Domain.Enums.ReservationStatus.Cancelled) newReservation.ReservationStatus = "Cancelled";

            var reservationTable = await context.Tables.FirstOrDefaultAsync(x => x.Id == newReservation.TableId);
            if (reservationTable != null) reservationTable.TableStatus = "Reserved";

            await context.Reservations.AddAsync(newReservation);
            await context.SaveChangesAsync();

            logger.LogInformation("Finished method {CreateReservationAsync} in time:{DateTime} ", "CreateReservationAsync",
                DateTimeOffset.UtcNow);
            return new Response<string>($"Successfully created Reservation by Id:{newReservation.Id}");
        }
        catch (Exception e)
        {
            logger.LogError("Exception {Exception}, time={DateTimeNow}", e.Message, DateTimeOffset.UtcNow);
            return new Response<string>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region UpdateReservationAsync

    public async Task<Response<string>> UpdateReservationAsync(UpdateReservationDto updateReservation)
    {
        try
        {
            logger.LogInformation("Starting method {UpdateReservationAsync} in time:{DateTime} ", "UpdateReservationAsync",
                DateTimeOffset.UtcNow);

            var existing = await context.Reservations.FirstOrDefaultAsync(x => x.Id == updateReservation.Id);
            if (existing == null)
            {
                logger.LogWarning("Reservation not found by id:{Id},time:{Time}", updateReservation.Id, DateTimeOffset.UtcNow);
                new Response<string>(HttpStatusCode.BadRequest, $"Not found Reservation by id:{updateReservation.Id}");
            }

            existing!.ReservationDate = updateReservation.ReservationDate;
            existing.ReservationTime = updateReservation.ReservationTime;
            existing.UserId = updateReservation.UserId;
            existing.TableId = updateReservation.TableId;
            existing.UpdateAt = DateTimeOffset.UtcNow;
            if (updateReservation.ReservationStatus == Domain.Enums.ReservationStatus.Confirmed) existing.ReservationStatus = "Confirmed";
            if (updateReservation.ReservationStatus == Domain.Enums.ReservationStatus.Cancelled) existing.ReservationStatus = "Cancelled";

            var reservationTable = await context.Tables.FirstOrDefaultAsync(x => x.Id == updateReservation.TableId);
            if (reservationTable != null)
            {
                if (existing.ReservationStatus == "Cancelled") reservationTable.TableStatus = "Reserved";
                if (existing.ReservationStatus == "Confirmed") reservationTable.TableStatus = "Busy";
            }

            await context.SaveChangesAsync();
            logger.LogInformation("Finished method {UpdateReservationAsync} in time:{DateTime} ", "UpdateReservationAsync",
                DateTimeOffset.UtcNow);
            return new Response<string>($"Successfully updated Reservation by id:{updateReservation.Id}");
        }
        catch (Exception e)
        {
            logger.LogError("Exception {Exception}, time={DateTimeNow}", e.Message, DateTimeOffset.UtcNow);
            return new Response<string>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region DeleteReservationAsync

    public async Task<Response<bool>> DeleteReservationAsync(int ReservationId)
    {
        try
        {
            logger.LogInformation("Starting method {DeleteReservationAsync} in time:{DateTime} ", "DeleteReservationAsync",
                DateTimeOffset.UtcNow);

            var Reservation = await context.Reservations.Where(x => x.Id == ReservationId).ExecuteDeleteAsync();

            logger.LogInformation("Finished method {DeleteReservationAsync} in time:{DateTime} ", "DeleteReservationAsync",
                DateTimeOffset.UtcNow);
            return Reservation == 0
                ? new Response<bool>(HttpStatusCode.BadRequest, $"Reservation not found by id:{ReservationId}")
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
