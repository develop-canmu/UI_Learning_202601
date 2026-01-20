using System.Runtime.CompilerServices;
using PolyQA.Network;

namespace PolyQA
{
    public class DataSender
    {
#if POLYQA_DISABLE
        [System.Diagnostics.Conditional("YOUR_PROJECT_NAME_NEVER_DEFINED_SYMBOL")]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Send(string key) => DataSenderInternal.Send(key);

#if POLYQA_DISABLE
        [System.Diagnostics.Conditional("YOUR_PROJECT_NAME_NEVER_DEFINED_SYMBOL")]
#endif
        public static void Send<T>(string key, T value) => DataSenderInternal.Send(key, value);

#if POLYQA_DISABLE
        [System.Diagnostics.Conditional("YOUR_PROJECT_NAME_NEVER_DEFINED_SYMBOL")]
#endif
        public static void RegisterInputCommand(string command, IInputReceiver receiver)
            => DataSenderInternal.RegisterInputCommand(command, receiver);

#if POLYQA_DISABLE
        [System.Diagnostics.Conditional("YOUR_PROJECT_NAME_NEVER_DEFINED_SYMBOL")]
#endif
        public static void UnregisterInputCommand(string command)
            => DataSenderInternal.UnRegisterInputCommand(command);

#if POLYQA_DISABLE
        [System.Diagnostics.Conditional("YOUR_PROJECT_NAME_NEVER_DEFINED_SYMBOL")]
#endif
        public static void LaunchManual()
        {
#if !POLYQA_DISABLE
            DataSenderMonoBehaviour.Launch();
#endif
        }
    }
}