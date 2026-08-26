using AutoMapper;
using Nexus.UserManagement.Service.Application.Abstractions.Outbox;
using Nexus.UserManagement.Service.Application.Abstractions.UnitOfWork;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;
using Shared.Kernel.Primitives;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence
{
    public class UnitOfWork(
        UserManagementContext context, 
        IDbContextOutbox outbox,
        IOutboxSignal outboxSignal) : IUnitOfWork
    {
        private readonly List<IDomainEvent> _pendingEvents = [];
        private readonly SemaphoreSlim _eventsLock = new(1, 1);
        private bool _disposed;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            
            var domainEvents = CollectDomainEvents();
            bool hasEvents = domainEvents.Count > 0;

            await _eventsLock.WaitAsync(cancellationToken);
            try
            {
                _pendingEvents.AddRange(domainEvents);
            }
            finally
            {
                _eventsLock.Release();
            }

            if (hasEvents)
                outbox.Append(domainEvents);

            int countUpdate = await context.SaveChangesAsync(cancellationToken);

            if (hasEvents)
                outboxSignal.Signal();

            return countUpdate;
        }

        public IReadOnlyCollection<IDomainEvent> GetPendingDomainEvents()
        {
            lock (_eventsLock)
            {
                return _pendingEvents.AsReadOnly();
            }
        }

        public void ClearPendingDomainEvents()
        {
            lock (_eventsLock)
            {
                _pendingEvents.Clear();
            }
        }

        private List<IDomainEvent> CollectDomainEvents()
        {
            var entries = context.ChangeTracker
                .Entries<IAggregateRoot>()
                .Where(e => e.Entity.DomainEvents.Count > 0)
                .ToList();

            var events = entries
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            entries.ForEach(e => e.Entity.ClearDomainEvents());

            return events;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _eventsLock.Dispose();
            context.Dispose();
        }
    }
}