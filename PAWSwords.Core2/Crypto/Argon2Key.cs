using Konscious.Security.Cryptography;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;

namespace PAWSwords.Core.Crypto
{
    public class Argon2Key : IDisposable
    {
        private Argon2Key(byte[] key)
        {
            Value = key;
        }

        public byte[] Value { get; private set; }

        public static async Task<Argon2Key> Calculate(SecureString password)
        {
            var buffer = new byte[password.Length];
            var ptr = Marshal.SecureStringToGlobalAllocAnsi(password);
            try
            {
                Marshal.Copy(ptr, buffer, 0, buffer.Length);
                using (var argon2 = new Argon2id(buffer)
                {
                    Iterations = 10,
                    DegreeOfParallelism = 16,
                    MemorySize = 256
                })
                {
                    return new Argon2Key(await argon2.GetBytesAsync(32));
                }
            }
            finally
            {
                Helpers.ClearData(buffer);
                Marshal.ZeroFreeGlobalAllocAnsi(ptr);
            }
        }

        public void Dispose()
        {
            Helpers.ClearData(Value);
            Value = null;
        }
    }
}