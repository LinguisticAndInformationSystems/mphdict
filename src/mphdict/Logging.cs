using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mphdict.Logging
{
    public class FileLogger : ILogger
    {
        private string filePath;
        private object _lock = new object();
        LogLevel ll = LogLevel.Debug;
        public FileLogger(string path)
        {
            filePath = path;
            string fullpath = Path.GetDirectoryName(path);
            if (!Directory.Exists(fullpath))
            {
                Directory.CreateDirectory(fullpath);
            }

        }
        public FileLogger MinimumLevel(LogLevel logLevel) {
            ll = logLevel;
            return this;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if ((formatter != null)&&(logLevel>= ll))
            {
                lock (_lock)
                {
                    StringBuilder exs = new StringBuilder($"{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")} \t {logLevel.ToString()}:{Environment.NewLine}");
                    if (exception != null)
                    {
                        exs = exs.Append($"Exception: {exception.Message}{Environment.NewLine}");
                        if (exception.InnerException != null) exs.Append($"Inner Exception: {exception.InnerException.Message}{Environment.NewLine}");
                        if (exception.Source != null) exs.Append($"Source: {exception.Source}{Environment.NewLine}");
                        if (exception.StackTrace != null) exs.Append($"StackTrace: {exception.StackTrace}{Environment.NewLine}");
                    }
                    File.AppendAllText(filePath, exs.ToString(), Encoding.Unicode);
                    File.AppendAllText(filePath, formatter(state, exception) + Environment.NewLine, Encoding.Unicode);
                }
            }
        }
    }

    public class FileLoggerProvider : ILoggerProvider
    {
        private string path;
        LogLevel logLevel=LogLevel.Debug;
        public FileLoggerProvider(string _path, LogLevel logLevel)
        {
            path = _path;
            this.logLevel = logLevel;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(path).MinimumLevel(logLevel);
        }

        public void Dispose()
        {
        }
    }
    public static class FileLoggerExtensions
    {
        public static ILoggerFactory AddFile(this ILoggerFactory factory,
                                        string filePath, LogLevel logLevel)
        {
            factory.AddProvider(new FileLoggerProvider(filePath, logLevel));
            return factory;
        }
    }

}

