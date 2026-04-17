using MediatR;
using TaskFlow.Application.Common;

namespace TaskFlow.Application.Interfaces.Messaging;

public interface IQuery<TResponse>: IRequest<Result<TResponse>>
{
    
}