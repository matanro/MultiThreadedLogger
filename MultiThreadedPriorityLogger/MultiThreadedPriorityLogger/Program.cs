using System;
using System.Threading.Tasks;
using System.Threading;

namespace MultiThreadedPriorityLogger
{
    class Program
    {
        static ILoggerService _logger = new LoggerManager();

        static void Main(string[] args)
        {
            Console.WriteLine("---- Started Writing To Logger -----");
            SimulateLogging();
            Console.WriteLine("---- Finished Writing To Logger -----");
            Console.Read();
        }
        public static void SimulateLogging()
        {
            for (int i = 0; i < 10; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    var random = new Random();

                    for (int j = 0; j < 10; j++)
                    {
                        //Make random sevirities messages
                        var logSeverity = (LogType)random.Next(0, 6);
                        var message = $"ThreadId:{Thread.CurrentThread.ManagedThreadId}, {DateTime.Now.ToString("HH:mm:ss:fff")} This is a message of severity {logSeverity.ToString()}";
                        _logger.WriteLog(logSeverity, message);
                        Task.Delay(100);
                    }
                });
            }

            Task.WaitAll();
        }
    }
  
}
