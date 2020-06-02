using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Bolt.App.Core.Extensions.Default
{
    public static class Log
    {
        private static ILogger _logger;

        /// <summary>
        /// Warning: Not thread safe. Should only be run when application start
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="loggerName"></param>
        internal static void Init(ILoggerFactory loggerFactory, string loggerName = "AppLogger")
        {
            _logger = loggerFactory.CreateLogger(loggerName);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warn(string message, params object[] args)
        {
            _logger?.LogWarning(message, args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warn(Exception exception, string message, params object[] args)
        {
            _logger?.LogWarning(exception, message, args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Error(string message, params object[] args)
        {
            _logger?.LogError(message, args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Error(Exception exception, string message, params object[] args)
        {
            _logger?.LogError(exception, message, args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Info(string message, params object[] args)
        {
            _logger?.LogInformation(message, args);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Debug(string message, params object[] args)
        {
            _logger?.LogDebug(message, args);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Trace(string message, params object[] args)
        {
            _logger?.LogTrace(message, args);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        public static bool IsTraceEnabled() => _logger?.IsEnabled(LogLevel.Trace) ?? false;

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInfoEnabled() => _logger?.IsEnabled(LogLevel.Information) ?? false;

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDebugEnabled() => _logger?.IsEnabled(LogLevel.Debug) ?? false;

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWarnEnabled() => _logger?.IsEnabled(LogLevel.Warning) ?? false;

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsErrorEnabled() => _logger?.IsEnabled(LogLevel.Error) ?? false;
    }
}