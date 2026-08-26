using MediatR;
using Microsoft.Extensions.Logging;
using Nexus.UserManagement.Service.Application.Abstractions.Events;
using Nexus.UserManagement.Service.Application.Abstractions.Messaging;
using Nexus.UserManagement.Service.Application.Abstractions.Transactions;
using Nexus.UserManagement.Service.Application.Abstractions.UnitOfWork;

namespace Nexus.UserManagement.Service.Application.Behaviors;

public sealed class TransactionBehavior<TRequest, TResponse>(
    ITransactionManager txManager,
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher eventDispatcher,
    ILogger<TransactionBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : ICommand
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (request is IQuery)
            return await next(ct);

        var requestName = typeof(TRequest).Name;

        if (txManager.HasActiveTransaction)
        {
            logger.LogDebug("[TX] NESTED => {Request}", requestName);
            return await next(ct);
        }

        await txManager.BeginAsync(ct);
        logger.LogDebug("[TX] BEGIN => {Request}", requestName);

        try
        {
            var response = await next(ct);

            var events = unitOfWork.GetPendingDomainEvents();

            if (events.Count > 0)
            {
                logger.LogDebug("[TX] DISPATCH {Count} in-process events => {Request}", events.Count, requestName);
                await eventDispatcher.DispatchAsync(events, ct);
            }

            await txManager.CommitAsync(ct);
            logger.LogDebug("[TX] COMMIT => {Request}", requestName);

            unitOfWork.ClearPendingDomainEvents();

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[TX] ROLLBACK => {Request}", requestName);
            await txManager.RollbackAsync(ct);
            unitOfWork.ClearPendingDomainEvents();
            throw;
        }
    }
}