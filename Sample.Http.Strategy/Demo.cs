using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

namespace Sample.Http.Strategy
{
    public class Demo
    {
        public async Task Run(string demoType)
        {
            DemoType t = DemoType.None;
            int tInt = 0;

            if (int.TryParse(demoType, out tInt))
            {
                t = (DemoType)tInt;
            }
            else
            {
                t = DemoType.None;
                //Console.WriteLine($"args[0]::{demoType}::");

                foreach (var name in Enum.GetNames(typeof(DemoType)))
                {
                    if (name.Equals(demoType, StringComparison.OrdinalIgnoreCase))
                    {
                        if (!Enum.TryParse<DemoType>(name, false, out t))
                        {
                            //Console.WriteLine($"Could not parse {demoType} to DemoType enum.");
                            t = DemoType.None;
                        }

                        break;
                    }
                }  

                //Console.WriteLine($"Converted DemoType::{t}::");
            }

            await Run(t);
        }

        public async Task Run(DemoType demoType)
        {
            Stopwatch watch = new Stopwatch();
            Console.WriteLine($"Demo.Run starts: { DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
            watch.Start();

            try
            {
                var uri = "http://bbon.me";

                using (HttpClient client = GetHttpClient(demoType))
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
                    catch (AggregateException ex)
                    {
                        foreach (var e in ex.InnerExceptions)
                        {
                            Console.WriteLine($"Demo.Run Exception: {e.Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Demo.Run: {ex.GetType().GetTypeInfo().FullName} {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Demo.Run: {ex.Message}");
            }
            finally
            {
                watch.Stop();
                Console.WriteLine($"Demo.Run ends: { DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                Console.WriteLine($"Demo.Run elapsed: { watch.Elapsed.TotalSeconds}");
            }
        }

        private HttpClient GetHttpClient(DemoType demoDescribe)
        {
            HttpClient client = null;
            HttpClientHandler clientHandler = new DelayHttpClientHandler(5);
            DelegatingHandler handler = null;
            switch (demoDescribe)
            {
                case DemoType.Timeout:
                    handler = new DefaultTestHandler(clientHandler);
                    break;
                case DemoType.Retry:
                    handler = new RetryHandler(clientHandler, 3);
                    break;
                default:
                    throw new NotSupportedException();
            }
            client = new HttpClient(handler);
            return client;
        }
    }
}
