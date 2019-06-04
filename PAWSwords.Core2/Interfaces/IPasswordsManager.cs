using PAWSwords.Core.Passwords;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PAWSwords.Core.Interfaces
{
    public interface IPasswordsManager
    {
        Task<PasswordEntry> Add(PasswordEntry password);
        Task<IList<LazyPasswordEntry>> GetAll();
        Task<bool> Remove(Guid id);
    }
}

