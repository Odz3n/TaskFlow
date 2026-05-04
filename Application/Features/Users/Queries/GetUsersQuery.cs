using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.User;
using TaskFlow.Application.Interfaces.Messaging;

namespace TaskFlow.Application.Features.Users.Queries;

public record GetUsersQuery(
    UserGetParameters Parameters
) : IQuery<PagedResult<GetUsersResponse>>;