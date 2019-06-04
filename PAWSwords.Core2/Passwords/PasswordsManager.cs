using PAWSwords.Core.Crypto;
using PAWSwords.Core.Interfaces;
using PAWSwords.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace PAWSwords.Core.Passwords
{
    public class PasswordsManager : IPasswordsManager
    {
        private readonly FileStorage _fileStorage;
        private readonly AesCrypter _aesCrypter;
        private readonly IMasterKeyManager _masterKeyManager;

        public PasswordsManager(FileStorage fileStorage, AesCrypter aesCrypter, IMasterKeyManager masterKeyManager)
        {
            _fileStorage = fileStorage;
            _aesCrypter = aesCrypter;
            _masterKeyManager = masterKeyManager;
        }

        public async Task<PasswordEntry> Add(PasswordEntry passwordEntry)
        {
            using (var masterKey = await _masterKeyManager.Get())
            {
                var iv = AesKey.Generate().Iv;

                var newPassword = new PasswordModel(
                    iv,
                    await GetEncryptedSecureString(passwordEntry.Password, masterKey.Value, iv),
                    await _aesCrypter.Encrypt(Encoding.ASCII.GetBytes(passwordEntry.Description), masterKey.Value, iv),
                    await _aesCrypter.Encrypt(Encoding.ASCII.GetBytes(passwordEntry.UserName), masterKey.Value, iv));

                var currentData = await _fileStorage.Read();
                var updatedPasswords = currentData.Passwords.Append(newPassword);

                await _fileStorage.Store(
                    new StorageModel(currentData.EncryptedMasterKey, currentData.Iv, updatedPasswords.ToList()));

                return new PasswordEntry(newPassword.Id, passwordEntry.Password, passwordEntry.UserName, passwordEntry.Description);
            }
        }

        public async Task<IList<LazyPasswordEntry>> GetAll()
        {
            var data = await _fileStorage.Read();
            using (var masterKey = await _masterKeyManager.Get())
            {
                var passwords = new List<LazyPasswordEntry>();

                foreach (var p in data.Passwords)
                {
                    var decryptedName = await _aesCrypter.Decrypt(p.EncryptedUserName, masterKey.Value, p.Iv);
                    var decryptedDescription = await _aesCrypter.Decrypt(p.EncryptedDescription, masterKey.Value, p.Iv);

                    passwords.Add(new LazyPasswordEntry(
                        p.Id,
                        Encoding.ASCII.GetString(decryptedName),
                        Encoding.ASCII.GetString(decryptedDescription),
                        p.EncryptedPassword,
                        p.Iv,
                        _aesCrypter));
                }

                return passwords;
            }
        }

        public async Task<bool> Remove(Guid id)
        {
            var data = await _fileStorage.Read();
            var updatedPasswords = data.Passwords.Where(p => p.Id != id).ToArray();
            //if the item was found in passwords and removed count should have reduced
            if (data.Passwords.Count == updatedPasswords.Length)
                return false;

            await _fileStorage.Store(new StorageModel(data.EncryptedMasterKey, data.Iv, updatedPasswords));
            return true;
        }

        private async Task<byte[]> GetEncryptedSecureString(SecureString secureString, byte[] masterKey, byte[] iv)
        {
            var buffer = new byte[secureString.Length];
            var ptr = Marshal.SecureStringToGlobalAllocAnsi(secureString);
            try
            {
                Marshal.Copy(ptr, buffer, 0, buffer.Length);
                return await _aesCrypter.Encrypt(buffer, masterKey, iv);
            }
            finally
            {
                Helpers. ClearData(buffer);
                Marshal.ZeroFreeGlobalAllocAnsi(ptr);
            }
        }
    }
}
