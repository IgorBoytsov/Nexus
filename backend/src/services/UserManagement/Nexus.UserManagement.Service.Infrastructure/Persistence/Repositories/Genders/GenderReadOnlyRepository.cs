using System.Data;
using Nexus.UserManagement.Service.Application.Interfaces.Repositories;
using Nexus.UserManagement.Service.Infrastructure.Persistence.Constants;
using Shared.Contracts.UserManagement.Responses;
using Shared.Dapper;

namespace Nexus.UserManagement.Service.Infrastructure.Persistence.Repositories.Genders
{
    internal sealed class GenderReadOnlyRepository(IDbConnection connection) : ReadOnlyRepository<GenderResponse>(connection, TableNames.Gender), IGenderReadOnlyRepository
    {
        
    }
}