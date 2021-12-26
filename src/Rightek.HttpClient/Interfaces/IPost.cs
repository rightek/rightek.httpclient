using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Rightek.HttpClient.Dtos;
using Rightek.HttpClient.Enums;

namespace Rightek.HttpClient.Interfaces
{
    public interface IPost
    {
        Task<Response.Default> PostAsync(CancellationToken cancellationToken = default);

        Task<Response.Default<T>> PostAsync<T>(CancellationToken cancellationToken = default);

        Task<Response.Default> PostFormAsync(IDictionary<string, string> data, CancellationToken cancellationToken = default);

        Task<Response.Default> PostXmlAsync(string xml, CancellationToken cancellationToken = default);

        Task<Response.Default> PostByteArrayAsync(byte[] bytes, CancellationToken cancellationToken = default);

        Task<Response.Default> PostJsonAsync(object data, CancellationToken cancellationToken = default);

        Task<Response.Default> PostAsync(object data, PostRequestType type = PostRequestType.JSON, CancellationToken cancellationToken = default);

        Task<Response.Default<T>> PostFormAsync<T>(IDictionary<string, string> data, CancellationToken cancellationToken = default);

        Task<Response.Default<T>> PostXmlAsync<T>(string xml, CancellationToken cancellationToken = default);

        Task<Response.Default<T>> PostByteArrayAsync<T>(byte[] bytes, CancellationToken cancellationToken = default);

        Task<Response.Default<T>> PostJsonAsync<T>(object data, CancellationToken cancellationToken = default);

        Task<Response.Default<T>> PostAsync<T>(object data, PostRequestType type = PostRequestType.JSON, CancellationToken cancellationToken = default);
    }
}