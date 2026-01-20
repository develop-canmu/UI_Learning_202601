using System;
using System.Collections;
using UnityEngine;

namespace PolyQA.Extensions
{
    public static class MonoBehaviourExtensions
    {
        public static Coroutine StartCoroutine(
            this MonoBehaviour self, Func<IEnumerator> routine, Action<Exception> onError = null)
        {
            return self.StartCoroutine(CreateRoutine(routine, onError));
        }

        private static IEnumerator CreateRoutine(Func<IEnumerator> func, Action<Exception> onError = null)
        {
            IEnumerator routine;
            try
            {
                routine = func();
            }
            catch (Exception e)
            {
                onError?.Invoke(e);
                yield break;
            }

            while (true)
            {
                object current = null;
                Exception ex = null;
                try
                {
                    if (!routine.MoveNext())
                    {
                        break;
                    }
                    current = routine.Current;
                }
                catch (Exception e)
                {
                    ex = e;
                    onError?.Invoke(e);
                }

                if (ex != null)
                {
                    yield return ex;
                    yield break;
                }
                yield return current;
            }
        }
    }
}