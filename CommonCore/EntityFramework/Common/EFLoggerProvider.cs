using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCore.EntityFramework.Common
{
    public class EFLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName) => new EFLogger(categoryName);

        public void Dispose()
        {
        }
    }

    public class EFLogger : ILogger
    {
        private readonly string categoryName;

        public EFLogger(string categoryName) => this.categoryName = categoryName;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            //ef core执行数据库查询时的categoryName为Microsoft.EntityFrameworkCore.Database.Command,日志级别为Information
            if (categoryName != "Microsoft.EntityFrameworkCore.Database.Command" ||
                logLevel != LogLevel.Information) return;
            var logContent = formatter(state, exception);
            Console.WriteLine("<------------------ sql start ------------------>");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("sql: ");
            Console.ResetColor();
            Console.Write(logContent);
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("<------------------ sql end ------------------>");
        }

        public IDisposable BeginScope<TState>(TState state) => null;
    }
}