
using Crossdyne.Toolkit.Results;
using Shared.Kernel.Exceptions;

namespace Nexus.UserManagement.Service.Domain.Exceptions
{
    public sealed class DekException(Error error) : DomainException(error)
    {
        
    }
}