using Domain.Constants;
using Domain.DTOs.MenuDTOs;
using Domain.Filters;
using Infrastructure.Permissions;
using Infrastructure.Services.MenuService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controller;

[ApiController]
[Route("api/[controller]")]
public class MenuController(IMenuService _menuService) : ControllerBase
{
    [HttpGet("Menus")]
    [PermissionAuthorize(Permissions.Menu.View)]
    public async Task<IActionResult> GetMenus([FromQuery] MenuFilter filter)
    {
        var res1 = await _menuService.GetMenusAsync(filter);
        return StatusCode(res1.StatusCode, res1);
    }

    [HttpGet("{MenuId:int}")]
    [PermissionAuthorize(Permissions.Menu.View)]
    public async Task<IActionResult> GetMenuById(int MenuId)
    {
        var res1 = await _menuService.GetMenuByIdAsync(MenuId);
        return StatusCode(res1.StatusCode, res1);
    }

    [HttpPost("create")]
    [PermissionAuthorize(Permissions.Menu.Create)]
    public async Task<IActionResult> CreateMenu([FromForm] CreateMenuDto createMenu)
    {
        var result = await _menuService.CreateMenuAsync(createMenu);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("update")]
    [PermissionAuthorize(Permissions.Menu.Edit)]
    public async Task<IActionResult> UpdateMenu([FromForm] UpdateMenuDto updateMenu)
    {
        var result = await _menuService.UpdateMenuAsync(updateMenu);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{MenuId:int}")]
    [PermissionAuthorize(Permissions.Menu.Delete)]
    public async Task<IActionResult> DeleteMenu(int MenuId)
    {
        var result = await _menuService.DeleteMenuAsync(MenuId);
        return StatusCode(result.StatusCode, result);
    }
}
