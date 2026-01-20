using System;
using System.Threading;
using System.Threading.Tasks;

namespace PolyQA.Executor
{
    public class MainThreadDispatcher
    {
        private static SynchronizationContext _mainThreadContext;

        public static void SetMainThreadContext()
        {
            _mainThreadContext = SynchronizationContext.Current;
        }

        public static void Post(Action action)
        {
            _mainThreadContext.Post(_ => action(), null);
        }

        public static Task<T> Run<T>(Func<T> func)
        {
            var tcs = new TaskCompletionSource<T>();
            _mainThreadContext.Post(_ =>
            {
                try
                {
                    tcs.SetResult(func());
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            }, null);
            return tcs.Task;
        }
    }
}