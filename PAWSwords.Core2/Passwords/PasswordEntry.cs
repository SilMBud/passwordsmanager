using System;
using System.Security;

namespace PAWSwords.Core.Passwords
{
    public class PasswordEntry
    {
        public PasswordEntry(Guid id, SecureString password, string userName, string description) : this(password, userName, description)
        {
            Id = id;
        }

        public PasswordEntry(SecureString password, string userName, string description)
        {
            Password = password;
            UserName = userName;
            Description = description;
        }

        public Guid? Id { get; }

        public SecureString Password { get; }

        public string UserName { get; }

        public string Description { get; }
    }
}
