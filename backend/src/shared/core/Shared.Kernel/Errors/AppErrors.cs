using Quantropic.Toolkit.Results;

namespace Shared.Kernel.Errors
{
    public static class AppErrors
    {
        public static readonly ErrorCode Validation = ErrorCode.Custom(nameof(Validation), 10000);
        public static readonly ErrorCode Duplicate = ErrorCode.Custom(nameof(Duplicate), 10001);
        public static readonly ErrorCode Security = ErrorCode.Custom(nameof(Security), 10002);
        public static readonly ErrorCode InvalidPassword = ErrorCode.Custom(nameof(InvalidPassword), 10003);
    }
}