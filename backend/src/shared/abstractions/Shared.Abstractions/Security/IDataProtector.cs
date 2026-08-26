namespace Shared.Abstractions.Security
{
    public interface IDataProtector
    {
        string Protect(string data);
        string Unprotect(string protectedData);
    }
}