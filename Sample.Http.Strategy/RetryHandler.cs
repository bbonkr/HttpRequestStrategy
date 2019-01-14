using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

namespace Sample.Http.Strategy
{
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
                catch (AggregateException ex)
                {
                    foreach (var e in ex.InnerExceptions)
                    {
                        Console.WriteLine($"{e.GetType().GetTypeInfo().FullName} {e.Message}");
                    }
                }
                catch (TaskCanceledException ex)
                {
                    Console.WriteLine($"Fail {i + 1} of {maxRetries}. {ex.GetType().GetTypeInfo().FullName} {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"!!Fail {i + 1} of {maxRetries}. {ex.GetType().GetTypeInfo().FullName} {ex.Message}");
                }

                if (response != null)
                {
                    break;
                }
            }

            return response;
        }

        private readonly int maxRetries = 3;
    }
}
