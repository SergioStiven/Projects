using System.Threading.Tasks;

namespace Xpinn.SportsGo.Util.Portable
{
    public interface ISecureableMessage
    {
        Task<string> EncryptEntity<TEntity>(TEntity entidad, bool useHashing = true) where TEntity : class;
        Task<TEntity> DecryptMessageToEntity<TEntity>(string messageToDecrypt, bool useHashing = true) where TEntity : class;
        string EncryptJson(string jsonEntity, bool useHashing = true);
        string DecryptJson(string jsonEntity, bool useHashing = true);
    }
}
