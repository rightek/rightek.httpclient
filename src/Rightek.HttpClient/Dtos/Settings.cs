using System;
using System.Collections.Generic;

namespace Rightek.HttpClient.Dtos
{
    public class Settings
    {
        public string BaseAddress { get; set; }
        public TimeSpan Timeout { get; set; }
        public string UriPrefix { get; set; }
        public string Uri { get; set; }
        public Action BeforeCall { get; set; }
        public Action<CallbackArgs> AfterCall { get; set; }
        public Action<CallbackArgs> OnError { get; set; }
        public IDictionary<string, object> Headers { get; set; }
        public List<Cookie> Cookies { get; set; }
        public Auth Auth { get; set; }
    }
}