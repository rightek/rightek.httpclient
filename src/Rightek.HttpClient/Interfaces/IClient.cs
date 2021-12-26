using System;
using System.Collections.Generic;

using Rightek.HttpClient.Dtos;

namespace Rightek.HttpClient.Interfaces
{
    public interface IClient
    {
        IRequest WithUri(string uri);

        IClient WithBasicAuth(string username, string password);

        IClient WithBearerToken(string bearerToken);

        IClient WithTimeout(TimeSpan timeout);

        IClient WithCookie(string name, string value);

        IClient WithCookies(IEnumerable<Cookie> cookies);

        IClient WithHeader(string key, object value);

        IClient WithHeaders(IDictionary<string, object> headers);

        IClient BeforeCall(Action action);

        IClient AfterCall(Action<CallbackArgs> action);

        IClient OnError(Action<CallbackArgs> action);

        IRequest Configure(Action<Settings> configure);

        void SetDefault(Action<Settings> configure);
    }
}