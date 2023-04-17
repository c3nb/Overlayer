using System;
using System.Runtime.ExceptionServices;

namespace Overlayer.Core.ExceptionHandling
{
    public static class ExceptionCatcher
    {
        public static void Catch()
        {
            AppDomain.CurrentDomain.FirstChanceException += All;
            AppDomain.CurrentDomain.UnhandledException += NotCatched;
        }
        public static void Drop()
        {
            AppDomain.CurrentDomain.FirstChanceException -= All;
            AppDomain.CurrentDomain.UnhandledException -= NotCatched;
        }
        public static event CatchedEvent AllExceptions = delegate { };
        public static event CatchedEvent Unhandled = delegate { };
        private static void All(object sender, FirstChanceExceptionEventArgs e)
            => AllExceptions(e.Exception);
        private static void NotCatched(object sender, UnhandledExceptionEventArgs e)
            => Unhandled(e.ExceptionObject as Exception);
    }
    public delegate void CatchedEvent(Exception exception);
}
