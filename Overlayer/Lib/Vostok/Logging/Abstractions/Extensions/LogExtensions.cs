using System;
using JetBrains.Annotations;
using Vostok.Commons.Time;
using Vostok.Logging.Abstractions.Helpers;

namespace Vostok.Logging.Abstractions
{
	[PublicAPI]
    public static class LogExtensions
    {
        #region Debug

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> on the <see cref="LogLevel.Debug"/> level without any additional properties.
        /// </summary>
        public static void Debug(this ILog log, [CanBeNull] string messageTemplate)
        {
            if (!log.IsEnabledFor(LogLevel.Debug))
                return;

            log.Log(new LogEvent(LogLevel.Debug, PreciseDateTime.Now, messageTemplate));
        }

        /// <summary>
        /// Logs the given <paramref name="exception"/> on the <see cref="LogLevel.Debug"/> level without a message or any additional properties.
        /// </summary>
        public static void Debug(this ILog log, [CanBeNull] Exception exception)
        {
            if (!log.IsEnabledFor(LogLevel.Debug))
                return;

            log.Log(new LogEvent(LogLevel.Debug, PreciseDateTime.Now, null, exception));
        }

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> and <paramref name="exception"/> on the <see cref="LogLevel.Debug"/> level without any additional properties.
        /// </summary>
        public static void Debug(this ILog log, [CanBeNull] Exception exception, [CanBeNull] string messageTemplate)
        {
            if (!log.IsEnabledFor(LogLevel.Debug))
                return;

            log.Log(new LogEvent(LogLevel.Debug, PreciseDateTime.Now, messageTemplate, exception));
        }

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> on the <see cref="LogLevel.Debug"/> level with given <paramref name="properties" />. The <paramref name="messageTemplate"/> can contain placeholders for <paramref name="properties"/>, see <see cref="LogEvent.MessageTemplate"/> for details.
        /// </summary>
        public static void Debug<T>(this ILog log, [CanBeNull] string messageTemplate, [CanBeNull] T properties)
        {
            if (!log.IsEnabledFor(LogLevel.Debug))
                return;

            if (!DeconstructionHelper.ShouldDeconstruct(messageTemplate, properties))
            {
                log.Debug(messageTemplate, (object)properties);
                return;
            }

            log.Log(new LogEvent(LogLevel.Debug, PreciseDateTime.Now, messageTemplate, LogPropertiesExtensions.GenerateInitialObjectProperties(properties, true), null));
        }

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> on the <see cref="LogLevel.Debug"/> level with given <paramref name="parameters" />. The <paramref name="messageTemplate"/> can contain placeholders for <paramref name="parameters"/>, see <see cref="LogEvent.MessageTemplate"/> for details.
        /// </summary>
        public static void Debug(this ILog log, [CanBeNull] string messageTemplate, [CanBeNull] params object[] parameters)
        {
            if (!log.IsEnabledFor(LogLevel.Debug))
                return;

            log.Log(new LogEvent(LogLevel.Debug, PreciseDateTime.Now, messageTemplate, LogEventExtensions.GenerateInitialParameters(messageTemplate, parameters), null));
        }

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> and <paramref name="exception"/> on the <see cref="LogLevel.Debug"/> level with given <paramref name="properties" />. The <paramref name="messageTemplate"/> can contain placeholders for <paramref name="properties"/>, see <see cref="LogEvent.MessageTemplate"/> for details.
        /// </summary>
        public static void Debug<T>(this ILog log, [CanBeNull] Exception exception, [CanBeNull] string messageTemplate, [CanBeNull] T properties)
        {
            if (!log.IsEnabledFor(LogLevel.Debug))
                return;

            if (!DeconstructionHelper.ShouldDeconstruct(messageTemplate, properties))
            {
                log.Debug(exception, messageTemplate, (object)properties);
                return;
            }

            log.Log(new LogEvent(LogLevel.Debug, PreciseDateTime.Now, messageTemplate, LogPropertiesExtensions.GenerateInitialObjectProperties(properties, true), exception));
        }

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> and <paramref name="exception"/> on the <see cref="LogLevel.Debug"/> level with given <paramref name="parameters" />. The <paramref name="messageTemplate"/> can contain placeholders for <paramref name="parameters"/>, see <see cref="LogEvent.MessageTemplate"/> for details.
        /// </summary>
        public static void Debug(this ILog log, [CanBeNull] Exception exception, [CanBeNull] string messageTemplate, [CanBeNull] params object[] parameters)
        {
            if (!log.IsEnabledFor(LogLevel.Debug))
                return;

            log.Log(new LogEvent(LogLevel.Debug, PreciseDateTime.Now, messageTemplate, LogEventExtensions.GenerateInitialParameters(messageTemplate, parameters), exception));
        }

        [Obsolete("Use the Debug(ILog, Exception, string) overload instead.")]
        public static void Debug(this ILog log, [CanBeNull] string messageTemplate, [CanBeNull] Exception exception)
        {
            if (!log.IsEnabledFor(LogLevel.Debug))
                return;

            log.Debug(exception, messageTemplate);
        }

        /// <summary>
        /// Returns true if given <paramref name="log"/> is enabled to log events of <see cref="LogLevel.Debug"/> level, or false otherwise.
        /// </summary>
        public static bool IsEnabledForDebug(this ILog log)
        {
            return log.IsEnabledFor(LogLevel.Debug);
        }

        #endregion

        #region Info

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> on the <see cref="LogLevel.Info"/> level without any additional properties.
        /// </summary>
        public static void Info(this ILog log, [CanBeNull] string messageTemplate)
        {
            if (!log.IsEnabledFor(LogLevel.Info))
                return;

            log.Log(new LogEvent(LogLevel.Info, PreciseDateTime.Now, messageTemplate));
        }

        /// <summary>
        /// Logs the given <paramref name="exception"/> on the <see cref="LogLevel.Info"/> level without a message or any additional properties.
        /// </summary>
        public static void Info(this ILog log, [CanBeNull] Exception exception)
        {
            if (!log.IsEnabledFor(LogLevel.Info))
                return;

            log.Log(new LogEvent(LogLevel.Info, PreciseDateTime.Now, null, exception));
        }

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> and <paramref name="exception"/> on the <see cref="LogLevel.Info"/> level without any additional properties.
        /// </summary>
        public static void Info(this ILog log, [CanBeNull] Exception exception, [CanBeNull] string messageTemplate)
        {
            if (!log.IsEnabledFor(LogLevel.Info))
                return;

            log.Log(new LogEvent(LogLevel.Info, PreciseDateTime.Now, messageTemplate, exception));
        }

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> on the <see cref="LogLevel.Info"/> level with given <paramref name="properties" />. The <paramref name="messageTemplate"/> can contain placeholders for <paramref name="properties"/>, see <see cref="LogEvent.MessageTemplate"/> for details.
        /// </summary>
        public static void Info<T>(this ILog log, [CanBeNull] string messageTemplate, [CanBeNull] T properties)
        {
            if (!log.IsEnabledFor(LogLevel.Info))
                return;

            if (!DeconstructionHelper.ShouldDeconstruct(messageTemplate, properties))
            {
                log.Info(messageTemplate, (object)properties);
                return;
            }

            log.Log(new LogEvent(LogLevel.Info, PreciseDateTime.Now, messageTemplate, LogPropertiesExtensions.GenerateInitialObjectProperties(properties, true), null));
        }

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> on the <see cref="LogLevel.Info"/> level with given <paramref name="parameters" />. The <paramref name="messageTemplate"/> can contain placeholders for <paramref name="parameters"/>, see <see cref="LogEvent.MessageTemplate"/> for details.
        /// </summary>
        public static void Info(this ILog log, [CanBeNull] string messageTemplate, [CanBeNull] params object[] parameters)
        {
            if (!log.IsEnabledFor(LogLevel.Info))
                return;

            log.Log(new LogEvent(LogLevel.Info, PreciseDateTime.Now, messageTemplate, LogEventExtensions.GenerateInitialParameters(messageTemplate, parameters), null));
        }

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> and <paramref name="exception"/> on the <see cref="LogLevel.Info"/> level with given <paramref name="properties" />. The <paramref name="messageTemplate"/> can contain placeholders for <paramref name="properties"/>, see <see cref="LogEvent.MessageTemplate"/> for details.
        /// </summary>
        public static void Info<T>(this ILog log, [CanBeNull] Exception exception, [CanBeNull] string messageTemplate, [CanBeNull] T properties)
        {
            if (!log.IsEnabledFor(LogLevel.Info))
                return;

            if (!DeconstructionHelper.ShouldDeconstruct(messageTemplate, properties))
            {
                log.Info(exception, messageTemplate, (object)properties);
                return;
            }

            log.Log(new LogEvent(LogLevel.Info, PreciseDateTime.Now, messageTemplate, LogPropertiesExtensions.GenerateInitialObjectProperties(properties, true), exception));
        }

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> and <paramref name="exception"/> on the <see cref="LogLevel.Info"/> level with given <paramref name="parameters" />. The <paramref name="messageTemplate"/> can contain placeholders for <paramref name="parameters"/>, see <see cref="LogEvent.MessageTemplate"/> for details.
        /// </summary>
        public static void Info(this ILog log, [CanBeNull] Exception exception, [CanBeNull] string messageTemplate, [CanBeNull] params object[] parameters)
        {
            if (!log.IsEnabledFor(LogLevel.Info))
                return;

            log.Log(new LogEvent(LogLevel.Info, PreciseDateTime.Now, messageTemplate, LogEventExtensions.GenerateInitialParameters(messageTemplate, parameters), exception));
        }

        [Obsolete("Use the Info(ILog, Exception, string) overload instead.")]
        public static void Info(this ILog log, [CanBeNull] string messageTemplate, [CanBeNull] Exception exception)
        {
            if (!log.IsEnabledFor(LogLevel.Info))
                return;

            log.Info(exception, messageTemplate);
        }

        /// <summary>
        /// Returns true if given <paramref name="log"/> is enabled to log events of <see cref="LogLevel.Info"/> level, or false otherwise.
        /// </summary>
        public static bool IsEnabledForInfo(this ILog log)
        {
            return log.IsEnabledFor(LogLevel.Info);
        }

        #endregion

        #region Warn

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> on the <see cref="LogLevel.Warn"/> level without any additional properties.
        /// </summary>
        public static void Warn(this ILog log, [CanBeNull] string messageTemplate)
        {
            if (!log.IsEnabledFor(LogLevel.Warn))
                return;

            log.Log(new LogEvent(LogLevel.Warn, PreciseDateTime.Now, messageTemplate));
        }

        /// <summary>
        /// Logs the given <paramref name="exception"/> on the <see cref="LogLevel.Warn"/> level without a message or any additional properties.
        /// </summary>
        public static void Warn(this ILog log, [CanBeNull] Exception exception)
        {
            if (!log.IsEnabledFor(LogLevel.Warn))
                return;

            log.Log(new LogEvent(LogLevel.Warn, PreciseDateTime.Now, null, exception));
        }

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> and <paramref name="exception"/> on the <see cref="LogLevel.Warn"/> level without any additional properties.
        /// </summary>
        public static void Warn(this ILog log, [CanBeNull] Exception exception, [CanBeNull] string messageTemplate)
        {
            if (!log.IsEnabledFor(LogLevel.Warn))
                return;

            log.Log(new LogEvent(LogLevel.Warn, PreciseDateTime.Now, messageTemplate, exception));
        }

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> on the <see cref="LogLevel.Warn"/> level with given <paramref name="properties" />. The <paramref name="messageTemplate"/> can contain placeholders for <paramref name="properties"/>, see <see cref="LogEvent.MessageTemplate"/> for details.
        /// </summary>
        public static void Warn<T>(this ILog log, [CanBeNull] string messageTemplate, [CanBeNull] T properties)
        {
            if (!log.IsEnabledFor(LogLevel.Warn))
                return;

            if (!DeconstructionHelper.ShouldDeconstruct(messageTemplate, properties))
            {
                log.Warn(messageTemplate, (object)properties);
                return;
            }

            log.Log(new LogEvent(LogLevel.Warn, PreciseDateTime.Now, messageTemplate, LogPropertiesExtensions.GenerateInitialObjectProperties(properties, true), null));
        }

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> on the <see cref="LogLevel.Warn"/> level with given <paramref name="parameters" />. The <paramref name="messageTemplate"/> can contain placeholders for <paramref name="parameters"/>, see <see cref="LogEvent.MessageTemplate"/> for details.
        /// </summary>
        public static void Warn(this ILog log, [CanBeNull] string messageTemplate, [CanBeNull] params object[] parameters)
        {
            if (!log.IsEnabledFor(LogLevel.Warn))
                return;

            log.Log(new LogEvent(LogLevel.Warn, PreciseDateTime.Now, messageTemplate, LogEventExtensions.GenerateInitialParameters(messageTemplate, parameters), null));
        }

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> and <paramref name="exception"/> on the <see cref="LogLevel.Warn"/> level with given <paramref name="properties" />. The <paramref name="messageTemplate"/> can contain placeholders for <paramref name="properties"/>, see <see cref="LogEvent.MessageTemplate"/> for details.
        /// </summary>
        public static void Warn<T>(this ILog log, [CanBeNull] Exception exception, [CanBeNull] string messageTemplate, [CanBeNull] T properties)
        {
            if (!log.IsEnabledFor(LogLevel.Warn))
                return;

            if (!DeconstructionHelper.ShouldDeconstruct(messageTemplate, properties))
            {
                log.Warn(exception, messageTemplate, (object)properties);
                return;
            }

            log.Log(new LogEvent(LogLevel.Warn, PreciseDateTime.Now, messageTemplate, LogPropertiesExtensions.GenerateInitialObjectProperties(properties, true), exception));
        }

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> and <paramref name="exception"/> on the <see cref="LogLevel.Warn"/> level with given <paramref name="parameters" />. The <paramref name="messageTemplate"/> can contain placeholders for <paramref name="parameters"/>, see <see cref="LogEvent.MessageTemplate"/> for details.
        /// </summary>
        public static void Warn(this ILog log, [CanBeNull] Exception exception, [CanBeNull] string messageTemplate, [CanBeNull] params object[] parameters)
        {
            if (!log.IsEnabledFor(LogLevel.Warn))
                return;

            log.Log(new LogEvent(LogLevel.Warn, PreciseDateTime.Now, messageTemplate, LogEventExtensions.GenerateInitialParameters(messageTemplate, parameters), exception));
        }

        [Obsolete("Use the Warn(ILog, Exception, string) overload instead.")]
        public static void Warn(this ILog log, [CanBeNull] string messageTemplate, [CanBeNull] Exception exception)
        {
            if (!log.IsEnabledFor(LogLevel.Warn))
                return;

            log.Warn(exception, messageTemplate);
        }

        /// <summary>
        /// Returns true if given <paramref name="log"/> is enabled to log events of <see cref="LogLevel.Warn"/> level, or false otherwise.
        /// </summary>
        public static bool IsEnabledForWarn(this ILog log)
        {
            return log.IsEnabledFor(LogLevel.Warn);
        }

        #endregion

        #region Error

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> on the <see cref="LogLevel.Error"/> level without any additional properties.
        /// </summary>
        public static void Error(this ILog log, [CanBeNull] string messageTemplate)
        {
            if (!log.IsEnabledFor(LogLevel.Error))
                return;

            log.Log(new LogEvent(LogLevel.Error, PreciseDateTime.Now, messageTemplate));
        }

        /// <summary>
        /// Logs the given <paramref name="exception"/> on the <see cref="LogLevel.Error"/> level without a message or any additional properties.
        /// </summary>
        public static void Error(this ILog log, [CanBeNull] Exception exception)
        {
            if (!log.IsEnabledFor(LogLevel.Error))
                return;

            log.Log(new LogEvent(LogLevel.Error, PreciseDateTime.Now, null, exception));
        }

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> and <paramref name="exception"/> on the <see cref="LogLevel.Error"/> level without any additional properties.
        /// </summary>
        public static void Error(this ILog log, [CanBeNull] Exception exception, [CanBeNull] string messageTemplate)
        {
            if (!log.IsEnabledFor(LogLevel.Error))
                return;

            log.Log(new LogEvent(LogLevel.Error, PreciseDateTime.Now, messageTemplate, exception));
        }

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> on the <see cref="LogLevel.Error"/> level with given <paramref name="properties" />. The <paramref name="messageTemplate"/> can contain placeholders for <paramref name="properties"/>, see <see cref="LogEvent.MessageTemplate"/> for details.
        /// </summary>
        public static void Error<T>(this ILog log, [CanBeNull] string messageTemplate, [CanBeNull] T properties)
        {
            if (!log.IsEnabledFor(LogLevel.Error))
                return;

            if (!DeconstructionHelper.ShouldDeconstruct(messageTemplate, properties))
            {
                log.Error(messageTemplate, (object)properties);
                return;
            }

            log.Log(new LogEvent(LogLevel.Error, PreciseDateTime.Now, messageTemplate, LogPropertiesExtensions.GenerateInitialObjectProperties(properties, true), null));
        }

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> on the <see cref="LogLevel.Error"/> level with given <paramref name="parameters" />. The <paramref name="messageTemplate"/> can contain placeholders for <paramref name="parameters"/>, see <see cref="LogEvent.MessageTemplate"/> for details.
        /// </summary>
        public static void Error(this ILog log, [CanBeNull] string messageTemplate, [CanBeNull] params object[] parameters)
        {
            if (!log.IsEnabledFor(LogLevel.Error))
                return;

            log.Log(new LogEvent(LogLevel.Error, PreciseDateTime.Now, messageTemplate, LogEventExtensions.GenerateInitialParameters(messageTemplate, parameters), null));
        }

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> and <paramref name="exception"/> on the <see cref="LogLevel.Error"/> level with given <paramref name="properties" />. The <paramref name="messageTemplate"/> can contain placeholders for <paramref name="properties"/>, see <see cref="LogEvent.MessageTemplate"/> for details.
        /// </summary>
        public static void Error<T>(this ILog log, [CanBeNull] Exception exception, [CanBeNull] string messageTemplate, [CanBeNull] T properties)
        {
            if (!log.IsEnabledFor(LogLevel.Error))
                return;

            if (!DeconstructionHelper.ShouldDeconstruct(messageTemplate, properties))
            {
                log.Error(exception, messageTemplate, (object)properties);
                return;
            }

            log.Log(new LogEvent(LogLevel.Error, PreciseDateTime.Now, messageTemplate, LogPropertiesExtensions.GenerateInitialObjectProperties(properties, true), exception));
        }

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> and <paramref name="exception"/> on the <see cref="LogLevel.Error"/> level with given <paramref name="parameters" />. The <paramref name="messageTemplate"/> can contain placeholders for <paramref name="parameters"/>, see <see cref="LogEvent.MessageTemplate"/> for details.
        /// </summary>
        public static void Error(this ILog log, [CanBeNull] Exception exception, [CanBeNull] string messageTemplate, [CanBeNull] params object[] parameters)
        {
            if (!log.IsEnabledFor(LogLevel.Error))
                return;

            log.Log(new LogEvent(LogLevel.Error, PreciseDateTime.Now, messageTemplate, LogEventExtensions.GenerateInitialParameters(messageTemplate, parameters), exception));
        }

        [Obsolete("Use the Error(ILog, Exception, string) overload instead.")]
        public static void Error(this ILog log, [CanBeNull] string messageTemplate, [CanBeNull] Exception exception)
        {
            if (!log.IsEnabledFor(LogLevel.Error))
                return;

            log.Error(exception, messageTemplate);
        }

        /// <summary>
        /// Returns true if given <paramref name="log"/> is enabled to log events of <see cref="LogLevel.Error"/> level, or false otherwise.
        /// </summary>
        public static bool IsEnabledForError(this ILog log)
        {
            return log.IsEnabledFor(LogLevel.Error);
        }

        #endregion

        #region Fatal

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> on the <see cref="LogLevel.Fatal"/> level without any additional properties.
        /// </summary>
        public static void Fatal(this ILog log, [CanBeNull] string messageTemplate)
        {
            if (!log.IsEnabledFor(LogLevel.Fatal))
                return;

            log.Log(new LogEvent(LogLevel.Fatal, PreciseDateTime.Now, messageTemplate));
        }

        /// <summary>
        /// Logs the given <paramref name="exception"/> on the <see cref="LogLevel.Fatal"/> level without a message or any additional properties.
        /// </summary>
        public static void Fatal(this ILog log, [CanBeNull] Exception exception)
        {
            if (!log.IsEnabledFor(LogLevel.Fatal))
                return;

            log.Log(new LogEvent(LogLevel.Fatal, PreciseDateTime.Now, null, exception));
        }

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> and <paramref name="exception"/> on the <see cref="LogLevel.Fatal"/> level without any additional properties.
        /// </summary>
        public static void Fatal(this ILog log, [CanBeNull] Exception exception, [CanBeNull] string messageTemplate)
        {
            if (!log.IsEnabledFor(LogLevel.Fatal))
                return;

            log.Log(new LogEvent(LogLevel.Fatal, PreciseDateTime.Now, messageTemplate, exception));
        }

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> on the <see cref="LogLevel.Fatal"/> level with given <paramref name="properties" />. The <paramref name="messageTemplate"/> can contain placeholders for <paramref name="properties"/>, see <see cref="LogEvent.MessageTemplate"/> for details.
        /// </summary>
        public static void Fatal<T>(this ILog log, [CanBeNull] string messageTemplate, [CanBeNull] T properties)
        {
            if (!log.IsEnabledFor(LogLevel.Fatal))
                return;

            if (!DeconstructionHelper.ShouldDeconstruct(messageTemplate, properties))
            {
                log.Fatal(messageTemplate, (object)properties);
                return;
            }

            log.Log(new LogEvent(LogLevel.Fatal, PreciseDateTime.Now, messageTemplate, LogPropertiesExtensions.GenerateInitialObjectProperties(properties, true), null));
        }

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> on the <see cref="LogLevel.Fatal"/> level with given <paramref name="parameters" />. The <paramref name="messageTemplate"/> can contain placeholders for <paramref name="parameters"/>, see <see cref="LogEvent.MessageTemplate"/> for details.
        /// </summary>
        public static void Fatal(this ILog log, [CanBeNull] string messageTemplate, [CanBeNull] params object[] parameters)
        {
            if (!log.IsEnabledFor(LogLevel.Fatal))
                return;

            log.Log(new LogEvent(LogLevel.Fatal, PreciseDateTime.Now, messageTemplate, LogEventExtensions.GenerateInitialParameters(messageTemplate, parameters), null));
        }

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> and <paramref name="exception"/> on the <see cref="LogLevel.Fatal"/> level with given <paramref name="properties" />. The <paramref name="messageTemplate"/> can contain placeholders for <paramref name="properties"/>, see <see cref="LogEvent.MessageTemplate"/> for details.
        /// </summary>
        public static void Fatal<T>(this ILog log, [CanBeNull] Exception exception, [CanBeNull] string messageTemplate, [CanBeNull] T properties)
        {
            if (!log.IsEnabledFor(LogLevel.Fatal))
                return;

            if (!DeconstructionHelper.ShouldDeconstruct(messageTemplate, properties))
            {
                log.Fatal(exception, messageTemplate, (object)properties);
                return;
            }

            log.Log(new LogEvent(LogLevel.Fatal, PreciseDateTime.Now, messageTemplate, LogPropertiesExtensions.GenerateInitialObjectProperties(properties, true), exception));
        }

        /// <summary>
        /// Logs the given <paramref name="messageTemplate"/> and <paramref name="exception"/> on the <see cref="LogLevel.Fatal"/> level with given <paramref name="parameters" />. The <paramref name="messageTemplate"/> can contain placeholders for <paramref name="parameters"/>, see <see cref="LogEvent.MessageTemplate"/> for details.
        /// </summary>
        public static void Fatal(this ILog log, [CanBeNull] Exception exception, [CanBeNull] string messageTemplate, [CanBeNull] params object[] parameters)
        {
            if (!log.IsEnabledFor(LogLevel.Fatal))
                return;

            log.Log(new LogEvent(LogLevel.Fatal, PreciseDateTime.Now, messageTemplate, LogEventExtensions.GenerateInitialParameters(messageTemplate, parameters), exception));
        }

        [Obsolete("Use the Fatal(ILog, Exception, string) overload instead.")]
        public static void Fatal(this ILog log, [CanBeNull] string messageTemplate, [CanBeNull] Exception exception)
        {
            if (!log.IsEnabledFor(LogLevel.Fatal))
                return;

            log.Fatal(exception, messageTemplate);
        }

        /// <summary>
        /// Returns true if given <paramref name="log"/> is enabled to log events of <see cref="LogLevel.Fatal"/> level, or false otherwise.
        /// </summary>
        public static bool IsEnabledForFatal(this ILog log)
        {
            return log.IsEnabledFor(LogLevel.Fatal);
        }

        #endregion

    }
}