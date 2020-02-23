using System;

namespace Rightek.HttpClient.Dtos
{
    public class CallbackArgs
    {
        public Settings Settings { get; set; }
        public Exception Exception { get; set; }
        public string ExceptionType { get; set; }
        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }
    }
}