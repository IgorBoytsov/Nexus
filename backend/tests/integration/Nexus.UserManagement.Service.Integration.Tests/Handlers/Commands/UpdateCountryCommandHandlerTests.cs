using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Nexus.UserManagement.Service.Application.Abstractions.UnitOfWork;
using Nexus.UserManagement.Service.Application.Features.Countries.Commands.Update;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Infrastructure.Outbox;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Countries;
using Xunit;

namespace Nexus.UserManagement.Service.Integration.Tests.Handlers.Commands
{
    public class UpdateCountryCommandHandlerTests : IClassFixture<TestFixture>, IAsyncLifetime
    {
        private readonly TestFixture _fixture;
        private readonly UserManagementContext _context;
        private readonly CountryRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly OutboxSignal _outboxSignal;
        private readonly UpdateCountryCommandHandler _handler;
        private readonly CancellationToken _ct = default;

        public UpdateCountryCommandHandlerTests(TestFixture fixture)
        {
            _fixture = fixture;

            _context = fixture.CreateDbContext();
            _repo = new CountryRepository(_context);
            _outboxSignal = (OutboxSignal)fixture.CreateOutboxSignal();
            _uow = fixture.CreateUnitOfWork(_context);

            _handler = new UpdateCountryCommandHandler(_uow, _repo);
        }

        public async ValueTask InitializeAsync() => await _fixture.ResetDatabaseAsync();

        public async ValueTask DisposeAsync()
        {
            _outboxSignal.Dispose();
            await _context.DisposeAsync();
        }

        [Fact]
        public async Task Handle_ExistingId_ShouldUpdatedCountry()
        {
            var country = Country.Create("Казахстан");
            await _context.Countries.AddAsync(country, _ct);
            await _context.SaveChangesAsync(_ct);

            string newName = "NewName";

            var command = new UpdateCountryCommand(country.Id, newName);

            var result = await _handler.Handle(command, _ct);

            result.IsSuccess.Should().BeTrue();
            var updated = await _context.Countries.FirstOrDefaultAsync(c => c.Id == country.Id, _ct);
            updated?.Name.Value.Should().Be(newName);
        }

        [Fact]
        public async Task Handle_NonExistingId_ShouldReturnFailure()
        {
            var command = new UpdateCountryCommand(Guid.NewGuid(), "NewName");
            var result = await _handler.Handle(command, _ct);
            result.IsSuccess.Should().BeFalse();
        }
    }
}