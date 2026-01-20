#if UNITY_EDITOR

namespace Pjfb
{
    public static class PerformanceSample
    {
        public static void TestSampleMethodByLoopCount()
        {
            PerformanceTestUtility.TestFunctionByLoopCount(LightMethod, HeavyMethod, 100000, "LightMethod()", "HeavyMethod()");
        }

        public static void TestSampleMethodByTime()
        {
            PerformanceTestUtility.TestFunctionByTime(LightMethod, HeavyMethod, 5000, "LightMethod()", "HeavyMethod()");
        }
        
        private static void LightMethod()
        {
            Factorial(1000);
        }

        private static void HeavyMethod()
        {
            Factorial(10000);
        }
        
        
        private static long Factorial(int n)
        {
            if (n == 0)
                return 1;
            else
                return n * Factorial(n - 1);
        }
    }
}
#endif