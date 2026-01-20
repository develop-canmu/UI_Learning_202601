// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("SODOcV5aQluuY55YfmqrTj7yqGu2l+RjPL2aj0fg/TXp4Y9etgktdFFZjcFZsiYXBktORm8/sywZxIrFy7jMAsLYmAx19yQyUnh10w8VkkW/reVWRpEzNN4L4EvpIY1Mp9zcolh1xrmraSh8tR+l0usk1xswZG5b42BuYVHjYGtj42BgYco3i9lSGKaoC8sGSpWamrjCzxa6ZwWJPCUS4FHjYENRbGdoS+cp55ZsYGBgZGFiN0O0RklPqyaLdO99T7Tp3WSssDbQV26NEkyNe//fNLLCSD0/OthR9tdhPyEfkfAy///NlCNjkrLgRy3VZuz8RZ2RyZU8hoI23yVNQ96lMz1FK5r0lTcDI+S0Pg5khVR2vfFqFmJAYX6HHVhYLmNiYGFg");
        private static int[] order = new int[] { 3,4,10,13,10,6,10,10,13,12,10,13,12,13,14 };
        private static int key = 97;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
