using System.Net;
using Domain.DTOs.TableDTOs;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Data;
using Infrastructure.Services.FileService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.TableService;

public class TableService(ILogger<TableService> logger, DataContext context) : ITableService
{
    #region GetTablesAsync

    public async Task<PagedResponse<List<GetTableDto>>> GetTablesAsync(TableFilter filter)
    {
        try
        {
            logger.LogInformation("Starting method {GetTablesAsync} in time:{DateTime} ", "GetTablesAsync",
                DateTimeOffset.UtcNow);

            var Tables = context.Tables.AsQueryable();
            if (filter.PlaceCount != null)
                Tables = Tables.Where(x => x.PlaceCount == filter.PlaceCount);

            var response = await Tables.Select(x => new GetTableDto()
            {
                TableNumber = x.TableNumber,
                TableStatus = x.TableStatus,
                Id = x.Id,
                PlaceCount = x.PlaceCount,
                CreateAt = x.CreateAt,
                UpdateAt = x.UpdateAt
            }).Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();

            var totalRecord = await Tables.CountAsync();

            logger.LogInformation("Finished method {GetTablesAsync} in time:{DateTime} ", "GetTablesAsync",
                DateTimeOffset.UtcNow);

            return new PagedResponse<List<GetTableDto>>(response, filter.PageNumber, filter.PageSize, totalRecord);
        }
        catch (Exception e)
        {
            logger.LogError("Exception {Exception}, time={DateTimeNow}", e.Message, DateTimeOffset.UtcNow);
            return new PagedResponse<List<GetTableDto>>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region GetTableByIdAsync

    public async Task<Response<GetTableDto>> GetTableByIdAsync(int TableId)
    {
        try
        {
            logger.LogInformation("Starting method {GetTableByIdAsync} in time:{DateTime} ", "GetTableByIdAsync",
                DateTimeOffset.UtcNow);
            var existing = await context.Tables.Where(x => x.Id == TableId).Select(x => new GetTableDto()
            {
                TableNumber = x.TableNumber,
                TableStatus = x.TableStatus,
                Id = x.Id,
                PlaceCount = x.PlaceCount,
                CreateAt = x.CreateAt,
                UpdateAt = x.UpdateAt
            }).FirstOrDefaultAsync();
            if (existing is null)
            {
                logger.LogWarning("Not found Table with id={Id},time={DateTimeNow}", TableId, DateTime.UtcNow);
                return new Response<GetTableDto>(HttpStatusCode.BadRequest, "Table not found");
            }

            logger.LogInformation("Finished method {GetTableByIdAsync} in time:{DateTime} ", "GetTableByIdAsync",
                DateTimeOffset.UtcNow);
            return new Response<GetTableDto>(existing);
        }
        catch (Exception e)
        {
            logger.LogError("Exception {Exception}, time={DateTimeNow}", e.Message, DateTimeOffset.UtcNow);
            return new Response<GetTableDto>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region CreateTableAsync

    public async Task<Response<string>> CreateTableAsync(CreateTableDto createTable)
    {
        try
        {
            logger.LogInformation("Starting method {CreateTableAsync} in time:{DateTime} ", "CreateTableAsync",
                DateTimeOffset.UtcNow);
            var newTable = new Table()
            {
                UpdateAt = DateTimeOffset.UtcNow,
                CreateAt = DateTimeOffset.UtcNow,
                PlaceCount = createTable.PlaceCount,
                TableNumber = createTable.TableNumber,
            };
            if (createTable.TableStatus == Domain.Enums.TableStatus.Busy) newTable.TableStatus = "Busy";
            if (createTable.TableStatus == Domain.Enums.TableStatus.Reserved) newTable.TableStatus = "Reserved";
            if (createTable.TableStatus == Domain.Enums.TableStatus.Free) newTable.TableStatus = "Free";

            await context.Tables.AddAsync(newTable);
            await context.SaveChangesAsync();
            logger.LogInformation("Finished method {CreateTableAsync} in time:{DateTime} ", "CreateTableAsync",
                DateTimeOffset.UtcNow);
            return new Response<string>($"Successfully created a new Table by id:{newTable.Id}");
        }
        catch (Exception e)
        {
            logger.LogError("Exception {Exception}, time={DateTimeNow}", e.Message, DateTimeOffset.UtcNow);
            return new Response<string>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region UpdateTableAsync

    public async Task<Response<string>> UpdateTableAsync(UpdateTableDto updateTable)
    {
        try
        {
            logger.LogInformation("Starting method {UpdateTableAsync} in time:{DateTime} ", "UpdateTableAsync",
                DateTimeOffset.UtcNow);
            var existing = await context.Tables.FirstOrDefaultAsync(x => x.Id == updateTable.Id);
            if (existing is null)
            {
                logger.LogWarning("Table not found by id:{Id},time:{DateTimeNow} ", updateTable.Id,
                    DateTimeOffset.UtcNow);
                return new Response<string>(HttpStatusCode.BadRequest, "Table not found");
            }

            existing.PlaceCount = updateTable.PlaceCount;
            existing.TableNumber = updateTable.TableNumber;
            existing.UpdateAt = DateTimeOffset.UtcNow;
            if (updateTable.TableStatus == Domain.Enums.TableStatus.Busy) existing.TableStatus = "Busy";
            if (updateTable.TableStatus == Domain.Enums.TableStatus.Reserved) existing.TableStatus = "Reserved";
            if (updateTable.TableStatus == Domain.Enums.TableStatus.Free) existing.TableStatus = "Free";

            await context.SaveChangesAsync();
            logger.LogInformation("Finished method {UpdateTableAsync} in time:{DateTime} ", "UpdateTableAsync",
                DateTimeOffset.UtcNow);
            return new Response<string>($"Successfully updated Table by id:{existing.Id}");
        }
        catch (Exception e)
        {
            logger.LogError("Exception {Exception}, time={DateTimeNow}", e.Message, DateTimeOffset.UtcNow);
            return new Response<string>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region DeleteTableAsync

    public async Task<Response<bool>> DeleteTableAsync(int TableId)
    {
        try
        {
            logger.LogInformation("Starting method {DeleteTableAsync} in time:{DateTime} ", "DeleteTableAsync",
                DateTimeOffset.UtcNow);

            var existing = await context.Tables.FirstOrDefaultAsync(x => x.Id == TableId);
            if (existing == null)
                return new Response<bool>(HttpStatusCode.BadRequest, $"Table not found by id:{TableId}");

            context.Tables.Remove(existing);
            await context.SaveChangesAsync();

            logger.LogInformation("Finished method {DeleteTableAsync} in time:{DateTime} ", "DeleteTableAsync",
                DateTimeOffset.UtcNow);
            return new Response<bool>(true);
        }
        catch (Exception e)
        {
            logger.LogError("Exception {Exception}, time={DateTimeNow}", e.Message, DateTimeOffset.UtcNow);
            return new PagedResponse<bool>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion
}
