using Microsoft.EntityFrameworkCore.Storage;
using Nexus.UserManagement.Service.Application.Abstractions.Transactions;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence
{
    public sealed class EfTransactionManager(UserManagementContext context) : ITransactionManager, IAsyncDisposable
    {
        private IDbContextTransaction? _transaction;
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private int _savepointDepth;

        public bool HasActiveTransaction => _transaction is not null;

        public async Task BeginAsync(CancellationToken ct = default)
        {
            await _semaphore.WaitAsync(ct);

            try
            {
                if (_transaction is not null)
                {
                    _savepointDepth++;
                    await _transaction.CreateSavepointAsync($"SP_{_savepointDepth}", ct);
                    return;
                }

                _transaction = await context.Database.BeginTransactionAsync(ct);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task CommitAsync(CancellationToken ct = default)
        {
            await _semaphore.WaitAsync(ct);

            try
            {
                if (_transaction is null)
                    throw new InvalidOperationException("Нету активных транзакций доступных для комита.");

                    if (_savepointDepth > 0)
                {
                    await _transaction.ReleaseSavepointAsync($"SP_{_savepointDepth}", ct);
                    _savepointDepth--;
                    return;
                }

                await _transaction.CommitAsync(ct);
            }
            finally
            {
                if (_savepointDepth == 0)
                    await DisposeTransactionAsync();

                _semaphore.Release();
            }
        }

        public async Task RollbackAsync(CancellationToken ct = default)
        {
            await _semaphore.WaitAsync(ct);
            
            try
            {
                if (_transaction is null)
                    return;

                if (_savepointDepth > 0)
                {
                    await _transaction.RollbackToSavepointAsync($"SP_{_savepointDepth}", ct);
                    _savepointDepth--;
                    return;
                }

                await _transaction.RollbackAsync(ct);
            }
            finally
            {
                if (_savepointDepth == 0)
                    await DisposeTransactionAsync();
                
                _semaphore.Release();
            }
        }

        private async Task DisposeTransactionAsync()
        {
            if (_transaction is not null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _semaphore.WaitAsync();

            try
            {
                if (_transaction is not null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
            finally
            {
                _semaphore.Release();
                _semaphore.Dispose();
            }
        }
    }
}