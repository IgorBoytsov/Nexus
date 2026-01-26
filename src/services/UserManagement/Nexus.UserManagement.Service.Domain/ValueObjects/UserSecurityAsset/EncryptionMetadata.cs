using Nexus.UserManagement.Service.Domain.Exceptions;
using Shared.Kernel.Results;
using System.Text.Json;

namespace Nexus.UserManagement.Service.Domain.ValueObjects.UserSecurityAsset
{
    public readonly record struct EncryptionMetadata
    {
        public const int MIN_COUNT_ITERATIONS = 100_000;

        public string Algorithm { get; init; }
        public int Iterations { get; init; }
        public string KdfType { get; init; }

        private EncryptionMetadata(string algorithm, int iterations, string kdfType)
        {
            Algorithm = algorithm;
            Iterations = iterations;
            KdfType = kdfType;
        }

        public static EncryptionMetadata Create(string algorithm, int iterations, string kdfType)
        {
            if (MIN_COUNT_ITERATIONS < 100_000)
                throw new IterationsExecption(new Error(ErrorCode.Security, $"Слишком маленькое кол-во итераций являеться не безопасным. Меньше {MIN_COUNT_ITERATIONS} указать нельзя."));

            return new EncryptionMetadata(algorithm, iterations, kdfType);
        }

        public static EncryptionMetadata FromStorage(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) 
                return default;

            return JsonSerializer.Deserialize<EncryptionMetadata>(json);
        }

        public override string ToString() => JsonSerializer.Serialize(this);

        public static implicit operator string(EncryptionMetadata metadata) => metadata.ToString();
    }
}