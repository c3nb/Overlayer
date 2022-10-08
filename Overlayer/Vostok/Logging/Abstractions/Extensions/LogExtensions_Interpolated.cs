#if NET6_0_OR_GREATER
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;
using Vostok.Commons.Collections;
using Vostok.Commons.Time;
using Vostok.Logging.Abstractions.Helpers;

// ReSharper disable MethodOverloadWithOptionalParameter

namespace Vostok.Logging.Abstractions
{
    [PublicAPI]
    public static class LogExtensions_Interpolated
    {
        #region Debug

        /// <summary>
        /// Logs the given <paramref name="message"/> as interpolated string on the <see cref="LogLevel.Debug"/> level.
        /// </summary>
        public static void Debug(this ILog log, [InterpolatedStringHandlerArgument("log")] ref DebugStringHandler message)
        {
            if (!message.IsEnabled)
                return;

            log.Log(new LogEvent(LogLevel.Debug, PreciseDateTime.Now, message.MessageTemplate.ToString(), message.Properties, null));
        }

        /// <summary>
        /// Logs the given <paramref name="exception"/> on the <see cref="LogLevel.Debug"/> level with the given <paramref name="message"/> as interpolated string.
        /// </summary>
        public static void Debug(this ILog log, [CanBeNull] Exception exception, [InterpolatedStringHandlerArgument("log")] ref DebugStringHandler message)
        {
            if (!message.IsEnabled)
                return;

            log.Log(new LogEvent(LogLevel.Debug, PreciseDateTime.Now, message.MessageTemplate.ToString(), message.Properties, exception));
        }

        [PublicAPI]
        [InterpolatedStringHandler]
        public ref struct DebugStringHandler
        {
            public DebugStringHandler(int literalLength, int formattedCount, ILog log, out bool isEnabled)
            {
                IsEnabled = isEnabled = log.IsEnabledFor(LogLevel.Debug);
                if (!isEnabled)
                    return;

                MessageTemplate = new StringBuilder(literalLength);

                if (formattedCount > 0)
                    Properties = LogEvent.CreateProperties(Math.Max(4, formattedCount));
            }

            public void AppendLiteral(string value)
            {
                MessageTemplate.Append(value);
            }

            public void AppendFormatted<T>(T value, int alignment, [CallerArgumentExpression("value")] string name = "")
            {
                var defaultHandler = CreateDefaultHandler();
                defaultHandler.AppendFormatted(value, alignment);
                AppendFormatted((object)defaultHandler.ToStringAndClear(), name);
            }

            public void AppendFormatted<T>(T value, string format, [CallerArgumentExpression("value")] string name = "")
            {
                var defaultHandler = CreateDefaultHandler();
                defaultHandler.AppendFormatted(value, format);
                AppendFormatted((object)defaultHandler.ToStringAndClear(), name);
            }

            public void AppendFormatted<T>(T value, int alignment, string format, [CallerArgumentExpression("value")] string name = "")
            {
                var defaultHandler = CreateDefaultHandler();
                defaultHandler.AppendFormatted(value, alignment, format);
                AppendFormatted((object)defaultHandler.ToStringAndClear(), name);
            }

            public void AppendFormatted(object value, [CallerArgumentExpression("value")] string name = "")
            {
                name = InterpolatedHelper.EscapeName(name);

                MessageTemplate.Append('{');
                MessageTemplate.Append(name);
                MessageTemplate.Append('}');
                
                Properties.SetUnsafe(name, value, true);
            }

            internal bool IsEnabled { get; }
            internal StringBuilder MessageTemplate { get; } = null!;
            internal ImmutableArrayDictionary<string, object> Properties { get; private set; } = null!;

            private static DefaultInterpolatedStringHandler CreateDefaultHandler() =>
                new DefaultInterpolatedStringHandler(0, 1, CultureInfo.InvariantCulture);
        }

        #endregion

        #region Info

        /// <summary>
        /// Logs the given <paramref name="message"/> as interpolated string on the <see cref="LogLevel.Info"/> level.
        /// </summary>
        public static void Info(this ILog log, [InterpolatedStringHandlerArgument("log")] ref InfoStringHandler message)
        {
            if (!message.IsEnabled)
                return;

            log.Log(new LogEvent(LogLevel.Info, PreciseDateTime.Now, message.MessageTemplate.ToString(), message.Properties, null));
        }

        /// <summary>
        /// Logs the given <paramref name="exception"/> on the <see cref="LogLevel.Info"/> level with the given <paramref name="message"/> as interpolated string.
        /// </summary>
        public static void Info(this ILog log, [CanBeNull] Exception exception, [InterpolatedStringHandlerArgument("log")] ref InfoStringHandler message)
        {
            if (!message.IsEnabled)
                return;

            log.Log(new LogEvent(LogLevel.Info, PreciseDateTime.Now, message.MessageTemplate.ToString(), message.Properties, exception));
        }

        [PublicAPI]
        [InterpolatedStringHandler]
        public ref struct InfoStringHandler
        {
            public InfoStringHandler(int literalLength, int formattedCount, ILog log, out bool isEnabled)
            {
                IsEnabled = isEnabled = log.IsEnabledFor(LogLevel.Info);
                if (!isEnabled)
                    return;

                MessageTemplate = new StringBuilder(literalLength);

                if (formattedCount > 0)
                    Properties = LogEvent.CreateProperties(Math.Max(4, formattedCount));
            }

            public void AppendLiteral(string value)
            {
                MessageTemplate.Append(value);
            }

            public void AppendFormatted<T>(T value, int alignment, [CallerArgumentExpression("value")] string name = "")
            {
                var defaultHandler = CreateDefaultHandler();
                defaultHandler.AppendFormatted(value, alignment);
                AppendFormatted((object)defaultHandler.ToStringAndClear(), name);
            }

            public void AppendFormatted<T>(T value, string format, [CallerArgumentExpression("value")] string name = "")
            {
                var defaultHandler = CreateDefaultHandler();
                defaultHandler.AppendFormatted(value, format);
                AppendFormatted((object)defaultHandler.ToStringAndClear(), name);
            }

            public void AppendFormatted<T>(T value, int alignment, string format, [CallerArgumentExpression("value")] string name = "")
            {
                var defaultHandler = CreateDefaultHandler();
                defaultHandler.AppendFormatted(value, alignment, format);
                AppendFormatted((object)defaultHandler.ToStringAndClear(), name);
            }

            public void AppendFormatted(object value, [CallerArgumentExpression("value")] string name = "")
            {
                name = InterpolatedHelper.EscapeName(name);

                MessageTemplate.Append('{');
                MessageTemplate.Append(name);
                MessageTemplate.Append('}');
                
                Properties.SetUnsafe(name, value, true);
            }

            internal bool IsEnabled { get; }
            internal StringBuilder MessageTemplate { get; } = null!;
            internal ImmutableArrayDictionary<string, object> Properties { get; private set; } = null!;

            private static DefaultInterpolatedStringHandler CreateDefaultHandler() =>
                new DefaultInterpolatedStringHandler(0, 1, CultureInfo.InvariantCulture);
        }

        #endregion

        #region Warn

        /// <summary>
        /// Logs the given <paramref name="message"/> as interpolated string on the <see cref="LogLevel.Warn"/> level.
        /// </summary>
        public static void Warn(this ILog log, [InterpolatedStringHandlerArgument("log")] ref WarnStringHandler message)
        {
            if (!message.IsEnabled)
                return;

            log.Log(new LogEvent(LogLevel.Warn, PreciseDateTime.Now, message.MessageTemplate.ToString(), message.Properties, null));
        }

        /// <summary>
        /// Logs the given <paramref name="exception"/> on the <see cref="LogLevel.Warn"/> level with the given <paramref name="message"/> as interpolated string.
        /// </summary>
        public static void Warn(this ILog log, [CanBeNull] Exception exception, [InterpolatedStringHandlerArgument("log")] ref WarnStringHandler message)
        {
            if (!message.IsEnabled)
                return;

            log.Log(new LogEvent(LogLevel.Warn, PreciseDateTime.Now, message.MessageTemplate.ToString(), message.Properties, exception));
        }

        [PublicAPI]
        [InterpolatedStringHandler]
        public ref struct WarnStringHandler
        {
            public WarnStringHandler(int literalLength, int formattedCount, ILog log, out bool isEnabled)
            {
                IsEnabled = isEnabled = log.IsEnabledFor(LogLevel.Warn);
                if (!isEnabled)
                    return;

                MessageTemplate = new StringBuilder(literalLength);

                if (formattedCount > 0)
                    Properties = LogEvent.CreateProperties(Math.Max(4, formattedCount));
            }

            public void AppendLiteral(string value)
            {
                MessageTemplate.Append(value);
            }

            public void AppendFormatted<T>(T value, int alignment, [CallerArgumentExpression("value")] string name = "")
            {
                var defaultHandler = CreateDefaultHandler();
                defaultHandler.AppendFormatted(value, alignment);
                AppendFormatted((object)defaultHandler.ToStringAndClear(), name);
            }

            public void AppendFormatted<T>(T value, string format, [CallerArgumentExpression("value")] string name = "")
            {
                var defaultHandler = CreateDefaultHandler();
                defaultHandler.AppendFormatted(value, format);
                AppendFormatted((object)defaultHandler.ToStringAndClear(), name);
            }

            public void AppendFormatted<T>(T value, int alignment, string format, [CallerArgumentExpression("value")] string name = "")
            {
                var defaultHandler = CreateDefaultHandler();
                defaultHandler.AppendFormatted(value, alignment, format);
                AppendFormatted((object)defaultHandler.ToStringAndClear(), name);
            }

            public void AppendFormatted(object value, [CallerArgumentExpression("value")] string name = "")
            {
                name = InterpolatedHelper.EscapeName(name);

                MessageTemplate.Append('{');
                MessageTemplate.Append(name);
                MessageTemplate.Append('}');
                
                Properties.SetUnsafe(name, value, true);
            }

            internal bool IsEnabled { get; }
            internal StringBuilder MessageTemplate { get; } = null!;
            internal ImmutableArrayDictionary<string, object> Properties { get; private set; } = null!;

            private static DefaultInterpolatedStringHandler CreateDefaultHandler() =>
                new DefaultInterpolatedStringHandler(0, 1, CultureInfo.InvariantCulture);
        }

        #endregion

        #region Error

        /// <summary>
        /// Logs the given <paramref name="message"/> as interpolated string on the <see cref="LogLevel.Error"/> level.
        /// </summary>
        public static void Error(this ILog log, [InterpolatedStringHandlerArgument("log")] ref ErrorStringHandler message)
        {
            if (!message.IsEnabled)
                return;

            log.Log(new LogEvent(LogLevel.Error, PreciseDateTime.Now, message.MessageTemplate.ToString(), message.Properties, null));
        }

        /// <summary>
        /// Logs the given <paramref name="exception"/> on the <see cref="LogLevel.Error"/> level with the given <paramref name="message"/> as interpolated string.
        /// </summary>
        public static void Error(this ILog log, [CanBeNull] Exception exception, [InterpolatedStringHandlerArgument("log")] ref ErrorStringHandler message)
        {
            if (!message.IsEnabled)
                return;

            log.Log(new LogEvent(LogLevel.Error, PreciseDateTime.Now, message.MessageTemplate.ToString(), message.Properties, exception));
        }

        [PublicAPI]
        [InterpolatedStringHandler]
        public ref struct ErrorStringHandler
        {
            public ErrorStringHandler(int literalLength, int formattedCount, ILog log, out bool isEnabled)
            {
                IsEnabled = isEnabled = log.IsEnabledFor(LogLevel.Error);
                if (!isEnabled)
                    return;

                MessageTemplate = new StringBuilder(literalLength);

                if (formattedCount > 0)
                    Properties = LogEvent.CreateProperties(Math.Max(4, formattedCount));
            }

            public void AppendLiteral(string value)
            {
                MessageTemplate.Append(value);
            }

            public void AppendFormatted<T>(T value, int alignment, [CallerArgumentExpression("value")] string name = "")
            {
                var defaultHandler = CreateDefaultHandler();
                defaultHandler.AppendFormatted(value, alignment);
                AppendFormatted((object)defaultHandler.ToStringAndClear(), name);
            }

            public void AppendFormatted<T>(T value, string format, [CallerArgumentExpression("value")] string name = "")
            {
                var defaultHandler = CreateDefaultHandler();
                defaultHandler.AppendFormatted(value, format);
                AppendFormatted((object)defaultHandler.ToStringAndClear(), name);
            }

            public void AppendFormatted<T>(T value, int alignment, string format, [CallerArgumentExpression("value")] string name = "")
            {
                var defaultHandler = CreateDefaultHandler();
                defaultHandler.AppendFormatted(value, alignment, format);
                AppendFormatted((object)defaultHandler.ToStringAndClear(), name);
            }

            public void AppendFormatted(object value, [CallerArgumentExpression("value")] string name = "")
            {
                name = InterpolatedHelper.EscapeName(name);

                MessageTemplate.Append('{');
                MessageTemplate.Append(name);
                MessageTemplate.Append('}');
                
                Properties.SetUnsafe(name, value, true);
            }

            internal bool IsEnabled { get; }
            internal StringBuilder MessageTemplate { get; } = null!;
            internal ImmutableArrayDictionary<string, object> Properties { get; private set; } = null!;

            private static DefaultInterpolatedStringHandler CreateDefaultHandler() =>
                new DefaultInterpolatedStringHandler(0, 1, CultureInfo.InvariantCulture);
        }

        #endregion

        #region Fatal

        /// <summary>
        /// Logs the given <paramref name="message"/> as interpolated string on the <see cref="LogLevel.Fatal"/> level.
        /// </summary>
        public static void Fatal(this ILog log, [InterpolatedStringHandlerArgument("log")] ref FatalStringHandler message)
        {
            if (!message.IsEnabled)
                return;

            log.Log(new LogEvent(LogLevel.Fatal, PreciseDateTime.Now, message.MessageTemplate.ToString(), message.Properties, null));
        }

        /// <summary>
        /// Logs the given <paramref name="exception"/> on the <see cref="LogLevel.Fatal"/> level with the given <paramref name="message"/> as interpolated string.
        /// </summary>
        public static void Fatal(this ILog log, [CanBeNull] Exception exception, [InterpolatedStringHandlerArgument("log")] ref FatalStringHandler message)
        {
            if (!message.IsEnabled)
                return;

            log.Log(new LogEvent(LogLevel.Fatal, PreciseDateTime.Now, message.MessageTemplate.ToString(), message.Properties, exception));
        }

        [PublicAPI]
        [InterpolatedStringHandler]
        public ref struct FatalStringHandler
        {
            public FatalStringHandler(int literalLength, int formattedCount, ILog log, out bool isEnabled)
            {
                IsEnabled = isEnabled = log.IsEnabledFor(LogLevel.Fatal);
                if (!isEnabled)
                    return;

                MessageTemplate = new StringBuilder(literalLength);

                if (formattedCount > 0)
                    Properties = LogEvent.CreateProperties(Math.Max(4, formattedCount));
            }

            public void AppendLiteral(string value)
            {
                MessageTemplate.Append(value);
            }

            public void AppendFormatted<T>(T value, int alignment, [CallerArgumentExpression("value")] string name = "")
            {
                var defaultHandler = CreateDefaultHandler();
                defaultHandler.AppendFormatted(value, alignment);
                AppendFormatted((object)defaultHandler.ToStringAndClear(), name);
            }

            public void AppendFormatted<T>(T value, string format, [CallerArgumentExpression("value")] string name = "")
            {
                var defaultHandler = CreateDefaultHandler();
                defaultHandler.AppendFormatted(value, format);
                AppendFormatted((object)defaultHandler.ToStringAndClear(), name);
            }

            public void AppendFormatted<T>(T value, int alignment, string format, [CallerArgumentExpression("value")] string name = "")
            {
                var defaultHandler = CreateDefaultHandler();
                defaultHandler.AppendFormatted(value, alignment, format);
                AppendFormatted((object)defaultHandler.ToStringAndClear(), name);
            }

            public void AppendFormatted(object value, [CallerArgumentExpression("value")] string name = "")
            {
                name = InterpolatedHelper.EscapeName(name);

                MessageTemplate.Append('{');
                MessageTemplate.Append(name);
                MessageTemplate.Append('}');
                
                Properties.SetUnsafe(name, value, true);
            }

            internal bool IsEnabled { get; }
            internal StringBuilder MessageTemplate { get; } = null!;
            internal ImmutableArrayDictionary<string, object> Properties { get; private set; } = null!;

            private static DefaultInterpolatedStringHandler CreateDefaultHandler() =>
                new DefaultInterpolatedStringHandler(0, 1, CultureInfo.InvariantCulture);
        }

        #endregion

    }
}

#endif