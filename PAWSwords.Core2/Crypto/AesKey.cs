using System;
using System.Security.Cryptography;

namespace PAWSwords.Core
{
    public class AesKey : IDisposable
    {
        private AesKey(byte[] key, byte[] iv)
        {
            Key = key;
            Iv = iv;
        }

        public byte[] Key { get; private set; }

        public byte[] Iv { get; private set; }

        public static AesKey Generate()
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.Mode = CipherMode.CBC;
                return new AesKey(aes.Key, aes.IV);
            }
        }

        public void Dispose()
        {
            Helpers.ClearData(Key);
            Key = null;

            Helpers.ClearData(Iv);
            Iv = null;
        }

    }
}
