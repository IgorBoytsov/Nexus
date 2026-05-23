using Crossdyne.Toolkit.Results;

namespace Shared.Kernel.Errors
{
    public static class AppErrors
    {
        public static readonly ErrorCode Validation = ErrorCode.Custom(nameof(Validation), 10000);
        public static readonly ErrorCode Duplicate = ErrorCode.Custom(nameof(Duplicate), 10001);
        public static readonly ErrorCode Security = ErrorCode.Custom(nameof(Security), 10002);
        public static readonly ErrorCode InvalidPassword = ErrorCode.Custom(nameof(InvalidPassword), 10003);
        public static readonly ErrorCode Api = ErrorCode.Custom(nameof(Api), 10004);
        public static readonly ErrorCode TimeEnded = ErrorCode.Custom(nameof(TimeEnded), 10005);
        public static readonly ErrorCode IncorrectValue = ErrorCode.Custom(nameof(IncorrectValue), 10005);
        public static readonly ErrorCode SessionExpired = ErrorCode.Custom(nameof(SessionExpired), 10006);
        public static readonly ErrorCode AccountNotSetUpForRecovery = ErrorCode.Custom(nameof(AccountNotSetUpForRecovery), 10007);
        public static readonly ErrorCode AlreadyUsed = ErrorCode.Custom(nameof(AlreadyUsed), 10008);
    }
}