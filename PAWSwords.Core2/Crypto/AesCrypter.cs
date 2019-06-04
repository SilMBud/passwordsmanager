using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PAWSwords.Core.Crypto
{
    public class AesCrypter
    {
        public async Task<byte[]> Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (var aes = CreateAes(key, iv))
            {
                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var msEncrypt = new MemoryStream())
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        await swEncrypt.BaseStream.WriteAsync(data, 0, data.Length);
                    }
                    return msEncrypt.ToArray();
                }
            }
        }

        public async Task<byte[]> Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (var aes = CreateAes(key, iv))
            {
                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (var msDecrypt = new MemoryStream(data))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    return await ReadFully(csDecrypt);
                }
            }
        }

        private Aes CreateAes(byte[] key, byte[] iv)
        {
            var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Key = key;
            aes.IV = iv;

            return aes;
        }

        private static async Task<byte[]> ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    await ms.WriteAsync(buffer, 0, read);

                return ms.ToArray();
            }
        }
    }
}
