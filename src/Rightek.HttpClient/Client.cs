using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Rightek.HttpClient.Dtos;
using Rightek.HttpClient.Enums;
using Rightek.HttpClient.Interfaces;
using Rightek.HttpClient.Internals;

using Cookie = Rightek.HttpClient.Dtos.Cookie;

namespace Rightek.HttpClient
{
    public sealed class Client : IInit, IClient, IRequest
    {
        static System.Net.Http.HttpClient _httpClient;
        static Settings _defaultSettings;

        Settings _requestSettings;

        public static IInit Instance { get; } = new Client();

        public IClient Init(Proxy proxy = null)
        {
            if (_httpClient != null) return this;

            var handler = new HttpClientHandler { UseCookies = false };
            if (handler.SupportsAutomaticDecompression) handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            if (proxy != null)
            {
                handler.Proxy = new WebProxy
                {
                    Address = new Uri($"http://{proxy.Host}:{proxy.Port}"),
                    BypassProxyOnLocal = false,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(userName: proxy.Username, password: proxy.Password)
                };
            }

            _httpClient = new System.Net.Http.HttpClient(handler);

            _defaultSettings = new Settings();

            return this;
        }

        public IRequest WithUri(string uri)
        {
            uri.ThrowIfNullOrEmpty(nameof(uri));

            if (_requestSettings == null) _requestSettings = new Settings();

            _requestSettings.Uri = uri;

            return this;
        }

        public IClient WithBasicAuth(string username, string password)
        {
            username.ThrowIfNullOrEmpty(nameof(username));
            password.ThrowIfNullOrEmpty(nameof(password));

            if (_requestSettings == null) _requestSettings = new Settings();

            _requestSettings.Auth = new Auth
            {
                AuthType = AuthType.BASIC,
                Username = username,
                Password = password
            };

            return this;
        }

        public IClient WithBearerToken(string bearerToken)
        {
            bearerToken.ThrowIfNullOrEmpty(nameof(bearerToken));

            if (_requestSettings == null) _requestSettings = new Settings();

            _requestSettings.Auth = new Auth
            {
                AuthType = AuthType.BEARER_TOKEN,
                BearerToken = bearerToken
            };

            return this;
        }

        public IClient WithTimeout(TimeSpan timeout)
        {
            if (timeout <= TimeSpan.Zero) throw new ArgumentException(Constants.TIMEOUT_ERROR_MESSAGE);

            if (_requestSettings == null) _requestSettings = new Settings();

            _requestSettings.Timeout = timeout;

            return this;
        }

        public IClient WithCookie(string name, string value)
        {
            name.ThrowIfNullOrEmpty(nameof(name));
            value.ThrowIfNullOrEmpty(nameof(value));

            if (_requestSettings == null) _requestSettings = new Settings();
            if (_requestSettings.Cookies == null) _requestSettings.Cookies = new List<Cookie>();

            _requestSettings.Cookies.Add(new Cookie { Name = name, Value = value });

            return this;
        }

        public IClient WithCookies(List<Cookie> cookies)
        {
            cookies.ThrowIfNull(nameof(cookies));
            if (cookies.Count == 0) throw new ArgumentException(Constants.COOKIES_ERROR_MESSAGE);

            if (_requestSettings == null) _requestSettings = new Settings();

            _requestSettings.Cookies = cookies;

            return this;
        }

        public IClient WithHeader(string key, object value)
        {
            key.ThrowIfNullOrEmpty(nameof(key));
            value.ThrowIfNull(nameof(value));

            if (_requestSettings == null) _requestSettings = new Settings();
            if (_requestSettings.Headers == null) _requestSettings.Headers = new Dictionary<string, object>();

            _requestSettings.Headers.Add(key, value);

            return this;
        }

        public IClient WithHeaders(IDictionary<string, object> headers)
        {
            headers.ThrowIfNull(nameof(headers));
            if (headers.Count == 0) throw new ArgumentException(Constants.HEADERS_ERROR_MESSAGE);

            if (_requestSettings == null) _requestSettings = new Settings();

            _requestSettings.Headers = headers;

            return this;
        }

        public IClient BeforeCall(Action action)
        {
            action.ThrowIfNull(nameof(action));

            if (_requestSettings == null) _requestSettings = new Settings();

            _requestSettings.BeforeCall = action;

            return this;
        }

        public IClient AfterCall(Action<CallbackArgs> action)
        {
            action.ThrowIfNull(nameof(action));

            if (_requestSettings == null) _requestSettings = new Settings();

            _requestSettings.AfterCall = action;

            return this;
        }

        public IClient OnError(Action<CallbackArgs> action)
        {
            action.ThrowIfNull(nameof(action));

            if (_requestSettings == null) _requestSettings = new Settings();

            _requestSettings.OnError = action;

            return this;
        }

        public IRequest Configure(Action<Settings> configure)
        {
            _requestSettings = new Settings();

            _requestSettings.Headers = new Dictionary<string, object>();
            _requestSettings.Cookies = new List<Cookie>();

            configure(_requestSettings);

            return this;
        }

        public void SetDefault(Action<Settings> configure)
        {
            _defaultSettings.Headers = new Dictionary<string, object>();
            _defaultSettings.Cookies = new List<Cookie>();

            configure(_defaultSettings);

            // set base address
            if (!string.IsNullOrWhiteSpace(_defaultSettings.BaseAddress))
            {
                if (_defaultSettings.BaseAddress.IsValidUri())
                {
                    var baseAddress = _defaultSettings.BaseAddress;

                    // you must place a slash at the end of the BaseAddress
                    // ref: https://stackoverflow.com/a/23438417/3367974
                    if (!baseAddress.EndsWith("/")) baseAddress += "/";

                    _httpClient.BaseAddress = new Uri(baseAddress);
                }
                else throw new ArgumentException(Constants.BASE_ADDRESS_ERROR_MESSAGE);
            }
        }

        public async Task<Response.Default> GetAsync(CancellationToken cancellationToken = default)
        {
            var settings = Util.ResolveSettings(_defaultSettings, _requestSettings);

            Util.PreValidate(settings);

            var req = Util.GetRequest(HttpMethod.Get, settings);

            if (settings.Timeout > TimeSpan.Zero) _httpClient.Timeout = settings.Timeout;

            return await sendAsync(settings, req, cancellationToken);
        }

        public async Task<Response.Default<T>> GetAsync<T>(CancellationToken cancellationToken = default)
        {
            var settings = Util.ResolveSettings(_defaultSettings, _requestSettings);

            Util.PreValidate(settings);

            var req = Util.GetRequest(HttpMethod.Get, settings);

            if (settings.Timeout > TimeSpan.Zero) _httpClient.Timeout = settings.Timeout;

            return await sendAsync<T>(settings, req, cancellationToken);
        }

        public async Task<Response.Default> PostAsync(object data, CancellationToken cancellationToken = default)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var settings = Util.ResolveSettings(_defaultSettings, _requestSettings);

            Util.PreValidate(settings);

            var req = Util.GetRequest(HttpMethod.Post, settings);
            req.Content = new StringContent(Util.Serialize(data), Encoding.UTF8, "application/json");

            if (settings.Timeout > TimeSpan.Zero) _httpClient.Timeout = settings.Timeout;

            return await sendAsync(settings, req, cancellationToken);
        }

        public async Task<Response.Default<T>> PostAsync<T>(object data, CancellationToken cancellationToken = default)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var settings = Util.ResolveSettings(_defaultSettings, _requestSettings);

            Util.PreValidate(settings);

            var req = Util.GetRequest(HttpMethod.Post, settings);
            req.Content = new StringContent(Util.Serialize(data), Encoding.UTF8, "application/json");

            if (settings.Timeout > TimeSpan.Zero) _httpClient.Timeout = settings.Timeout;

            return await sendAsync<T>(settings, req, cancellationToken);
        }

        public async Task<Response.Default> PostXmlAsync(string xml, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(xml)) throw new ArgumentNullException(nameof(xml));

            var settings = Util.ResolveSettings(_defaultSettings, _requestSettings);

            Util.PreValidate(settings);

            var req = Util.GetRequest(HttpMethod.Post, settings);
            req.Content = new StringContent(xml, Encoding.UTF8, "text/xml");

            if (settings.Timeout > TimeSpan.Zero) _httpClient.Timeout = settings.Timeout;

            return await sendAsync(settings, req, cancellationToken);
        }

        public async Task<Response.Default<T>> PostXmlAsync<T>(string xml, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(xml)) throw new ArgumentNullException(nameof(xml));

            var settings = Util.ResolveSettings(_defaultSettings, _requestSettings);

            Util.PreValidate(settings);

            var req = Util.GetRequest(HttpMethod.Post, settings);
            req.Content = new StringContent(xml, Encoding.UTF8, "text/xml");

            if (settings.Timeout > TimeSpan.Zero) _httpClient.Timeout = settings.Timeout;

            return await sendAsync<T>(settings, req, cancellationToken);
        }

        public async Task<Response.Default> PostFormAsync(IEnumerable<KeyValuePair<string, string>> data, CancellationToken cancellationToken = default)
        {
            data.ThrowIfNull(nameof(data));

            var settings = Util.ResolveSettings(_defaultSettings, _requestSettings);

            Util.PreValidate(settings);

            var req = Util.GetRequest(HttpMethod.Post, settings);
            req.Content = new FormUrlEncodedContent(data);

            if (settings.Timeout > TimeSpan.Zero) _httpClient.Timeout = settings.Timeout;

            return await sendAsync(settings, req, cancellationToken);
        }

        public async Task<Response.Default<T>> PostFormAsync<T>(IEnumerable<KeyValuePair<string, string>> data, CancellationToken cancellationToken = default)
        {
            data.ThrowIfNull(nameof(data));

            var settings = Util.ResolveSettings(_defaultSettings, _requestSettings);

            Util.PreValidate(settings);

            var req = Util.GetRequest(HttpMethod.Post, settings);
            req.Content = new FormUrlEncodedContent(data);

            if (settings.Timeout > TimeSpan.Zero) _httpClient.Timeout = settings.Timeout;

            return await sendAsync<T>(settings, req, cancellationToken);
        }

        public async Task<Response.File> DownloadAsync(CancellationToken cancellationToken = default)
        {
            var settings = Util.ResolveSettings(_defaultSettings, _requestSettings);

            Util.PreValidate(settings);

            var req = Util.GetRequest(HttpMethod.Get, settings);

            if (settings.Timeout > TimeSpan.Zero) _httpClient.Timeout = settings.Timeout;

            try
            {
                settings.BeforeCall?.Invoke();

                var startAt = DateTime.Now;
                var res = await _httpClient.SendAsync(req, cancellationToken);
                var endAt = DateTime.Now;

                settings.AfterCall?.Invoke(new CallbackArgs
                {
                    Settings = settings,
                    StartAt = startAt,
                    EndAt = endAt
                });

                return new Response.File
                {
                    IsSuccessful = true,
                    StatusCode = res.StatusCode,
                    Bytes = await res.Content.ReadAsByteArrayAsync()
                };
            }
            catch (TimeoutException ex)
            {
                settings.OnError?.Invoke(new CallbackArgs
                {
                    Exception = ex
                });

                return new Response.File
                {
                    IsSuccessful = false,
                    StatusCode = HttpStatusCode.RequestTimeout
                };
            }
            catch (Exception ex)
            {
                settings.OnError?.Invoke(new CallbackArgs
                {
                    Exception = ex
                });

                return new Response.File
                {
                    IsSuccessful = false,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
            finally
            {
                _requestSettings = null;
            }
        }

        public async Task<Response.Default<T>> UploadAsync<T>(byte[] bytes, string fileName = null, object data = null, CancellationToken cancellationToken = default)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            if (bytes.Length == 0) throw new ArgumentException(Constants.BYTES_ERROR_MESSAGE);

            var settings = Util.ResolveSettings(_defaultSettings, _requestSettings);

            Util.PreValidate(settings);

            var req = Util.GetRequest(HttpMethod.Post, settings);

            fileName = string.IsNullOrWhiteSpace(fileName) ? DateTime.Now.ToString("yyyyMMdd-HHmmss") : fileName;

            var mfdc = new MultipartFormDataContent
            {
                { new StringContent(Convert.ToBase64String(bytes), Encoding.Default, "application/octet-stream"), "file", fileName }
            };

            if (data != null)
            {
                mfdc.Add(new StringContent(Util.Serialize(data), Encoding.UTF8, "application/json"));
            }

            req.Content = mfdc;

            if (settings.Timeout > TimeSpan.Zero) _httpClient.Timeout = settings.Timeout;

            return await sendAsync<T>(settings, req, cancellationToken);
        }

        #region private members

        async Task<Response.Default> sendAsync(Settings settings, HttpRequestMessage req, CancellationToken cancellationToken)
        {
            try
            {
                settings.BeforeCall?.Invoke();

                var startAt = DateTime.Now;
                var res = await _httpClient.SendAsync(req, cancellationToken);
                var endAt = DateTime.Now;

                settings.AfterCall?.Invoke(new CallbackArgs
                {
                    Settings = settings,
                    StartAt = startAt,
                    EndAt = endAt
                });

                return new Response.Default
                {
                    IsSuccessful = res.IsSuccessStatusCode,
                    StatusCode = res.StatusCode
                };
            }
            catch (TimeoutException ex)
            {
                return handleException(ex, HttpStatusCode.RequestTimeout);
            }
            catch (Exception ex)
            {
                return handleException(ex, HttpStatusCode.InternalServerError);
            }
            finally
            {
                _requestSettings = null;
            }
        }

        async Task<Response.Default<T>> sendAsync<T>(Settings settings, HttpRequestMessage req, CancellationToken cancellationToken)
        {
            try
            {
                settings.BeforeCall?.Invoke();

                var startAt = DateTime.Now;
                var res = await _httpClient.SendAsync(req, cancellationToken);
                var endAt = DateTime.Now;

                settings.AfterCall?.Invoke(new CallbackArgs
                {
                    Settings = settings,
                    StartAt = startAt,
                    EndAt = endAt
                });

                using (var s = await res.Content.ReadAsStreamAsync())
                {
                    using (var sr = new StreamReader(s, Encoding.UTF8))
                    {
                        var responseText = sr.ReadToEnd();

                        if (res.IsSuccessStatusCode)
                        {
                            return new Response.Default<T>
                            {
                                IsSuccessful = true,
                                Response = typeof(T) == typeof(string) || typeof(T).IsValueType
                                    ? (T)(object)responseText
                                    : Util.Deserialize<T>(responseText),
                                StatusCode = res.StatusCode,
                                RawResponse = responseText
                            };
                        }

                        return new Response.Default<T>
                        {
                            IsSuccessful = false,
                            StatusCode = res.StatusCode,
                            RawResponse = responseText
                        };
                    }
                }
            }
            catch (TimeoutException ex)
            {
                return handleException<T>(ex, HttpStatusCode.RequestTimeout);
            }
            catch (Exception ex)
            {
                return handleException<T>(ex, HttpStatusCode.InternalServerError);
            }
            finally
            {
                _requestSettings = null;
            }
        }

        Response.Default handleException(Exception ex, HttpStatusCode statusCode)
        {
            _requestSettings.OnError?.Invoke(new CallbackArgs
            {
                Exception = ex,
                ExceptionType = ex.GetType().Name,
                Settings = _requestSettings
            });

            return new Response.Default
            {
                IsSuccessful = false,
                StatusCode = statusCode
            };
        }

        Response.Default<T> handleException<T>(Exception ex, HttpStatusCode statusCode)
        {
            _requestSettings.OnError?.Invoke(new CallbackArgs
            {
                Exception = ex,
                ExceptionType = ex.GetType().Name,
                Settings = _requestSettings
            });

            return new Response.Default<T>
            {
                IsSuccessful = false,
                StatusCode = statusCode
            };
        }

        #endregion private members
    }
}