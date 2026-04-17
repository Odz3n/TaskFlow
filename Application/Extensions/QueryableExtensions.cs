using System.Linq.Expressions;
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

        var isDescending = parameters.SortOrder.ToLowerInvariant() == "desc";

        return isDescending
            ? query.OrderByDescending(sortExpression)
            : query.OrderBy(sortExpression);
    }
    public static IQueryable<T> ApplySearch<T>(
        this IQueryable<T> query,
        string? searchTerm,
        params Expression<Func<T, string?>>[] searchProperties)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return query;

        var searchLower = searchTerm.ToLowerInvariant();

        return query
            .Where(item => searchProperties.Any(property =>
                EF.Property<string>(item, GetPropertyName(property)).ToLowerInvariant().Contains(searchLower)));
    }
    private static string GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> expression)
    {
        if (expression.Body is MemberExpression memberExpression)
            return memberExpression.Member.Name;
        throw new ArgumentException("Invalid expression");
    }
    // public static async Task<PagedResult<TDto>> ToPagedResultAsync<TEntity, TDto>(
    //     this IQueryable<TEntity> query,
    //     QueryParameters parameters,
    //     IConfigurationProvider mapperConfig,
    //     CancellationToken cancellationToken = default)
    // {
    //     var page = Math.Max(1, parameters.Page);
    //     var pageSize = parameters.PageSize;

    //     var totalCount = await query.CountAsync(cancellationToken);

    //     var items = await query
    //         .Skip((page - 1) * pageSize)
    //         .Take(pageSize)
    //         .ProjectTo<TDto>(mapperConfig)
    //         .ToListAsync(cancellationToken);

    //     return new PagedResult<TDto>(items, totalCount, page, pageSize);
    // }
}