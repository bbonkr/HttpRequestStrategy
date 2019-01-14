응용프로그램을 개발할 때 외부 응용프로그램의 기능에 의존해야 하는 경우가 있습니다.

이 때, 외부 기능에 데이터를 전송하고, 실행결과를 전달받기 위해 HTTP 를 사용할 수 있습니다.

.NET Standard 에서는 HTTP 에서 요청, 응답을 처리하기 위한 [System.Net.Http](https://docs.microsoft.com/en-us/dotnet/api/system.net.http?view=netcore-2.2) 네임스페이스의 [HttpClient](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=netcore-2.2) 객체를 제공합니다.

## HTTP Request 시간제한

응답을 무한하게 대기해야 하는 경우가 아니면 Timeout 값을 지정해서 응답성을 향상시키기 위한 정책입니다.

기본값은 100,000 ms (100 s)입니다.

기본적으로 적용할 시간제한 정책을 수립합니다.

```csharp
using (HttpClient client = new HttpClient()){
    client.Timeout = TimeSpan.FromSeconds(3); // 시간제한 3초
}
```

## HTTP Request 재시도

Http 요청을 한번만 시도하고 실패로 처리할 것인지, 정해진 횟수만큼 시도하고 실패로 처리할 것인지에 대한 정책입니다.

[System.Net.Http](https://docs.microsoft.com/en-us/dotnet/api/system.net.http?view=netcore-2.2) 네임스페이스의 [DelegatingHandler](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.delegatinghandler?view=netcore-2.2) 객체를 확장해서 구현할 수 있습니다.

```c#
public class RetryHandler : DelegatingHandler
{
    public RetryHandler(HttpMessageHandler innerHandler, int maxRetries) : base(innerHandler)
    {
        this.maxRetries = maxRetries;
    }

    public RetryHandler(HttpMessageHandler innerHandler)
        : this(innerHandler, 3)
    { }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        HttpResponseMessage response = null;
        for (int i = 0; i < maxRetries; i++)
        {
            Console.WriteLine($"Try {i + 1} of {maxRetries}.");

            try
            {
                response = await base.SendAsync(request, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fail {i + 1} of {maxRetries}. {ex.GetType().GetTypeInfo().FullName} {ex.Message}");
            }
        }

        return response;
    }
    private readonly int maxRetries = 3;
}
```

사용예제

```c#
var uri = "http://google.com";

using (HttpClient client = new HttpClient(new RetryHandler(new HttpClientHandler())))
{
    client.Timeout = TimeSpan.FromSeconds(3);
    try
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
        var responseMessage = await client.SendAsync(requestMessage);
        if (responseMessage.IsSuccessStatusCode)
        {
            Console.WriteLine("Request succeed.");
        }
        else
        {
            Console.WriteLine("Request failed.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Demo.Run: {ex.GetType().GetTypeInfo().FullName} {ex.Message}");
        throw;
    }
}
```
