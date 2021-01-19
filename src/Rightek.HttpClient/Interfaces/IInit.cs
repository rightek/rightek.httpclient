using Rightek.HttpClient.Dtos;

namespace Rightek.HttpClient.Interfaces
{
    public interface IInit
    {
        IClient Init(Proxy proxy = null);
    }
}