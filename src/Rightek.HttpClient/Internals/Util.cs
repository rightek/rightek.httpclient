using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

using Rightek.HttpClient.Dtos;
using Rightek.HttpClient.Enums;

namespace Rightek.HttpClient.Internals
{
    internal static class Util
    {
        public static void ThrowIfNull<T>(this T o, string paramName) where T : class
        {
            if (o is null) throw new ArgumentNullException(paramName);
        }

        public static void ThrowIfNullOrEmpty(this string s, string paramName)
        {
            if (string.IsNullOrWhiteSpace(s)) throw new ArgumentNullException(paramName);
        }

        public static bool IsValidUri(this string uri)
        {
            try
            {
                new Uri(uri);
            }
            catch (UriFormatException)
            {
                return false;
            }

            return true;
        }

        public static void PreValidate(Settings requestSettings)
        {
            requestSettings.Uri.ThrowIfNull("Uri");
        }

        public static HttpRequestMessage GetRequest(HttpMethod method, Settings settings)
        {
            var requestUri = settings.Uri.IsValidUri()
                // uri is absolute, so adding prefix doesn't make sense
                ? settings.Uri
                // uri is a relative
                : $"{settings.UriPrefix}{settings.Uri}";

            // you must place a slash at the end of the BaseAddress, and you must not place a slash at the beginning of your relative URI
            // ref: https://stackoverflow.com/a/23438417/3367974
            if (!string.IsNullOrWhiteSpace(settings.BaseAddress) && requestUri.StartsWith("/")) requestUri = requestUri.Substring(1);

            var req = new HttpRequestMessage(method, requestUri);

            // headers
            if (settings.Headers != null)
            {
                foreach (var h in settings.Headers.Where(c => c.Key != "Content-Type")) req.Headers.Add(h.Key, Convert.ToString(h.Value));
            }
            // cookies
            if (settings.Cookies != null)
            {
                req.Headers.Add("Cookie", string.Join(";", settings.Cookies.Select(c => $"{c.Name}={c.Value}")));
            }
            // auth
            if (settings.Auth != null)
            {
                if (settings.Auth.AuthType == AuthType.BASIC)
                {
                    req.Headers.Authorization = new AuthenticationHeaderValue(
                        "Basic",
                        Convert.ToBase64String(Encoding.ASCII.GetBytes($"{settings.Auth.Username}:{settings.Auth.Password}"))
                    );
                }
                else
                {
                    req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.Auth.BearerToken);
                }
            }

            return req;
        }

        public static Settings ResolveSettings(Settings defaultSettings, Settings requestSettings)
        {
            return new Settings
            {
                BaseAddress = defaultSettings?.BaseAddress,
                UriPrefix = defaultSettings?.UriPrefix,
                AfterCall = requestSettings?.AfterCall ?? defaultSettings?.AfterCall,
                Auth = requestSettings?.Auth ?? defaultSettings?.Auth,
                BeforeCall = requestSettings?.BeforeCall ?? defaultSettings?.BeforeCall,
                OnError = requestSettings?.OnError ?? defaultSettings?.OnError,
                Timeout = (requestSettings?.Timeout ?? defaultSettings?.Timeout) ?? TimeSpan.Zero,
                Uri = requestSettings?.Uri ?? defaultSettings?.Uri,
                Cookies = requestSettings?.Cookies?.Count > 0
                    ? requestSettings.Cookies
                    : defaultSettings?.Cookies,
                Headers = requestSettings?.Headers?.Count > 0
                    ? requestSettings.Headers
                    : defaultSettings?.Headers
            };
        }

        public static T Deserialize<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return default;

            return JsonSerializer.Deserialize<T>(json);
        }

        public static string Serialize(object o)
        {
            if (o == null) return default;

            return JsonSerializer.Serialize(o);
        }

        public static string GetContentType(PostRequestType type)
        {
            if (type == PostRequestType.XML) return "text/xml";

            return "application/json";
        }
    }
}