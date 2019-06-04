using PAWSwords.Core.Crypto;
using System;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace PAWSwords.Core.Passwords
{
    public class LazyPasswordEntry
    {
        private readonly byte[] _encryptedPassword;
        private readonly byte[] _iv;
        private readonly AesCrypter _aesCrypter;

        public LazyPasswordEntry(Guid id, string name, string description, byte[] encryptedPassword, byte[] iv, AesCrypter aesCrypter)
        {
            Id = id;
            UserName = name;
            Description = description;
            _encryptedPassword = encryptedPassword;
            _iv = iv;
            _aesCrypter = aesCrypter;
        }

        public Guid Id { get; }

        public string UserName { get; }

        public string Description { get; }
        public async Task<SecureString> GetPassword(byte[] masterKey)
        {
            var decryptedPassword = await _aesCrypter.Decrypt(_encryptedPassword, masterKey, _iv);

            var securePassword = new SecureString();
            var secureKeyCharArray = Encoding.ASCII.GetChars(decryptedPassword);
            for (int i = 0; i < secureKeyCharArray.Length; i++)
            {
                securePassword.AppendChar(secureKeyCharArray[i]);
                secureKeyCharArray[i] = (char)0;
            }

            securePassword.MakeReadOnly();
            return securePassword;
        }
    }
}