using System;
using System.Threading.Tasks;

namespace Sample.Http.Strategy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("1 or timeout : Timeout sample");
                Console.WriteLine("2 or retry   : Retry sample");
                Console.WriteLine();
                Console.WriteLine("E.G.)");
                Console.WriteLine("$ dotnet run timeout");

                Console.WriteLine("Press Enter Key to exit.");
                Console.ReadLine();
            }
            else
            {
                var demoType = args[0];

                var demo = new Demo();
                try
                {
                    demo.Run(demoType)
                          .ContinueWith(t =>
                          {
                              if (t.Exception != null)
                              {
                                  Console.WriteLine($"Main -> Demo.Run Exception: {t.Exception.Message}");
                              }
                          })
                          .ContinueWith(t =>
                          {
                              if (t.IsCompleted)
                              {
                                  Console.WriteLine("Press Enter Key to exit.");
                                  Console.ReadLine();
                              }
                          })
                          .Wait();
                }
                catch (AggregateException ex)
                {
                    foreach (var e in ex.InnerExceptions)
                    {
                        Console.WriteLine($"Main -> Demo.Run Exception: {e.Message}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Main -> Demo.Run Exception: {ex.Message}");
                }
            }
        }
    }
}
