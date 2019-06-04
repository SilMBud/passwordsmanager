using PAWSwords.Core.Master;
using System.Security;
using System.Threading.Tasks;

namespace PAWSwords.Core.Interfaces
{
    public interface IMasterKeyManager
    {
        Task<bool> IsCreated();

        Task Create(SecureString password);

        Task<Key> Get();

        Task Load(SecureString password);
    }
}
