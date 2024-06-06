using Domain.Constants;
using Domain.DTOs.PaymentDTOs;
using Domain.Filters;
using Infrastructure.Permissions;
using Infrastructure.Services.PaymentService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controller;

[ApiController]
[Route("api/[controller]")]
public class PaymentController(IPaymentService _PaymentService) : ControllerBase
{
    [HttpGet("Payments")]
    [PermissionAuthorize(Permissions.Payment.View)]
    public async Task<IActionResult> GetPayments([FromQuery] PaymentFilter filter)
    {
        var res1 = await _PaymentService.GetPaymentsAsync(filter);
        return StatusCode(res1.StatusCode, res1);
    }

    [HttpGet("{PaymentId:int}")]
    [PermissionAuthorize(Permissions.Payment.View)]
    public async Task<IActionResult> GetPaymentById(int PaymentId)
    {
        var res1 = await _PaymentService.GetPaymentByIdAsync(PaymentId);
        return StatusCode(res1.StatusCode, res1);
    }

    [HttpPost("create")]
    [PermissionAuthorize(Permissions.Payment.Create)]
    public async Task<IActionResult> CreatePayment([FromForm] CreatePaymentDto createPayment)
    {
        var result = await _PaymentService.CreatePaymentAsync(createPayment);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("update")]
    [PermissionAuthorize(Permissions.Payment.Edit)]
    public async Task<IActionResult> UpdatePayment([FromForm] UpdatePaymentDto updatePayment)
    {
        var result = await _PaymentService.UpdatePaymentAsync(updatePayment);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{PaymentId:int}")]
    [PermissionAuthorize(Permissions.Payment.Delete)]
    public async Task<IActionResult> DeletePayment(int PaymentId)
    {
        var result = await _PaymentService.DeletePaymentAsync(PaymentId);
        return StatusCode(result.StatusCode, result);
    }
}
