using System.Threading;
using System.Threading.Tasks;

using Rightek.HttpClient.Dtos;

namespace Rightek.HttpClient.Interfaces
{
    public interface IDownload
    {
        Task<Response.File> DownloadAsync(CancellationToken cancellationToken = default);
    }
}