using NLog;
using NLog.Config;
using NLog.Filters;
using NLog.Targets;
using NLog.Targets.Wrappers;
using System;

namespace Apsis.Logging
{
    /// <summary>
    /// Logging Configuration
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// NLog Setup
        /// </summary>
        public static LoggingConfiguration Initialize()
        {
            // Create NLog configuration object 
            var config = new LoggingConfiguration();

            // Create targets and add them to the configuration 
            var fileTarget = GetFileTarget();
            config.AddTarget(fileTarget);

            // Define logging rules
            LoggingRule logRule = new LoggingRule("*", LogLevel.Trace, fileTarget);

            // Define filter to ignore logs from Microsoft DLLs
            var filterToIgnoreAllMicrosoftLogs = new ConditionBasedFilter
            {
                Condition = "starts-with(logger, 'Microsoft.')",
                Action = FilterResult.Ignore
            };
            logRule.Filters.Add(filterToIgnoreAllMicrosoftLogs);

            config.LoggingRules.Add(logRule);

            // Define wrappers
            var asyncWrapper = GetAsyncTargetWrapper();
            asyncWrapper.WrappedTarget = fileTarget;
            SimpleConfigurator.ConfigureForTargetLogging(asyncWrapper, LogLevel.Trace);

            // Activate the configuration
            LogManager.Configuration = config;
            return config;
        }

        /// <summary>
        /// Configuration to target file to log events
        /// </summary>
        /// <returns></returns>
        private static FileTarget GetFileTarget()
        {
            return new FileTarget
            {
                // Set target properties 
                Layout = @"${date:format=HH\:mm\:ss} | ${level} | ${message} ${exception} ${stacktrace}",
                FileName = "${basedir}/Logs/Log-${shortdate}.txt", // ${basedir} -> bin\debug folder
                Name = "FileTarget",
                KeepFileOpen = false,
                CreateDirs = true,
                ConcurrentWrites = true,
                ArchiveOldFileOnStartup = true,
                //ArchiveAboveSize = WIP : Need to have a max file size
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static AsyncTargetWrapper GetAsyncTargetWrapper()
        {
            return new AsyncTargetWrapper
            {
                TimeToSleepBetweenBatches = 100,
                OverflowAction = AsyncTargetWrapperOverflowAction.Grow
            };
        }
    }
}
