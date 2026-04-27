using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.DTOs.Project;
using TaskFlow.Domain.Models;

namespace TaskFlow.Application.Extensions;

public static class ProjectQueryExtensions
{
    public static IQueryable<Project> ApplyProjectSearch(
        this IQueryable<Project> query,
        ProjectGetParameters? parameters
    )
    {
        if (parameters == null)
            return query;
        
        if (parameters.Name != null)
            query = query
                .Where(p => EF.Functions.Like(p.Name, $"%{parameters.Name}%"));

        if (parameters.Description != null)
            query = query
                .Where(p => EF.Functions.Like(p.Description, $"%{parameters.Description}%"));

        if (parameters.CreatedDate != null)
            query = query
                .Where(p => EF.Functions.Like(p.CreatedDate.ToShortTimeString(), $"%{parameters.CreatedDate}%"));

        if (parameters.IsArchived != null)
            query = query
                .Where(p => EF.Functions.Like(p.IsArchived.ToString(), $"%{parameters.IsArchived}%"));

        return query;
    }
}