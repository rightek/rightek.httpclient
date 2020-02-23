using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Rightek.HttpClient.Dtos;

namespace Rightek.HttpClient.Interfaces
{
    public interface IPost
    {
        Task<Response.Default> PostAsync(object data, CancellationToken cancellationToken = default);

        Task<Response.Default<T>> PostAsync<T>(object data, CancellationToken cancellationToken = default);

        Task<Response.Default> PostXmlAsync(string xml, CancellationToken cancellationToken = default);

        Task<Response.Default<T>> PostXmlAsync<T>(string xml, CancellationToken cancellationToken = default);

        Task<Response.Default> PostFormAsync(IEnumerable<KeyValuePair<string, string>> data, CancellationToken cancellationToken = default);

        Task<Response.Default<T>> PostFormAsync<T>(IEnumerable<KeyValuePair<string, string>> data, CancellationToken cancellationToken = default);
    }
}