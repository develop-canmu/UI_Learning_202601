using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PolyQA.Extensions
{
    public static class TaskExtensions
    {
        public static void HandleException(this Task task, ILogger logger)
        {
            task.ContinueWith(
                t => HandleExceptionInternal(t, logger),
                TaskContinuationOptions.OnlyOnFaulted);
        }
        
        private static void HandleExceptionInternal(Task task, ILogger logger)
        {
            if (task.Exception == null) return;
            
            foreach (var exception in task.Exception.InnerExceptions)
            {
                if (exception is ConnectException)
                {
                    logger.LogDebug(exception.InnerException, "Task {id} threw an exception", task.Id);
                }
                else
                {
                    logger.LogError(exception, "Task {id} threw an exception", task.Id);
                }
            }
        }
    }
}

