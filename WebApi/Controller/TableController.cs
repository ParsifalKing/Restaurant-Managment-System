using Domain.Constants;
using Domain.DTOs.TableDTOs;
using Domain.Filters;
using Infrastructure.Permissions;
using Infrastructure.Services.TableService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controller;

[ApiController]
[Route("api/[controller]")]
public class TableController(ITableService _TableService) : ControllerBase
{
    [HttpGet("Tables")]
    [PermissionAuthorize(Permissions.Table.View)]
    public async Task<IActionResult> GetTables([FromQuery] TableFilter filter)
    {
        var res1 = await _TableService.GetTablesAsync(filter);
        return StatusCode(res1.StatusCode, res1);
    }

    [HttpGet("{TableId:int}")]
    [PermissionAuthorize(Permissions.Table.View)]
    public async Task<IActionResult> GetTableById(int TableId)
    {
        var res1 = await _TableService.GetTableByIdAsync(TableId);
        return StatusCode(res1.StatusCode, res1);
    }

    [HttpPost("create")]
    [PermissionAuthorize(Permissions.Table.Create)]
    public async Task<IActionResult> CreateTable([FromForm] CreateTableDto createTable)
    {
        var result = await _TableService.CreateTableAsync(createTable);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("update")]
    [PermissionAuthorize(Permissions.Table.Edit)]
    public async Task<IActionResult> UpdateTable([FromForm] UpdateTableDto updateTable)
    {
        var result = await _TableService.UpdateTableAsync(updateTable);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{TableId:int}")]
    [PermissionAuthorize(Permissions.Table.Delete)]
    public async Task<IActionResult> DeleteTable(int TableId)
    {
        var result = await _TableService.DeleteTableAsync(TableId);
        return StatusCode(result.StatusCode, result);
    }
}
