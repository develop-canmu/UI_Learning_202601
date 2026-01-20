#nullable enable
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Polyscape.RPCLite
{
    public static class TaskExtensions
    {
        public static void Forget(this Task task, ILogger logger)
        {
            task.ContinueWith(
                t => logger.LogError(t.Exception, "Task {id} threw an exception", t.Id),
                TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}

