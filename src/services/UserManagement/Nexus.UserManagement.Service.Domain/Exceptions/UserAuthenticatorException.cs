using Shared.Kernel.Exceptions;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Domain.Exceptions
{
    public sealed class UserAuthenticatorException(Error error) : DomainException(error)
    {
    }
}