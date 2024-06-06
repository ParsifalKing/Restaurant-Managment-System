using Domain.Constants;
using Domain.DTOs.DishDTOs;
using Domain.Filters;
using Infrastructure.Permissions;
using Infrastructure.Services.DishService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controller;

[ApiController]
[Route("api/[controller]")]
public class DishController(IDishService _dishService) : ControllerBase
{
    [HttpGet("Dishs")]
    [PermissionAuthorize(Permissions.Dish.View)]
    public async Task<IActionResult> GetDishes([FromQuery] DishFilter filter)
    {
        var res1 = await _dishService.GetDishesAsync(filter);
        return StatusCode(res1.StatusCode, res1);
    }

    [HttpGet("{DishId:int}")]
    [PermissionAuthorize(Permissions.Dish.View)]
    public async Task<IActionResult> GetDishById(int DishId)
    {
        var res1 = await _dishService.GetDishByIdAsync(DishId);
        return StatusCode(res1.StatusCode, res1);
    }

    [HttpPost("create")]
    [PermissionAuthorize(Permissions.Dish.Create)]
    public async Task<IActionResult> CreateDish([FromForm] CreateDishDto createDish)
    {
        var result = await _dishService.CreateDishAsync(createDish);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("update")]
    [PermissionAuthorize(Permissions.Dish.Edit)]
    public async Task<IActionResult> UpdateDish([FromForm] UpdateDishDto updateDish)
    {
        var result = await _dishService.UpdateDishAsync(updateDish);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{DishId:int}")]
    [PermissionAuthorize(Permissions.Dish.Delete)]
    public async Task<IActionResult> DeleteDish(int DishId)
    {
        var result = await _dishService.DeleteDishAsync(DishId);
        return StatusCode(result.StatusCode, result);
    }
}

