using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;

namespace Nexus.Authentication.Service.Application.Secure
{
    public static class SrpConstants
    {
        public static readonly BigInteger N = BigInteger.Parse("00AC6BDB41324A9A9BF166DE5E1F403D434A6E1B3B94A7E62AC1211858E002C75AD4455C9D19C0A3180296917A376205164043E20144FF485719D181A99EB574671AC58054457ED444A67032EA17D03AD43464D2397449CA593630A670D90D95A78E846A3C8AF80862098D80F33C42ED7059E75225E0A52718E2379369F65B79680A6560B080092EE71986066735A96A7D42E7597116742B02D3A154471B6A23D84E0D642C790D597A2BB7F5A48F734898BDD138C69493E723491959C1B4BD40C91C1C7924F88D046467A006507E781220A80C55A927906A7C6C9C227E674686DD5D1B855D28F0D604E24586C608630B9A34C4808381A54F0D9080A5F90B60187F", NumberStyles.HexNumber);

        public static readonly int g = 2;
        public static readonly BigInteger k = CalculateK();

        private static BigInteger CalculateK()
        {
            using var sha256 = SHA256.Create();

            byte[] nBytes = ToFixedLengthStatic(N, 384); 
            byte[] gBytes = ToFixedLengthStatic(new BigInteger(SrpConstants.g), 384);

            byte[] combined = new byte[768];
            Buffer.BlockCopy(nBytes, 0, combined, 0, 384);
            Buffer.BlockCopy(gBytes, 0, combined, 384, 384);

            byte[] hash = sha256.ComputeHash(combined);
            return new BigInteger(hash, isUnsigned: true, isBigEndian: true);
        }

        private static byte[] ToFixedLengthStatic(BigInteger value, int length)
        {
            byte[] bytes = value.ToByteArray(isUnsigned: true, isBigEndian: true);
            if (bytes.Length == length) return bytes;
            if (bytes.Length > length) throw new ArgumentException("Value too large");
            
            byte[] padded = new byte[length];
            Buffer.BlockCopy(bytes, 0, padded, length - bytes.Length, bytes.Length);
            return padded;
        }
    }
}