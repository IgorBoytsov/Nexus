namespace Shared.Kernel.Guard
{
    public static class GuardException
    {
        public static IGuardClause Against { get; } = new GuardClause();
    }
}