using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using FluentAssertions;

using Rightek.HttpClient.Dtos;
using Rightek.HttpClient.Internals;

using Xunit;

namespace Rightek.HttpClient.Tests
{
    public class ClientTests
    {
        public ClientTests()
        {
            Client.Instance.SetDefault(s =>
            {
                s.BaseAddress = "https://dl.dropboxusercontent.com/";
            });
        }

        [Fact]
        public void ThrowArgumentNullExceptionWhenUriIsEmpty()
        {
            var expectedParameterName = "uri";
            var uri = "";

            try
            {
                Client.Instance.WithUri(uri);
            }
            catch (ArgumentException ex)
            {
                ex.ParamName.Should().Be(expectedParameterName);
            }
        }

        [Fact]
        public void ThrowArgumentNullExceptionWhenUsernameIsEmpty()
        {
            var expectedParameterName = "username";
            var username = "";
            var password = "";

            try
            {
                Client.Instance.WithBasicAuth(username, password);
            }
            catch (ArgumentException ex)
            {
                ex.ParamName.Should().Be(expectedParameterName);
            }
        }

        [Fact]
        public void ThrowArgumentNullExceptionWhenPasswordIsEmpty()
        {
            var expectedParameterName = "password";
            var username = "username";
            var password = "";

            try
            {
                Client.Instance.WithBasicAuth(username, password);
            }
            catch (ArgumentException ex)
            {
                ex.ParamName.Should().Be(expectedParameterName);
            }
        }

        [Fact]
        public void ThrowArgumentNullExceptionWhenBearerTokenIsEmpty()
        {
            var expectedParameterName = "bearerToken";
            var bearerToken = "";

            try
            {
                Client.Instance.WithBearerToken(bearerToken);
            }
            catch (ArgumentException ex)
            {
                ex.ParamName.Should().Be(expectedParameterName);
            }
        }

        [Fact]
        public void ThrowArgumentExceptionWhenTimeoutIsZeroOrNegative()
        {
            try
            {
                Client.Instance.WithTimeout(TimeSpan.Zero);
            }
            catch (ArgumentException ex)
            {
                ex.Message.Should().Be(Constants.TIMEOUT_ERROR_MESSAGE);
            }

            try
            {
                Client.Instance.WithTimeout(TimeSpan.FromSeconds(-1));
            }
            catch (ArgumentException ex)
            {
                ex.Message.Should().Be(Constants.TIMEOUT_ERROR_MESSAGE);
            }
        }

        [Fact]
        public void ThrowArgumentNullExceptionWhenCookieNameIsEmpty()
        {
            var expectedParameterName = "name";
            var name = "";
            var value = "";

            try
            {
                Client.Instance.WithCookie(name, value);
            }
            catch (ArgumentException ex)
            {
                ex.ParamName.Should().Be(expectedParameterName);
            }
        }

        [Fact]
        public void ThrowArgumentNullExceptionWhenCookieValueIsEmpty()
        {
            var expectedParameterName = "value";
            var name = "name";
            var value = "";

            try
            {
                Client.Instance.WithCookie(name, value);
            }
            catch (ArgumentException ex)
            {
                ex.ParamName.Should().Be(expectedParameterName);
            }
        }

        [Fact]
        public void ThrowArgumentNullExceptionWhenCookiesIsNull()
        {
            var expectedParameterName = "cookies";

            try
            {
                Client.Instance.WithCookies(null);
            }
            catch (ArgumentException ex)
            {
                ex.ParamName.Should().Be(expectedParameterName);
            }
        }

        [Fact]
        public void ThrowArgumentExceptionWhenCookiesCountIsZero()
        {
            var cookies = new List<Cookie>();

            try
            {
                Client.Instance.WithCookies(cookies);
            }
            catch (ArgumentException ex)
            {
                ex.Message.Should().Be(Constants.COOKIES_ERROR_MESSAGE);
            }
        }

        [Fact]
        public void ThrowArgumentNullExceptionWhenHeadersIsNull()
        {
            var expectedParameterName = "headers";

            try
            {
                Client.Instance.WithHeaders(null);
            }
            catch (ArgumentException ex)
            {
                ex.ParamName.Should().Be(expectedParameterName);
            }
        }

        [Fact]
        public void ThrowArgumentExceptionWhenHeadersCountIsZero()
        {
            var headers = new Dictionary<string, object>();

            try
            {
                Client.Instance.WithHeaders(headers);
            }
            catch (ArgumentException ex)
            {
                ex.Message.Should().Be(Constants.HEADERS_ERROR_MESSAGE);
            }
        }

        [Fact]
        public async Task BasicGetShouldWorkWithCorrectUri()
        {
            Client.Instance.SetDefault(s => s.BaseAddress = null);

            var uri = "https://dl.dropboxusercontent.com/s/pqm5s3kx64q03fc/Rightek.HttpClient.json";

            var res = await Client.Instance.WithUri(uri).GetAsync<WhatTypeOfMusicDoYouLove>();

            res.IsSuccessful.Should().BeTrue();
            res.Response.answer.Should().Be("Trance Music");
        }

        [Fact]
        public async Task WithBasAddressGetShouldWork()
        {
            var uri = "s/pqm5s3kx64q03fc/Rightek.HttpClient.json";

            var res = await Client.Instance.WithUri(uri).GetAsync<WhatTypeOfMusicDoYouLove>();

            res.IsSuccessful.Should().BeTrue();
            res.Response.answer.Should().Be("Trance Music");
        }
    }

    public class WhatTypeOfMusicDoYouLove
    {
        public string answer { get; set; }
    }
}