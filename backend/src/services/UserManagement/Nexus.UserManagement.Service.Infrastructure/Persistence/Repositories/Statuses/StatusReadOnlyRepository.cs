using System.Data;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Constants;
using Shared.Contracts.UserManagement.Responses;
using Shared.Dapper;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Statuses
{
    internal sealed class StatusReadOnlyRepository(IDbConnection connection) : ReadOnlyRepository<StatusResponse>(connection, TableNames.Status), IStatusReadOnlyRepository
    {
        
    }
}