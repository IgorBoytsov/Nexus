using Crossdyne.Toolkit.Results;
using MediatR;
using Nexus.Bff.Extensions;
using Shared.Contracts.Authentication.Responses;
using Shared.Contracts.Cache.Interfaces;
using Shared.Contracts.Common;

namespace Nexus.Bff.Features.Auth.Command.CompleteSrpAuth
{
    public sealed class CompleteSrpAuthCommandHandler(IRedisCacheService cache) : IRequestHandler<CompleteSrpAuthCommand, Result<CompleteSrpAuthResponse>>
    {
        public async Task<Result<CompleteSrpAuthResponse>> Handle(CompleteSrpAuthCommand request, CancellationToken cancellationToken)
        {
            var templateCacheKey = RedisKeyExtensions.SrpTempToken(request.TempAuthToken);
            var userSession = await cache.GetJsonAsync<UserSession>(templateCacheKey);

            if (userSession is null)
                return new Error(ErrorCode.Server, "Произошла непредвиденная ошибка на стороне сервера. Пожалуйста повторите процесс входа.");

            var resultCache = await cache.SetJsonAsync(RedisKeyExtensions.SessionKey(userSession.SessionId), userSession, TimeSpan.FromDays(30));
            
            if (!resultCache)
                return new Error(ErrorCode.Server, "Произошла непредвиденная ошибка на стороне сервера. Пожалуйста повторите процесс входа.");

            await cache.RemoveAsync(templateCacheKey);

            return new CompleteSrpAuthResponse(userSession.SessionId, userSession.AccessToken, userSession.RefreshToken, userSession.AccessTokenExpiresAt, userSession.UserId, userSession.Login);
        }
    }
}