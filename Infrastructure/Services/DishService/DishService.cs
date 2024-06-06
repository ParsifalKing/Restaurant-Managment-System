using System.Net;
using Domain.DTOs.DishDTOs;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Data;
using Infrastructure.Services.FileService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.DishService;

public class DishService(IFileService fileService, ILogger<DishService> logger, DataContext context) : IDishService
{
    #region GetDishesAsync

    public async Task<PagedResponse<List<GetDishDto>>> GetDishesAsync(DishFilter filter)
    {
        try
        {
            logger.LogInformation("Starting method {GetDishesAsync} in time:{DateTime} ", "GetDishesAsync",
                DateTimeOffset.UtcNow);

            var Dishes = context.Dishes.AsQueryable();
            if (!string.IsNullOrEmpty(filter.Name))
                Dishes = Dishes.Where(x => x.Name.ToLower().Contains(filter.Name.ToLower()));

            var response = await Dishes.Select(x => new GetDishDto()
            {
                Name = x.Name,
                Description = x.Description,
                Id = x.Id,
                MenuId = x.MenuId,
                Price = x.Price,
                PathPhoto = x.PathPhoto,
                CreateAt = x.CreateAt,
                UpdateAt = x.UpdateAt
            }).Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();

            var totalRecord = await Dishes.CountAsync();

            logger.LogInformation("Finished method {GetDishesAsync} in time:{DateTime} ", "GetDishesAsync",
                DateTimeOffset.UtcNow);

            return new PagedResponse<List<GetDishDto>>(response, filter.PageNumber, filter.PageSize, totalRecord);
        }
        catch (Exception e)
        {
            logger.LogError("Exception {Exception}, time={DateTimeNow}", e.Message, DateTimeOffset.UtcNow);
            return new PagedResponse<List<GetDishDto>>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region GetDishByIdAsync

    public async Task<Response<GetDishDto>> GetDishByIdAsync(int DishId)
    {
        try
        {
            logger.LogInformation("Starting method {GetDishByIdAsync} in time:{DateTime} ", "GetDishByIdAsync",
                DateTimeOffset.UtcNow);
            var existing = await context.Dishes.Where(x => x.Id == DishId).Select(x => new GetDishDto()
            {
                Name = x.Name,
                Description = x.Description,
                Id = x.Id,
                MenuId = x.MenuId,
                Price = x.Price,
                PathPhoto = x.PathPhoto,
                CreateAt = x.CreateAt,
                UpdateAt = x.UpdateAt
            }).FirstOrDefaultAsync();
            if (existing is null)
            {
                logger.LogWarning("Not found Dish with id={Id},time={DateTimeNow}", DishId, DateTime.UtcNow);
                return new Response<GetDishDto>(HttpStatusCode.BadRequest, "Dish not found");
            }

            logger.LogInformation("Finished method {GetDishByIdAsync} in time:{DateTime} ", "GetDishByIdAsync",
                DateTimeOffset.UtcNow);
            return new Response<GetDishDto>(existing);
        }
        catch (Exception e)
        {
            logger.LogError("Exception {Exception}, time={DateTimeNow}", e.Message, DateTimeOffset.UtcNow);
            return new Response<GetDishDto>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region CreateDishAsync

    public async Task<Response<string>> CreateDishAsync(CreateDishDto createDish)
    {
        try
        {
            logger.LogInformation("Starting method {CreateDishAsync} in time:{DateTime} ", "CreateDishAsync",
                DateTimeOffset.UtcNow);
            var newDish = new Dish()
            {
                UpdateAt = DateTimeOffset.UtcNow,
                CreateAt = DateTimeOffset.UtcNow,
                Description = createDish.Description,
                Name = createDish.Name,
                MenuId = createDish.MenuId,
                Price = createDish.Price,
                PathPhoto = createDish.PathPhoto == null ? "null" : await fileService.CreateFile(createDish.PathPhoto)
            };

            await context.Dishes.AddAsync(newDish);
            await context.SaveChangesAsync();
            logger.LogInformation("Finished method {CreateDishAsync} in time:{DateTime} ", "CreateDishAsync",
                DateTimeOffset.UtcNow);
            return new Response<string>($"Successfully created a new Dish by id:{newDish.Id}");
        }
        catch (Exception e)
        {
            logger.LogError("Exception {Exception}, time={DateTimeNow}", e.Message, DateTimeOffset.UtcNow);
            return new Response<string>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region UpdateDishAsync

    public async Task<Response<string>> UpdateDishAsync(UpdateDishDto updateDish)
    {
        try
        {
            logger.LogInformation("Starting method {UpdateDishAsync} in time:{DateTime} ", "UpdateDishAsync",
                DateTimeOffset.UtcNow);
            var existing = await context.Dishes.FirstOrDefaultAsync(x => x.Id == updateDish.Id);
            if (existing is null)
            {
                logger.LogWarning("Dish not found by id:{Id},time:{DateTimeNow} ", updateDish.Id,
                    DateTimeOffset.UtcNow);
                return new Response<string>(HttpStatusCode.BadRequest, "Dish not found");
            }

            if (updateDish.PathPhoto != null)
            {
                if (existing.PathPhoto != null) fileService.DeleteFile(existing.PathPhoto);
                existing.PathPhoto = await fileService.CreateFile(updateDish.PathPhoto);
            }

            existing.Description = updateDish.Description;
            existing.Name = updateDish.Name;
            existing.Price = updateDish.Price;
            existing.MenuId = updateDish.MenuId;
            existing.UpdateAt = DateTimeOffset.UtcNow;

            await context.SaveChangesAsync();
            logger.LogInformation("Finished method {UpdateDishAsync} in time:{DateTime} ", "UpdateDishAsync",
                DateTimeOffset.UtcNow);
            return new Response<string>($"Successfully updated Dish by id:{existing.Id}");
        }
        catch (Exception e)
        {
            logger.LogError("Exception {Exception}, time={DateTimeNow}", e.Message, DateTimeOffset.UtcNow);
            return new Response<string>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    #endregion

    #region DeleteDishAsync

    public async Task<Response<bool>> DeleteDishAsync(int DishId)
    {
        try
        {
            logger.LogInformation("Starting method {DeleteDishAsync} in time:{DateTime} ", "DeleteDishAsync",
                DateTimeOffset.UtcNow);

            var existing = await context.Dishes.FirstOrDefaultAsync(x => x.Id == DishId);
            if (existing == null)
                return new Response<bool>(HttpStatusCode.BadRequest, $"Dish not found by id:{DishId}");
            if (existing.PathPhoto != null)
                fileService.DeleteFile(existing.PathPhoto);
            context.Dishes.Remove(existing);
            await context.SaveChangesAsync();

            logger.LogInformation("Finished method {DeleteDishAsync} in time:{DateTime} ", "DeleteDishAsync",
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

