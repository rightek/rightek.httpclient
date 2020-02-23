using System.Threading;
using System.Threading.Tasks;

using Rightek.HttpClient.Dtos;

namespace Rightek.HttpClient.Interfaces
{
    public interface IGet
    {
        Task<Response.Default> GetAsync(CancellationToken cancellationToken = default);

        Task<Response.Default<T>> GetAsync<T>(CancellationToken cancellationToken = default);
    }
}