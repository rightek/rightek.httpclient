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

        Task<Response.Default> PostAsync(object data, PostRequestType type = PostRequestType.JSON, CancellationToken cancellationToken = default);

        Task<Response.Default<T>> PostAsync<T>(object data, PostRequestType type = PostRequestType.JSON, CancellationToken cancellationToken = default);
    }
}