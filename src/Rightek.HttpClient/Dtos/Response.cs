using System.Net;

namespace Rightek.HttpClient.Dtos
{
    public class Response
    {
        public class Default
        {
            public bool IsSuccessful { get; set; }
            public HttpStatusCode StatusCode { get; set; }
            public string RawResponse { get; set; }
        }

        public class Default<T> : Default
        {
            public T Response { get; set; }
        }

        public class File : Default
        {
            public byte[] Bytes { get; set; }
        }
    }
}