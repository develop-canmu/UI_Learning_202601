#if UNITY_EDITOR
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Pjfb
{
    public static class PerformanceTestUtility
    {
        private const int WarmupTime = 1000;

        public static void TestFunctionByLoopCount(Action testFunction1, Action testFunction2, ulong loopCount = 100000,
            string functionName1 = "Action1()", string functionName2 = "Action2()")
        {
            long ms1 = TestFunctionByLoopCount(testFunction1,loopCount,functionName1);
            long ms2 = TestFunctionByLoopCount(testFunction2,loopCount,functionName2);
            
            if(ms2 > ms1)
                Debug.Log($"[Performance Test] {functionName1} is {((double)ms2/ms1):0.##}x faster than {functionName2}");
            else if(ms1 > ms2)
                Debug.Log($"[Performance Test] {functionName2} is {((double)ms1/ms2):0.##}x faster than {functionName1}");
            else
                Debug.Log($"[Performance Test] {functionName1} is as fast as {functionName2}");
        }
        public static long TestFunctionByLoopCount(Action testFunction, ulong loopCount = 100000, string functionName = "Action()")
        {
            if (testFunction is null)
            {
                Debug.Log("[Performance Test] Action is null!");
                return 0;
            }
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Debug.Log("[Performance Test] Warmup...");
            while (sw.ElapsedMilliseconds < WarmupTime)
            {
                testFunction();
            }
            
            sw.Stop();
            sw.Reset();
            sw.Start();
            for (ulong i = 0; i < loopCount; i++)
            {
                testFunction();
            }
            sw.Stop();
            Debug.Log($"[Performance Test] Function {functionName} completed {loopCount} loops in {sw.ElapsedMilliseconds}ms");
            return sw.ElapsedMilliseconds;
        }
        
        
         /// <summary>
         /// 
         /// </summary>
         /// <param name="testFunction1">First function to be tested</param>
         /// <param name="testFunction2">Second function to be tested</param>
         /// <param name="timer">Timer in ms</param>
         /// <param name="functionName1">First function name</param>
         /// <param name="functionName2">Second function name</param>
        public static void TestFunctionByTime(Action testFunction1, Action testFunction2, long timer = 5000,
            string functionName1 = "Action1()", string functionName2 = "Action2()")
        {
            long loopCount1 = TestFunctionByTime(testFunction1, timer, functionName1);
            long loopCount2 = TestFunctionByTime(testFunction2, timer, functionName2);
            
            if(loopCount1 > loopCount2)
                Debug.Log($"[Performance Test] {functionName1} is {((double)loopCount1/loopCount2):0.##}x faster than {functionName2}");
            else if(loopCount2 > loopCount1)
                Debug.Log($"[Performance Test] {functionName2} is {((double)loopCount2/loopCount1):0.##}x faster than {functionName1}");
            else
                Debug.Log($"[Performance Test] {functionName1} is as fast as {functionName2}");
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="testFunction">The function to be tested</param>
        /// <param name="timer">Timer in ms</param>
        /// <param name="functionName">Function name</param>
        public static long TestFunctionByTime(Action testFunction, long timer = 5000, string functionName = "Action()")
        {
            if (testFunction is null)
            {
                Debug.Log("[Performance Test] Action is null!");
                return 0;
            }
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Debug.Log("[Performance Test] Warmup...");
            while (sw.ElapsedMilliseconds < WarmupTime)
            {
                testFunction();
            }
            
            sw.Stop();
            sw.Reset();
            sw.Start();
            int loopCount = 0;
            while (sw.ElapsedMilliseconds < timer)
            {
                testFunction();
                loopCount++;
            }
   
            sw.Stop();
            Debug.Log($"[Performance Test] Function {functionName} completed {loopCount} loops in {sw.ElapsedMilliseconds}ms");
            return loopCount;
        }
    }
}
#endif