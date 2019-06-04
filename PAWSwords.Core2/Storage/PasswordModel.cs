using Newtonsoft.Json;
using System;

namespace PAWSwords.Core.Storage
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PasswordModel
    {
        [JsonConstructor]
        public PasswordModel(Guid id, byte[] iv, byte[] encryptedPassword, byte[] encryptedDescription, byte[] encryptedUserName)
        {
            Id = id;
            Iv = iv;
            EncryptedPassword = encryptedPassword;
            EncryptedDescription = encryptedDescription;
            EncryptedUserName = encryptedUserName;
        }

        public PasswordModel(byte[] iv, byte[] encryptedPassword, byte[] encryptedDescription, byte[] encryptedUserName)
            : this(Guid.NewGuid(), iv, encryptedPassword, encryptedDescription, encryptedUserName)
        {
        }

        [JsonProperty("id")]
        public Guid Id { get; }

        [JsonProperty("iv")]
        public byte[] Iv { get; }

        [JsonProperty("encryptedPassword")]
        public byte[] EncryptedPassword { get; }

        [JsonProperty("encryptedUserName")]
        public byte[] EncryptedUserName { get; }

        [JsonProperty("encryptedDescription")]
        public byte[] EncryptedDescription { get; }
    }
}
