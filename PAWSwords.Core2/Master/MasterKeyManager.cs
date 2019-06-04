using PAWSwords.Core.Crypto;
using PAWSwords.Core.Interfaces;
using PAWSwords.Core.Storage;
using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PAWSwords.Core.Master
{
    public class MasterKeyManager : IMasterKeyManager
    {
        private readonly AesCrypter _aesCrypter;
        private readonly FileStorage _fileStorage;
        private byte[] _masterKey;

        public MasterKeyManager(AesCrypter aesCrypter, FileStorage fileStorage)
        {
            _aesCrypter = aesCrypter;
            _fileStorage = fileStorage;
        }

        public async Task Create(SecureString masterPassword)
        {
            using (var masterKey = AesKey.Generate())
            {
                using (var derivedKey = await Argon2Key.Calculate(masterPassword))
                {
                    var encryptedMasterKey = await _aesCrypter.Encrypt(masterKey.Key, derivedKey.Value, masterKey.Iv);

                    var storableModel = new StorageModel(encryptedMasterKey, (byte[])masterKey.Iv.Clone(), new List<PasswordModel>());
                    await _fileStorage.Store(storableModel);

                    _masterKey = new byte[masterKey.Key.Length];
                    masterKey.Key.CopyTo(_masterKey, 0);
                    ProtectedMemory.Protect(_masterKey, MemoryProtectionScope.SameProcess);
                }
            }
        }

        public Task<Key> Get()
        {
            if (_masterKey == null)
                throw new InvalidOperationException("Load method must be called before to initialize Get return value");

            try
            {
                ProtectedMemory.Unprotect(_masterKey, MemoryProtectionScope.SameProcess);
                byte[] temporaryDecryptedKey = new byte[_masterKey.Length];
                _masterKey.CopyTo(temporaryDecryptedKey, 0);
                return Task.FromResult(new Key(temporaryDecryptedKey));
            }
            finally
            {
                ProtectedMemory.Protect(_masterKey, MemoryProtectionScope.SameProcess);
            }
        }

        public async Task<bool> IsCreated()
        {
            try
            {
                await _fileStorage.Read();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task Load(SecureString masterPassword)
        {
            using (var derivedKey = await Argon2Key.Calculate(masterPassword))
            {
                var data = await _fileStorage.Read();

                var decryptedMasterKey = await _aesCrypter.Decrypt(data.EncryptedMasterKey, derivedKey.Value, data.Iv);
                _masterKey = decryptedMasterKey;
                ProtectedMemory.Protect(_masterKey, MemoryProtectionScope.SameProcess);
            }
        }
    }
}
