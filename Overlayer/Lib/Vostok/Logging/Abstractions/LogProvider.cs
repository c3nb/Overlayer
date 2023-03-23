using System;
using JetBrains.Annotations;

namespace Vostok.Logging.Abstractions
{
    /// <summary>
    /// <para><see cref="LogProvider"/> is a static shared configuration point that allows to decouple configuration of logging in libraries from calling code.</para>
    /// <para>It is intended to be used primarily by library developers who must not force their users to explicitly provide <see cref="ILog"/> instances.</para>
    /// <para>It is expected to be configured by a hosting system or just directly in the application entry point.</para>
    /// </summary>
    [PublicAPI]
    public static class LogProvider
    {
        private static readonly ILog DefaultInstance = new SilentLog();

        private static volatile ILog instance;

        /// <summary>
        /// Returns <c>true</c> if a global <see cref="ILog"/> instance has already been configured with <see cref="Configure"/> method. Returns <c>false</c> otherwise.
        /// </summary>
        public static bool IsConfigured => instance != null;

        /// <summary>
        /// <para>Returns the global default instance of <see cref="ILog"/> if it's been configured.</para>
        /// <para>If nothing has been configured yet, falls back to an instance of <see cref="SilentLog"/>.</para>
        /// </summary>
        [NotNull]
        public static ILog Get() => instance ?? DefaultInstance;

        /// <summary>
        /// <para>Configures the global default <see cref="ILog"/> with given instance, which will be returned by all subsequent <see cref="Get"/> calls.</para>
        /// <para>By default, this method fails when trying to overwrite a previously configured instance. This behaviour can be changed with <paramref name="canOverwrite"/> parameter.</para>
        /// </summary>
        /// <exception cref="ArgumentNullException">Provided instance was <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Attempted to overwrite previously configured instance.</exception>
        public static void Configure([NotNull] ILog log, bool canOverwrite = false)
        {
            if (!canOverwrite && instance != null)
                throw new InvalidOperationException($"Can't overwrite existing configured log implementation of type '{instance.GetType().Name}'.");

            instance = log ?? throw new ArgumentNullException(nameof(log));
        }
    }
}
