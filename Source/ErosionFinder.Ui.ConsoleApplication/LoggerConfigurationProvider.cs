using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;

namespace ErosionFinder.Ui.ConsoleApplication
{
    static class LoggerConfigurationProvider
    {
        private static LoggingLevelSwitch LoggingLevelSwitch = GetLoggingLevelSwitch();

        public static LoggerConfiguration LoggerConfiguration = GetLoggerConfiguration();

        public static void AlterLogLevel(LogEventLevel minimumLevel)
        {
            LoggingLevelSwitch.MinimumLevel = minimumLevel;
        }

        private static LoggingLevelSwitch GetLoggingLevelSwitch()
            => new LoggingLevelSwitch(LogEventLevel.Error);

        private static LoggerConfiguration GetLoggerConfiguration()
            => new LoggerConfiguration()
            .MinimumLevel.ControlledBy(LoggingLevelSwitch)
            .WriteTo.File(GetLogFilePath(), rollingInterval: RollingInterval.Minute);

        private static string GetLogFilePath() => $"logs/{DateTime.Now:yyyyMMdd-HHmm}.txt";
    }
}