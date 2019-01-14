using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace Sample.Http.Strategy
{
    public class DelayHttpClientHandler : HttpClientHandler
    {
        public DelayHttpClientHandler(int delaySeconds)
        {
            this.delaySeconds = delaySeconds;
        }

        public DelayHttpClientHandler() : this(3)
        {

        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));

            return await base.SendAsync(request, cancellationToken);
        }

        private readonly int delaySeconds = 3;
    }
}
