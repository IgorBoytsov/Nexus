using System.Data;
using Dapper;
using Nexus.UserManagement.Service.Application.Features.Users.Queries.GetProfileInfo;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Nexus.UserManagement.Service.Infrastructure.Helpers;
using Rebout.Nexus.Contracts.UserManagement.v1;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Users
{
    internal sealed class UserReadOnlyRepository(IDbConnection connection) : IUserReadOnlyRepository
    {        
        public async Task<Shared.Contracts.UserAuthDataResponse> GetUserByIdAuth(Guid userId)
        {
            var sql = SqlLoader.Load("Users", "GetUserByIdAuth");
            var user = await connection.QueryFirstOrDefaultAsync<Shared.Contracts.UserAuthDataResponse>(sql, new { userId });

            return user!;
        }

        public async Task<Shared.Contracts.UserAuthDataResponse> GetUserByLoginAuth(string login)
        {
            var sql = SqlLoader.Load("Users", "GetUserByLoginAuth");
            var user = await connection.QueryFirstOrDefaultAsync<Shared.Contracts.UserAuthDataResponse>(sql, new { login });

            return user!;
        }

        public async Task<ProfileInfoResponse> GetProfileInfo(Guid userId)
        {
            var sql = SqlLoader.Load("Users", "GetProfileInfo");
            var profileInfo = await connection.QueryFirstOrDefaultAsync<ProfileInfoResponse>(sql, new { userId });

            return profileInfo!;
        }

        public async Task<PublicEncryptionInfoResponse> GetPublicEncryptionInfoResponse(string login)
        {
            var sql = SqlLoader.Load("Users", "GetPublicEncryptionInfo");
            var info = await connection.QueryFirstOrDefaultAsync<PublicEncryptionInfoResponse>(sql, new { login });

            return info!;
        }
    }
}