using System;
using Microsoft.Extensions.Logging;

namespace PolyQA.Extensions
{
    public static class LoggerExtensions
    {
        public static void LogExceptionAll(this ILogger logger, Exception exception)
        {
            var ex = exception;
            var i = 1;
            while (ex != null)
            {
                logger.LogError(ex, "Exception ({i})", i++);
                ex = ex.InnerException;
            }
        }
    }
}