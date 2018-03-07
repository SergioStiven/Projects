using System.IO;
using System.Threading.Tasks;

namespace Xpinn.SportsGo.Util.Portable.Abstract
{
    public interface IHttpClient
    {
        Task<TBack> PostAsync<TSend, TBack>(string requestUri, TSend entityToSend) where TSend : class where TBack : class;
        Task<TBack> PostAsync<TBack>(string requestUri, TBack entityToSend) where TBack : class;
        Task<TBack> PostAsync<TBack>(string requestUri) where TBack : class;
        Task<TBack> PostStreamAsync<TBack>(string requestUri, Stream entityToSend) where TBack : class;
        Task<Stream> GetStreamAsync(string requestUri);
    }
}
