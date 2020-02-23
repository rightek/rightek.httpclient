using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Rightek.HttpClient.Tests")]
namespace Rightek.HttpClient.Internals
{
    internal class Constants
    {
        public const string TIMEOUT_ERROR_MESSAGE = "Timeout should be more than zero.";
        public const string COOKIES_ERROR_MESSAGE = "Should be at least one cookie in Cookies.";
        public const string HEADERS_ERROR_MESSAGE = "Should be at least one cookie in Headers.";
        public const string BASE_ADDRESS_ERROR_MESSAGE = "Base address is not a valid url.";
        public const string BYTES_ERROR_MESSAGE = "You passed nothing to upload.";
    }
}