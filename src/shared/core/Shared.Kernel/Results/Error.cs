namespace Shared.Kernel.Results
{
    public sealed record Error(ErrorCode Code, string Message);
}