using MediatR;
using Crossdyne.Toolkit.Results;
using Shared.Kernel.Errors;
using Shared.Abstractions.Cache;

namespace Nexus.UserManagement.Service.Application.Features.Users.Commands.ResetPasswordConfirmCode
{
    public sealed class ResetPasswordConfirmCodeCommandHandler(ICacheService redis) : IRequestHandler<ResetPasswordConfirmCodeCommand, Result>
    {
        private readonly ICacheService _redis = redis;

        public async Task<Result> Handle(ResetPasswordConfirmCodeCommand request, CancellationToken cancellationToken)
        {
            string normalizeLogin = request.Login.ToLowerInvariant();

            var codeStr = await _redis.GetStringAsync($"ConfirmCode for {normalizeLogin}");

            if (string.IsNullOrWhiteSpace(codeStr))
                return Result.Failure(new Error(AppErrors.TimeEnded, "Время действия кода закончилось. Повторите попытку"));

            var storageCode = int.Parse(codeStr);
            var requestCode = int.Parse(request.Code);

            if (storageCode != requestCode)
                return Result.Failure(new Error(AppErrors.IncorrectValue, "Вы ввели не верный код."));

            await _redis.RemoveAsync($"ConfirmCode for {normalizeLogin}");

            return Result.Success();
        }
    }
}