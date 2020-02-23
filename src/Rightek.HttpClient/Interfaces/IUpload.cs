using System.Threading;
using System.Threading.Tasks;

using Rightek.HttpClient.Dtos;

namespace Rightek.HttpClient.Interfaces
{
    public interface IUpload
    {
        Task<Response.Default<T>> UploadAsync<T>(byte[] bytes, string fileName = null, object data = null, CancellationToken cancellationToken = default);
    }
}