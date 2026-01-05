using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Makro.IMS.Services.Api.Sieve
{
    public static class ISieveProcessorExtensions
    {
        public static void ApplyMapping<T>(this SievePropertyMapper mapper, ISieveMapping<T> mapping) where T : class
        {
            mapping.ConfigureMap(mapper);
        }

        public static async Task<PagedResult<T>> GetPagedAsync<T>(this ISieveProcessor sieveProcessor, IQueryable<T> query, SieveModel sieveModel) where T : class
        {
            var result = new PagedResult<T>();

            var (pagedQuery, page, pageSize, rowCount, pageCount) = await GetPagedResultAsync(sieveProcessor, query, sieveModel);

            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = rowCount;
            result.PageCount = pageCount;

            result.Results = await pagedQuery.ToListAsync();

            return result;
        }



        public static async Task<PagedResult<TDto>> GetPagedAsync<T, TDto>(this ISieveProcessor sieveProcessor, IQueryable<T> query, SieveModel sieveModel, IMapper mapper) where T : class where TDto : class
        {
            var result = new PagedResult<TDto>();

            var (pagedQuery, page, pageSize, rowCount, pageCount) = await GetPagedResultAsync(sieveProcessor, query, sieveModel);

            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = rowCount;
            result.PageCount = pageCount;

            result.Results = mapper.Map<IList<TDto>>((await pagedQuery.ToListAsync()));

            return result;
        }

        private static async Task<(IQueryable<T> pagedQuery, int page, int pageSize, int rowCount, int pageCount)> GetPagedResultAsync<T>(ISieveProcessor sieveProcessor, IQueryable<T> query, SieveModel sieveModel = null) where T : class
        {
            var page = sieveModel?.Page ?? 1;
            var pageSize = sieveModel?.PageSize ?? 50;

            if (sieveModel != null)
            {
                // apply pagination in a later step
                query = sieveProcessor.Apply(sieveModel, query, applyPagination: false);
            }

            var rowCount = await query.CountAsync();

            var pageCount = (int)Math.Ceiling((double)rowCount / pageSize);

            var skip = (page - 1) * pageSize;
            var pagedQuery = query.Skip(skip).Take(pageSize);

            return (pagedQuery, page, pageSize, rowCount, pageCount);
        }


        public static async Task<PagedResult<T>> GetPagedExportAsync<T>(this ISieveProcessor sieveProcessor, IQueryable<T> query, SieveModel sieveModel) where T : class
        {
            var result = new PagedResult<T>();

            var pagedQuery = await GetPagedExportResultAsync(sieveProcessor, query, sieveModel);

            result.Results = await pagedQuery.ToListAsync();

            return result;
        }

        private static async Task<IQueryable<T>> GetPagedExportResultAsync<T>(ISieveProcessor sieveProcessor, IQueryable<T> query, SieveModel sieveModel = null) where T : class
        {
            
            if (sieveModel != null)
            {
                // apply pagination in a later step
                query = sieveProcessor.Apply(sieveModel, query, applyPagination: false);
            }
            
            var pagedQuery = query;

            return pagedQuery;
        }


        public static async Task<PagedResult<TDto>> GetPagedIListAsync<T, TDto>(this ISieveIListProcessor sieveProcessor, List<T> query, SieveModel sieveModel, IMapper mapper) where T : class where TDto : class
        {
            var result = new PagedResult<TDto>();

            var (pagedQuery, page, pageSize, rowCount, pageCount) = await GetPagedResultIListAsync(sieveProcessor, query, sieveModel);

            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = rowCount;
            result.PageCount = pageCount;

            result.Results = mapper.Map<IList<TDto>>(pagedQuery);

            return result;
        }

        public static async Task<PagedResult<T>> GetPagedIListAsync<T>(this ISieveIListProcessor sieveProcessor, List<T> query, SieveModel sieveModel) where T : class
        {
            var result = new PagedResult<T>();

            var (pagedQuery, page, pageSize, rowCount, pageCount) = await GetPagedResultIListAsync(sieveProcessor, query, sieveModel);

            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = rowCount;
            result.PageCount = pageCount;

            result.Results = pagedQuery;

            return result;
        }
        public static async Task<PagedResult<T>> GetPagedExportIListAsync<T>(this ISieveIListProcessor sieveProcessor, List<T> query, SieveModel sieveModel) where T : class
        {
            var result = new PagedResult<T>();

            var pagedQuery = await GetPagedExportResultIListAsync(sieveProcessor, query, sieveModel);

            result.Results = pagedQuery;

            return result;
        }

        private static async Task<List<T>> GetPagedExportResultIListAsync<T>(ISieveIListProcessor sieveProcessor, List<T> query, SieveModel sieveModel = null) where T : class
        {

            if (sieveModel != null)
            {
                // apply pagination in a later step
                query = sieveProcessor.Apply(sieveModel, query, applyPagination: false);
            }

            var pagedQuery = query;

            return pagedQuery;
        }
        private static async Task<(IList<T> pagedQuery, int page, int pageSize, int rowCount, int pageCount)> GetPagedResultIListAsync<T>(ISieveIListProcessor sieveProcessor, List<T> query, SieveModel sieveModel = null) where T : class
        {
            var page = sieveModel?.Page ?? 1;
            var pageSize = sieveModel?.PageSize ?? 50;

            if (sieveModel != null)
            {
                // apply pagination in a later step
                query = sieveProcessor.Apply(sieveModel, query, applyPagination: false);
            }

            var rowCount = query.ToList().Count();

            var pageCount = (int)Math.Ceiling((double)rowCount / pageSize);

            var skip = (page - 1) * pageSize;
            var pagedQuery = query.ToList().Skip(skip).Take(pageSize).ToList();
            return (pagedQuery, page, pageSize, rowCount, pageCount);
        }
    }
}
