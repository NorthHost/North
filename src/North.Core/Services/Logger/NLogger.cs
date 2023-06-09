﻿using NLog;
using NLog.Config;
using NLog.Targets;

namespace North.Core.Services.Logger
{
    /// <summary>
    /// NLog 打印组件
    /// </summary>
    public class NLogger : ILogger
    {
        private const string Target_Name_File = "FileTarget";
        private const string Target_Name_ColoredConsole = "ConsoleTarget";
        private const string Rule_Name_File = "FileRule";
        private const string Rule_Name_ColoredConsole = "ConsoleRule";

        private readonly NLog.Logger _logger;

        public NLogger(LogSetting setting)
        {
            _logger = LogManager.GetCurrentClassLogger();
            ConfigLoggers(setting);
        }


        /// <summary>
        /// 配置日志
        /// </summary>
        /// <param name="settings"></param>
        /// <exception cref="ArgumentException"></exception>
        public void ConfigLoggers(LogSetting settings)
        {
            // 获取 NLog 配置
            LoggingConfiguration config = LogManager.Configuration;

            // 修改 Target（配置输出路径和格式）
            var fileTarget = (FileTarget)config.FindTargetByName(Target_Name_File);
            var coloredConsoleTarget = (ColoredConsoleTarget)config.FindTargetByName(Target_Name_ColoredConsole);

            fileTarget.Layout = settings.Layout;
            fileTarget.FileName = settings.Output;
            coloredConsoleTarget.Layout = settings.Layout;

            // 修改 Rule（配置日志级别）
            var fileRule = config.FindRuleByName(Rule_Name_File);
            var consoleRule = config.FindRuleByName(Rule_Name_ColoredConsole);

            if (fileRule.Levels.Any())
            {
                fileRule.DisableLoggingForLevels(fileRule.Levels.First(), fileRule.Levels.Last());
                consoleRule.DisableLoggingForLevels(consoleRule.Levels.First(), consoleRule.Levels.Last());
            }

            var minLogLevel = ParseLogLevel(settings.Levels.Min);
            var maxLogLevel = ParseLogLevel(settings.Levels.Max);
            if (minLogLevel < maxLogLevel)
            {
                fileRule.EnableLoggingForLevels(minLogLevel, maxLogLevel);
                consoleRule.EnableLoggingForLevels(minLogLevel, maxLogLevel);
            }
            else
            {
                throw new ArgumentException("MinLevel cannot over MaxLevel");
            }

            // 重新加载配置
            LogManager.ReconfigExistingLoggers();
        }


        /// <summary>
        /// 解析日志等级
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private static LogLevel ParseLogLevel(Microsoft.Extensions.Logging.LogLevel level)
        {
            return level switch
            {
                Microsoft.Extensions.Logging.LogLevel.Trace => LogLevel.Trace,
                Microsoft.Extensions.Logging.LogLevel.Debug => LogLevel.Debug,
                Microsoft.Extensions.Logging.LogLevel.Information => LogLevel.Info,
                Microsoft.Extensions.Logging.LogLevel.Warning => LogLevel.Warn,
                Microsoft.Extensions.Logging.LogLevel.Error => LogLevel.Error,
                Microsoft.Extensions.Logging.LogLevel.Critical => LogLevel.Fatal,
                _ => LogLevel.Off
            };
        }


        #region 日志输出 
        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Debug(string message, Exception e)
        {
            _logger.Debug(e, message);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Error(string message, Exception e)
        {
            _logger.Error(e, message);
        }

        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public void Fatal(string message, Exception e)
        {
            _logger.Fatal(e, message);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Info(string message, Exception e)
        {
            _logger.Info(e, message);
        }

        public void Trace(string message)
        {
            _logger.Trace(message);
        }

        public void Trace(string message, Exception e)
        {
            _logger.Trace(e, message);
        }

        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        public void Warn(string message, Exception e)
        {
            _logger.Warn(e, message);
        }
        #endregion
    }
}
