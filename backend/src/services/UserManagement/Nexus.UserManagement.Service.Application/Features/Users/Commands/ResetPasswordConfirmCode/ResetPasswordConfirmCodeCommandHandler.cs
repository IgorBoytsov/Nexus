using MediatR;
using Quantropic.Toolkit.Results;
using Shared.Contracts;
using Shared.Kernel.Errors;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ResetPasswordConfirmCode
{
    public sealed class ResetPasswordConfirmCodeCommandHandler(IRedisCacheService redis) : IRequestHandler<ResetPasswordConfirmCodeCommand, Result>
    {
        private readonly IRedisCacheService _redis = redis;

        public async Task<Result> Handle(ResetPasswordConfirmCodeCommand request, CancellationToken cancellationToken)
        {
            var codeStr = await _redis.GetStringAsync($"ConfirmCode for {request.Login}");

            if (string.IsNullOrWhiteSpace(codeStr))
                return Result.Failure(new Error(AppErrors.TimeEnded, "Время действия кода закончилось. Повторите попытку"));

            var code = int.Parse(codeStr);

            if (code != request.Code)
                return Result.Failure(new Error(AppErrors.IncorrectValue, "Вы ввели не верный код."));

            await _redis.RemoveAsync($"ConfirmCode for {request.Login}");

            return Result.Success();
        }
    }
}