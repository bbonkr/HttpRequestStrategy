using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Http.Strategy
{
    public class DefaultTestHandler : DelegatingHandler
    {
        public DefaultTestHandler(HttpMessageHandler innerHandler) 
            : base(innerHandler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;

            response = await base.SendAsync(request, cancellationToken);

            return response;
        }
    }
}
