using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PVG.Core.BaseModels;
using PVG.Domain.Constants;
using PVG.Domain.Models;
using PVG.Domain.Settings;

namespace PVG.Application.Services
{
    public class BaseService
    {
        protected readonly AppSettings _appSettings;
        protected readonly IMapper _mapper;

        public BaseService(IOptions<AppSettings> settings, IMapper mapper)
        {
            _appSettings = settings.Value;
            _mapper = mapper;
        }

        protected BaseResponse SuccessResponse(object? data, string message = "")
        {
            return new BaseResponse()
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Result = data,
                Message = message,
            };
        }

        protected BaseResponse BadRequestResponse(string errorCode, string errorMessage)
        {
            return new BaseResponse()
            {
                IsSuccess = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Error = new ErrorModel { ErrorCode = errorCode, ErrorMessage = $"{errorMessage} ({errorCode})", },
            };
        }

        protected BaseResponse CatchErrorResponse(Exception e)
        {
            return new BaseResponse()
            {
                IsSuccess = false,
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Internal Server Error",
                Error = new ErrorModel { ErrorCode = ErrorCodeConst.ERROR_SYS_ERR, ErrorMessage = $"{e.Message} ({ErrorCodeConst.ERROR_SYS_ERR})", },
            };
        }

        protected PaginationModel Paging<T>(List<T> items, int page, int pageSize)
        {
            var pagination = new PaginationModel();
            pagination.Items = items.Skip((page - 1) * pageSize)
                    .Take(pageSize).ToList();
            pagination.PageNumber = page;
            pagination.PerPage = pageSize;
            pagination.TotalItems = items.Count;
            pagination.TotalPages = (int)Math.Ceiling(pagination.TotalItems / (double)pagination.PerPage);

            return pagination;
        }

        protected IQueryable<T> Paging<T>(IQueryable<T> query, bool isPaging, int currentPage, int perPage)
        {
            if (isPaging)
            {
                query = query.Skip(perPage * (currentPage - 1)).Take(perPage);
            }
            return query;
        }

        protected List<T> Paging<T>(List<T> query, bool isPaging, int currentPage, int perPage)
        {
            if (isPaging)
            {
                query = query.Skip(perPage * (currentPage - 1)).Take(perPage).ToList();
            }
            return query;
        }

        protected async Task<PaginationModel> OffsetPagination<T>(IQueryable<T> query, int page, int pageSize)
        {
            var pagination = new PaginationModel();
            pagination.Items = await query
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();

            pagination.PageNumber = page;
            pagination.PerPage = pageSize;
            pagination.TotalItems = await query.CountAsync();
            pagination.TotalPages = (int)Math.Ceiling(pagination.TotalItems / (double)pagination.PerPage);

            return pagination;
        }
    }
}