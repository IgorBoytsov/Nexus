namespace Shared.Contracts.Security.Interfaces
{
    public interface IDataProtector
    {
        string Protect(string data);
        string Unprotect(string protectedData);
    }
}