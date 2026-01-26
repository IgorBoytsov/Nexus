using Shared.Kernel.Exceptions;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Domain.Exceptions
{
    public sealed class UserSecurityAssetsExeption(Error error) : DomainException(error)
    {
    }
}