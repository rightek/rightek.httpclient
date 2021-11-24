using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;

using Rightek.HttpClient.Dtos;
using Rightek.HttpClient.Internals;

using Xunit;

namespace Rightek.HttpClient.UnitTests
{
    public class ClientTests
    {
        public ClientTests()
        {
            Client.Instance.Init().SetDefault(s =>
            {
                s.BaseAddress = "https://dl.dropboxusercontent.com/";
            });
        }

        [Fact]
        public void Throw_argument_null_exception_when_uri_is_empty()
        {
            var expectedParameterName = "uri";
            var uri = "";

            try
            {
                Client.Instance.Init().WithUri(uri);
            }
            catch (ArgumentException ex)
            {
                ex.ParamName.Should().Be(expectedParameterName);
            }
        }

        [Fact]
        public void Throw_argument_null_exception_when_username_is_empty()
        {
            var expectedParameterName = "username";
            var username = "";
            var password = "";

            try
            {
                Client.Instance.Init().WithBasicAuth(username, password);
            }
            catch (ArgumentException ex)
            {
                ex.ParamName.Should().Be(expectedParameterName);
            }
        }

        [Fact]
        public void Throw_argument_null_exception_when_password_is_empty()
        {
            var expectedParameterName = "password";
            var username = "username";
            var password = "";

            try
            {
                Client.Instance.Init().WithBasicAuth(username, password);
            }
            catch (ArgumentException ex)
            {
                ex.ParamName.Should().Be(expectedParameterName);
            }
        }

        [Fact]
        public void Throw_argument_null_exception_when_bearer_token_is_empty()
        {
            var expectedParameterName = "bearerToken";
            var bearerToken = "";

            try
            {
                Client.Instance.Init().WithBearerToken(bearerToken);
            }
            catch (ArgumentException ex)
            {
                ex.ParamName.Should().Be(expectedParameterName);
            }
        }

        [Fact]
        public void Throw_argument_exception_when_timeout_is_zero_or_negative()
        {
            try
            {
                Client.Instance.Init().WithTimeout(TimeSpan.Zero);
            }
            catch (ArgumentException ex)
            {
                ex.Message.Should().Be(Constants.TIMEOUT_ERROR_MESSAGE);
            }

            try
            {
                Client.Instance.Init().WithTimeout(TimeSpan.FromSeconds(-1));
            }
            catch (ArgumentException ex)
            {
                ex.Message.Should().Be(Constants.TIMEOUT_ERROR_MESSAGE);
            }
        }

        [Fact]
        public void Throw_argument_null_exception_when_cookie_name_is_empty()
        {
            var expectedParameterName = "name";
            var name = "";
            var value = "";

            try
            {
                Client.Instance.Init().WithCookie(name, value);
            }
            catch (ArgumentException ex)
            {
                ex.ParamName.Should().Be(expectedParameterName);
            }
        }

        [Fact]
        public void Throw_argument_null_exception_when_cookie_value_is_empty()
        {
            var expectedParameterName = "value";
            var name = "name";
            var value = "";

            try
            {
                Client.Instance.Init().WithCookie(name, value);
            }
            catch (ArgumentException ex)
            {
                ex.ParamName.Should().Be(expectedParameterName);
            }
        }

        [Fact]
        public void Throw_argument_null_exception_when_cookies_is_null()
        {
            var expectedParameterName = "cookies";

            try
            {
                Client.Instance.Init().WithCookies(null);
            }
            catch (ArgumentException ex)
            {
                ex.ParamName.Should().Be(expectedParameterName);
            }
        }

        [Fact]
        public void Throw_argument_exception_when_cookies_count_is_zero()
        {
            var cookies = new List<Cookie>();

            try
            {
                Client.Instance.Init().WithCookies(cookies);
            }
            catch (ArgumentException ex)
            {
                ex.Message.Should().Be(Constants.COOKIES_ERROR_MESSAGE);
            }
        }

        [Fact]
        public void Throw_argument_null_exception_when_headers_is_null()
        {
            var expectedParameterName = "headers";

            try
            {
                Client.Instance.Init().WithHeaders(null);
            }
            catch (ArgumentException ex)
            {
                ex.ParamName.Should().Be(expectedParameterName);
            }
        }

        [Fact]
        public void Throw_argument_exception_when_headers_count_is_zero()
        {
            var headers = new Dictionary<string, object>();

            try
            {
                Client.Instance.Init().WithHeaders(headers);
            }
            catch (ArgumentException ex)
            {
                ex.Message.Should().Be(Constants.HEADERS_ERROR_MESSAGE);
            }
        }

        [Fact]
        public async Task Get_should_work_with_correct_uri()
        {
            Client.Instance.Init().SetDefault(s => s.BaseAddress = null);

            var uri = "https://dl.dropboxusercontent.com/s/pqm5s3kx64q03fc/Rightek.HttpClient.json";

            var res = await Client.Instance.Init().WithUri(uri).GetAsync<WhatTypeOfMusicDoYouLove>();

            res.IsSuccessful.Should().BeTrue();
            res.Response.answer.Should().Be("Trance Music");
        }

        [Fact]
        public async Task Get_should_work_with_base_address()
        {
            var uri = "s/pqm5s3kx64q03fc/Rightek.HttpClient.json";

            var res = await Client.Instance.Init().WithUri(uri).GetAsync<WhatTypeOfMusicDoYouLove>();

            res.IsSuccessful.Should().BeTrue();
            res.Response.answer.Should().Be("Trance Music");
        }
    }

    public class WhatTypeOfMusicDoYouLove
    {
        public string answer { get; set; }
    }
}