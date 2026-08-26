using System.Data;
using Nexus.UserManagement.Service.Application.Abstractions.Repositories;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Constants;
using Shared.Contracts.UserManagement.Responses;
using Shared.Dapper;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Roles
{
    internal sealed class RoleReadOnlyRepository(IDbConnection connection) : ReadOnlyRepository<RoleResponse>(connection, TableNames.Role), IRoleReadOnlyRepository
    {
        
    }
}