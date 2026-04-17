using MediatR;
using TaskFlow.Application.Common;

namespace TaskFlow.Application.Interfaces.Messaging;

public interface IQueryHandler<TQuery, TResponse>
    :IRequestHandler<TQuery, Result<TResponse>>
    where TQuery: IQuery<TResponse>
{
    
}