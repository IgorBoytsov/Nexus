using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ExistByLogin
{
    public sealed class ExistUserByLoginCommandHandler(IUserRepository userRepository) : IRequestHandler<ExistUserByLoginCommand, Result>
    {
        public async Task<Result> Handle(ExistUserByLoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var exist = await userRepository.Exist(x => x.Login == request.Login, cancellationToken);

                if (!exist)
                    return Result.Failure(new Error(ErrorCode.NotExists, "Пользователь не найден"));

                return Result.Success();
            }
            catch (Exception)
            {
                return Result.Failure(new Error(ErrorCode.Server, "Ошибка на стороне сервера"));
            }
        }
    }
}