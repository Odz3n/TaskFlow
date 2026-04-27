using System.Linq.Expressions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common;

namespace TaskFlow.Application.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplyPagination<T>(
        this IQueryable<T> query,
        QueryParameters parameters
    )
    {
        var page = Math.Max(1, parameters.Page);
        var pageSize = parameters.PageSize;

        return query
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
    }
    public static IQueryable<T> ApplySort<T>(
        this IQueryable<T> query,
        QueryParameters parameters,
        Dictionary<string, Expression<Func<T, object>>> sortMapping
    )
    {
        if (string.IsNullOrWhiteSpace(parameters.SortBy))
            return query;

        if (!sortMapping.TryGetValue(parameters.SortBy.ToLowerInvariant(), out var sortExpression))
            return query;

        var isDescending = !string.IsNullOrWhiteSpace(parameters.SortOrder) &&
                       parameters.SortOrder.ToLowerInvariant() == "desc";

        return isDescending
            ? query.OrderByDescending(sortExpression)
            : query.OrderBy(sortExpression);
    }
    public static async Task<PagedResult<TDto>> ToPagedResultAsync<TEntity, TDto>(
        this IQueryable<TEntity> query,
        QueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var page = Math.Max(1, parameters.Page);
        var pageSize = parameters.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<TDto>()
            .ToListAsync(cancellationToken);

        return new PagedResult<TDto>(items, totalCount, page, pageSize);
    }
}