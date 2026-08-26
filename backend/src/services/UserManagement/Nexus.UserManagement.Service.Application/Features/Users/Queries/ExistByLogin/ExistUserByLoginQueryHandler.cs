using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.UserManagement.Service.Application.Abstractions.Repositories;

namespace Nexus.UserManagement.Service.Application.Features.Users.Queries.ExistByLogin
{
    public sealed class ExistUserByLoginQueryHandler(IUserReadOnlyRepository userRepository) : IRequestHandler<ExistUserByLoginQuery, Result>
    {
        public async Task<Result> Handle(ExistUserByLoginQuery request, CancellationToken cancellationToken)
        {
            var exist = await userRepository.ExistUserByLoginAsync(request.Login);
            
            if (!exist)
                return Result.Failure(new Error(ErrorCode.NotExists, "Пользователь не найден"));
                
            return Result.Success();
        }
    }
}