using Domain.Constants;
using Domain.DTOs.ReservationDTOs;
using Domain.Filters;
using Infrastructure.Permissions;
using Infrastructure.Services.ReservationService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controller;

[ApiController]
[Route("api/[controller]")]
public class ReservationController(IReservationService _ReservationService) : ControllerBase
{
    [HttpGet("Reservations")]
    [PermissionAuthorize(Permissions.Reservation.View)]
    public async Task<IActionResult> GetReservations([FromQuery] ReservationFilter filter)
    {
        var res1 = await _ReservationService.GetReservationsAsync(filter);
        return StatusCode(res1.StatusCode, res1);
    }

    [HttpGet("{ReservationId:int}")]
    [PermissionAuthorize(Permissions.Reservation.View)]
    public async Task<IActionResult> GetReservationById(int ReservationId)
    {
        var res1 = await _ReservationService.GetReservationByIdAsync(ReservationId);
        return StatusCode(res1.StatusCode, res1);
    }

    [HttpPost("create")]
    [PermissionAuthorize(Permissions.Reservation.Create)]
    public async Task<IActionResult> CreateReservation([FromForm] CreateReservationDto createReservation)
    {
        var result = await _ReservationService.CreateReservationAsync(createReservation);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("update")]
    [PermissionAuthorize(Permissions.Reservation.Edit)]
    public async Task<IActionResult> UpdateReservation([FromForm] UpdateReservationDto updateReservation)
    {
        var result = await _ReservationService.UpdateReservationAsync(updateReservation);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{ReservationId:int}")]
    [PermissionAuthorize(Permissions.Reservation.Delete)]
    public async Task<IActionResult> DeleteReservation(int ReservationId)
    {
        var result = await _ReservationService.DeleteReservationAsync(ReservationId);
        return StatusCode(result.StatusCode, result);
    }
}
