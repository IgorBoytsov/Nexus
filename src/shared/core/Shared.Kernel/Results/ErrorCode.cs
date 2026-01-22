namespace Shared.Kernel.Results
{
    public enum ErrorCode
    {
        None = 0,
        Create = 1,
        Delete = 2,
        Update = 3,
        NotFound = 4,
        InvalidPassword = 5,
        Server = 6,
        Conflict = 7,
        Save = 8,
        Unauthorized = 9,
        Validation = 10,
    }
}