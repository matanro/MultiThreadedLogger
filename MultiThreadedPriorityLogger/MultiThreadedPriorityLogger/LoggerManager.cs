using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MultiThreadedPriorityLogger
{
    public class LoggerManager : ILoggerService
    {
        #region Fields
        private static  Dictionary<LogType, BlockingCollection<string>> _LoggerManager = new Dictionary<LogType, BlockingCollection<string>>();
        private readonly string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly object _locker = new object();
        private Timer timer;
        #endregion

        #region Constructors
        public LoggerManager()
        {
            //setting up a timer that will print all the logs once in half seconds
            timer = new Timer(500);
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
            timer.Start();
        }
        #endregion

        #region Public Methods and Interface Implementaion
        public void WriteLog(LogType sevirity, string message)
        {
            try
            {
                // Lock each adding to avoid duplicates and errors
                lock (_locker)
                {

                    if (_LoggerManager.ContainsKey(sevirity) && !_LoggerManager[sevirity].Contains(message))
                    {
                        _LoggerManager[sevirity].Add(message);
                    }
                    else if (!_LoggerManager.ContainsKey(sevirity))
                    {
                        _LoggerManager.Add(sevirity, new BlockingCollection<string>());
                        _LoggerManager[sevirity].Add(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        #endregion

        #region Private Methods Area
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            PrintToFile();
        }

        private void PrintToFile()
        {
            try
            {
                //Get the sorted Sevirities of the errors the dictionary 
                var Sevirites = _LoggerManager.Keys.OrderBy(s => s).Reverse().ToList();
                foreach (LogType sev in Sevirites)
                {
                    //Print Message and remove it from the blocking collection
                    while (_LoggerManager[sev].TryTake(out string _logMessage, TimeSpan.FromSeconds(1)))
                    {
                        //The actual printing to file
                        using (StreamWriter w = File.AppendText(filePath + "\\" + "log.txt"))
                        {
                            Log(_logMessage, w);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
           
        }
       
  

        private void Log(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.Write("\r\nLog Entry : ");
                txtWriter.WriteLine("  :{0}", logMessage);
                txtWriter.WriteLine("-------------------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion
    }
}
