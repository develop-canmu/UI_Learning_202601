#if POLYQA_USE_INPUT_SYSTEM

using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PolyQA.Input
{
    public class UnityInputSystemService : IInputServiceImpl, IDisposable
    {
        private readonly ILogger _logger = Logging.CreateLogger<UnityInputSystemService>();
        private readonly InputState _state;
        private readonly Touchscreen _touchscreen;
        public UnityInputSystemService(InputState inputState)
        {
            _state = inputState;
            _touchscreen = InputSystem.AddDevice<Touchscreen>("PolyQA Touchscreen");
            _state.OnTouched += OnTouched;
        }

        private void OnTouched(Touch touch)
        {
            InputSystem.QueueStateEvent(_touchscreen,
                new TouchState
                {
                    phase = ToInputSystem(touch.phase),
                    touchId = touch.fingerId + 1, // New Input System は 1 から始まる
                    position = touch.position,
                });
        }

        private UnityEngine.InputSystem.TouchPhase ToInputSystem(UnityEngine.TouchPhase phase)
        {
            switch (phase)
            {
                case UnityEngine.TouchPhase.Began:
                    return UnityEngine.InputSystem.TouchPhase.Began;
                case UnityEngine.TouchPhase.Moved:
                    return UnityEngine.InputSystem.TouchPhase.Moved;
                case UnityEngine.TouchPhase.Stationary:
                    return UnityEngine.InputSystem.TouchPhase.Stationary;
                case UnityEngine.TouchPhase.Ended:
                    return UnityEngine.InputSystem.TouchPhase.Ended;
                case UnityEngine.TouchPhase.Canceled:
                    return UnityEngine.InputSystem.TouchPhase.Canceled;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, "Unexpected value");
            }
        }

        public void Dispose()
        {
            InputSystem.RemoveDevice(_touchscreen);
            _state.OnTouched -= OnTouched;
        }
    }
}

#endif