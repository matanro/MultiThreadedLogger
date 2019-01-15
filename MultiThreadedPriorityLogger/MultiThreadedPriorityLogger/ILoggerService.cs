

namespace MultiThreadedPriorityLogger
{

    public enum LogType { Debug,Info,Warn,Error,Critical,Fatal}

    public interface ILoggerService
    {
        void WriteLog(LogType sevirity, string message);       
    }
}
