using System.Collections.Concurrent;
using Medallion.Threading;

namespace Shared.Test.Cache
{
    public sealed class InMemoryDistributedLockProvider : IDistributedLockProvider
    {
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new();

        public IDistributedLock CreateLock(string name)
        {
            var semaphore = _semaphores.GetOrAdd(name, _ => new SemaphoreSlim(1, 1));
            return new InMemoryDistributedLock(semaphore);
        }
    }

    public sealed class InMemoryDistributedLock : IDistributedLock
    {
        private readonly SemaphoreSlim _semaphore;

        public InMemoryDistributedLock(SemaphoreSlim semaphore)
        {
            _semaphore = semaphore;
        }

        public string Name => throw new NotImplementedException();

        public IDistributedSynchronizationHandle Acquire(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IDistributedSynchronizationHandle> AcquireAsync(
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default)
        {
            _semaphore.Wait(cancellationToken);
            return new ValueTask<IDistributedSynchronizationHandle>(
                new InMemoryDistributedSynchronizationHandle(_semaphore));
        }

        public IDistributedSynchronizationHandle? TryAcquire(TimeSpan timeout = default, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IDistributedSynchronizationHandle?> TryAcquireAsync(
            TimeSpan timeout = default,
            CancellationToken cancellationToken = default)
        {
            if (_semaphore.Wait(timeout, cancellationToken))
            {
                return new ValueTask<IDistributedSynchronizationHandle?>(
                    new InMemoryDistributedSynchronizationHandle(_semaphore));
            }
            return new ValueTask<IDistributedSynchronizationHandle?>((IDistributedSynchronizationHandle?)null);
        }
    }

    public sealed class InMemoryDistributedSynchronizationHandle : IDistributedSynchronizationHandle
    {
        private readonly SemaphoreSlim _semaphore;
        private bool _disposed;

        public InMemoryDistributedSynchronizationHandle(SemaphoreSlim semaphore)
        {
            _semaphore = semaphore;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _semaphore.Release();
                _disposed = true;
            }
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            return default;
        }

        public CancellationToken HandleLostToken => default;
    }
}