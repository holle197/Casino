using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Wallet.PrivateKeyGenerator.HASH
{
    internal static class SHA256Hashing
    {
        internal static byte[] GenerateHashBytes(this string secret)
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(secret);
            byte[] hashBytes = SHA256.HashData(textBytes);

            return hashBytes;
        }
    }
}
