# Rightek.HttpClient
Simple wrapper around `System.Net.Http.HttpClient`

> Rightek.HttpClient has no dependency to JSON.net and uses the new built-in JSON api from Microsoft, `System.Text.Json`. this new JSON api is powerfull and fast, that's why Rightek.HttpClient doesn't provide any way to use another JSON serializer.

## Basic Usage
```cs
public class WhatTypeOfMusicDoYouLove
{
    public string answer { get; set; }
}
```
```cs
var uri = "https://dl.dropboxusercontent.com/s/pqm5s3kx64q03fc/Rightek.HttpClient.json";
var res = await Client.Instance
    .Init()
    .WithUri(uri)
    .GetAsync<WhatTypeOfMusicDoYouLove>();

if (res.IsSuccessful)
{
    var answer = res.Response.answer;
    // answer should be "Trance Music"
}
else
{
    // why you looking at me? it's not Rightek.HttpClient's fault
}
```

## Nuget [![nuget](https://img.shields.io/nuget/v/Rightek.HttpClient.svg?color=%23268bd2&style=flat-square)](https://www.nuget.org/packages/Rightek.HttpClient) [![stats](https://img.shields.io/nuget/dt/Rightek.HttpClient.svg?color=%2382b414&style=flat-square)](https://www.nuget.org/stats/packages/Rightek.HttpClient?groupby=Version)

`PM> Install-Package Rightek.HttpClient`

## Config API

- `Init`:  Initial HttpClient, here you can configure the proxy
- `WithUri`:  Request uri
- `WithBasicAuth`:  Basic authentication mode
- `WithBearerToken`: Bearer token authentication mode
- `WithTimeout`: Request timeout
- `WithCookie`: Request cookie
- `WithCookies`: Request cookies
- `WithHeader`: Request header
- `WithHeaders`: Request headers
- `BeforeCall`: Before request callback
- `AfterCall`: After request callback
- `OnError`: Error callback
- `Configure`: You can use this method instead of all above methods and set all configs at once
- `SetDefault`: You should call it once at application startup, default setting can be override using above methods

## Core API

- `DownloadAsync`: Download file
- `UploadAsync`: Upload file
- `UploadAsync<T>`: Upload file
- `GetAsync`: Get http request
- `GetAsync<T>`: Get http request
- `PostFormAsync`: Post http request
- `PostXmlAsync`: Post http request
- `PostByteArrayAsync`: Post http request
- `PostJsonAsync`: Post http request
- `PostAsync`: Post http request (No data, JSON, XML, Form, Byte Array)
- `PostFormAsync<T>`: Post http request
- `PostXmlAsync<T>`: Post http request
- `PostByteArrayAsync<T>`: Post http request
- `PostJsonAsync<T>`: Post http request
- `PostAsync<T>`: Post http request (No data, JSON, XML, Form, Byte Array)

## License
MIT

---
Made with ♥ by people @ [Rightek](http://rightek.ir)
