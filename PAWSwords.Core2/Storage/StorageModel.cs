using Newtonsoft.Json;
using System.Collections.Generic;

namespace PAWSwords.Core.Storage
{
    [JsonObject(MemberSerialization.OptIn)]
    public class StorageModel
    {
        [JsonConstructor]
        public StorageModel(byte[] encryptedMasterKey, byte[] iv, IList<PasswordModel> passwords)
        {
            EncryptedMasterKey = encryptedMasterKey;
            Iv = iv;

            foreach (var item in passwords)
                Passwords.Add(item);
        }

        [JsonProperty("encryptedMasterKey")]
        public byte[] EncryptedMasterKey { get; }

        [JsonProperty("iv")]
        public byte[] Iv { get; }

        [JsonProperty("passwords")]
        public IList<PasswordModel> Passwords { get; } = new List<PasswordModel>();
    }
}
