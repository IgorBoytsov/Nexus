using Shared.Kernel.Exceptions;
using Shared.Kernel.Results;

namespace Nexus.UserManagement.Service.Domain.Exceptions
{
    public sealed class IterationsExecption(Error error) : DomainException(error)
    {
    }
}