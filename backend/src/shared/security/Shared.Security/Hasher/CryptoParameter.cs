namespace Shared.Security.Hasher
{
    public sealed record class CryptoParameter
    {
        public byte[] Salt { get; set; } = null!;
        public int DegreeOfParallelism { get; set; }
        public int Iterations { get; set; }
        public int MemorySizeKb { get; set; }
    }
}