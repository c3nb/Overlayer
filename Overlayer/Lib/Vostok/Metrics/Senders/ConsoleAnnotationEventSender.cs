using System;
using JetBrains.Annotations;
using Vostok.Metrics.Models;

namespace Vostok.Metrics.Senders
{
    /// <summary>
    /// Prints incoming <see cref="AnnotationEvent"/>s to system console.
    /// </summary>
    [PublicAPI]
    public class ConsoleAnnotationEventSender : TextAnnotationEventSender
    {
        public ConsoleAnnotationEventSender()
            : base(Console.Out.WriteLine)
        {
        }
    }
}
