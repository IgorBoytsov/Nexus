using Crossdyne.Toolkit.Results;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Nexus.UserManagement.Service.Application.Abstractions.UnitOfWork;
using Nexus.UserManagement.Service.Application.Features.Users.Commands.ChangeAvatar;
using Nexus.UserManagement.Service.Domain.Models;
using Nexus.UserManagement.Service.Domain.ValueObjects.Common;
using Nexus.UserManagement.Service.Domain.ValueObjects.User;
using Nexus.UserManagement.Service.Infrastructure.Clients;
using Nexus.UserManagement.Service.Infrastructure.Outbox;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Contexts;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Users;
using Shared.Kernel.Errors;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;

namespace Nexus.UserManagement.Service.Integration.Tests.Handlers.Commands
{
    public class ChangeAvatarCommandHandlerTests : IClassFixture<TestFixture>, IAsyncLifetime
    {
        private readonly TestFixture _fixture;
        private readonly UserManagementContext _context;
        private readonly UserRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly OutboxSignal _outboxSignal;
        private readonly ChangeAvatarCommandHandler _handler;
        private readonly CancellationToken _ct = default;

        public ChangeAvatarCommandHandlerTests(TestFixture fixture)
        {
            _fixture = fixture;

            _context = fixture.CreateDbContext();
            _repo = new UserRepository(_context);
            _outboxSignal = (OutboxSignal)fixture.CreateOutboxSignal();
            _uow = fixture.CreateUnitOfWork(_context);
            var fileClient = new FileService(new HttpClient{ BaseAddress = new Uri(_fixture.FileServiceMock.Url!) });

            _handler = new ChangeAvatarCommandHandler(fileClient, _repo, _uow, NullLogger<ChangeAvatarCommandHandler>.Instance);
        }

        public async ValueTask InitializeAsync() => await _fixture.ResetDatabaseAsync();

        public async ValueTask DisposeAsync()
        {
            _outboxSignal.Dispose();
            await _context.DisposeAsync();
        }

        [Fact]
        public async Task Handle_ExistingId_ShouldUpdateAvatar()
        {
            _fixture.FileServiceMock.Reset();

            var user = User.Create(
                Login.Create("TestLogin"),
                UserName.Create("TestUserName"),
                Email.Create("valid@email.com"),
                statusId: Guid.NewGuid(),
                genderId: Guid.NewGuid(),
                countryId: Guid.NewGuid());

            user.ChangeAvatar(S3Key.Create("TestBucket", ["users", "avatar"], "OldName.png"));

            await _context.Users.AddAsync(user, _ct);
            await _context.SaveChangesAsync(_ct);

            _fixture.FileServiceMock
                .Given(Request.Create().WithPath("/**").UsingPost())
                .RespondWith(Response.Create().WithStatusCode(200));

            var command = new ChangeAvatarCommand(
                user.Id,
                new MemoryStream([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]),
                "NewName.png");

            var result = await _handler.Handle(command, _ct);

            result.IsSuccess.Should().BeTrue();

            var updated = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == user.Id, _ct);

            updated.Should().NotBeNull();
            updated!.AvatarKey.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_NonExistingUserId_ShouldReturnFailureNotFoundCode()
        {
            var command = new ChangeAvatarCommand(Guid.NewGuid(), new MemoryStream([1, 2, 3]), "NewName.png");
            var result = await _handler.Handle(command, _ct);
            result.IsSuccess.Should().BeFalse();
            result.Errors.FirstOrDefault(e => e.Code == ErrorCode.NotFound)?.Code.Should().Be(ErrorCode.NotFound);
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldReturnFailureApiErrorCodeFileService()
        {            
            _fixture.FileServiceMock.Reset();

            var user = User.Create(
                Login.Create("TestLogin"),
                UserName.Create("TestUserName"),
                Email.Create("valid@email.com"),
                statusId: Guid.NewGuid(),
                genderId: Guid.NewGuid(),
                countryId: Guid.NewGuid());

            user.ChangeAvatar(S3Key.Create("TestBucket", ["users", "avatar"], "OldName.png"));

            await _context.Users.AddAsync(user, _ct);
            await _context.SaveChangesAsync(_ct);

            _fixture.FileServiceMock
                .Given(Request.Create().WithPath("/**").UsingPost())
                .RespondWith(Response.Create().WithStatusCode(400));

            var command = new ChangeAvatarCommand(
                user.Id,
                new MemoryStream([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]),
                "NewName.png");

            var result = await _handler.Handle(command, _ct);
            result.IsSuccess.Should().BeFalse();
            result.Errors.FirstOrDefault(e => e.Code == AppErrors.Api)?.Code.Should().Be(AppErrors.Api);
        }

        [Fact]
        public async Task Handle_InvalidIncorrectMimeType_ShouldReturnFailureValidationErrorCode()
        {            
            var user = User.Create(
                Login.Create("TestLogin"),
                UserName.Create("TestUserName"),
                Email.Create("valid@email.com"),
                statusId: Guid.NewGuid(),
                genderId: Guid.NewGuid(),
                countryId: Guid.NewGuid());

            user.ChangeAvatar(S3Key.Create("TestBucket", ["users", "avatar"], "OldName.png"));

            await _context.Users.AddAsync(user, _ct);
            await _context.SaveChangesAsync(_ct);

            var command = new ChangeAvatarCommand(
                user.Id,
                new MemoryStream([0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00]),
                "NewName.docx");
                
            var result = await _handler.Handle(command, _ct);
            result.IsSuccess.Should().BeFalse();
            result.Errors.FirstOrDefault(e => e.Code == AppErrors.Validation)?.Code.Should().Be(AppErrors.Validation);
        }
    }
}