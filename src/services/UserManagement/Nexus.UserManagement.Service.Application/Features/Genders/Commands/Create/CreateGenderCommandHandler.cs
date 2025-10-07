using MediatR;
using Nexus.UserManagement.Service.Application.Abstractions.Contexts;
using Nexus.UserManagement.Service.Domain.Models;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Application.Features.Genders.Commands.Create
{
    public sealed class CreateGenderCommandHandler(IWriteDbContext writeContext) : IRequestHandler<CreateGenderCommand, Result<Guid>>
    {
        private readonly IWriteDbContext _writeContext = writeContext;

        public async Task<Result<Guid>> Handle(CreateGenderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var gender = Gender.Create(request.Name);

                await _writeContext.Genders.AddAsync(gender, cancellationToken);

                await _writeContext.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(gender.Id);
            }
            catch (Exception)
            {
                return Result<Guid>.Failure(new Error(ErrorCode.Create, "Ошибка на стороне сервера"));
            }
        }
    }
}