#if POLYQA_USE_INPUT_SYSTEM

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PolyQA.Record
{
    public class UnityInputSystemRecorder : IInputRecorder
    {
        private readonly ILogger _logger = Logging.CreateLogger<UnityInputSystemRecorder>();
        private readonly RuntimeContext _context;
        private readonly List<TouchHistory> _touchHistories = new();
        private List<TouchHistory> _lastFrameTouches = new();
        private List<TouchHistory> _currentFrameTouches = new();
        private DateTime _lastSentActionTime;

        public UnityInputSystemRecorder(RuntimeContext context)
        {
            _context = context;
        }

        public void Update()
        {
            var touchscreen = InputSystem.devices
                .OfType<Touchscreen>()
                .OrderByDescending(x => x.lastUpdateTime)
                .FirstOrDefault(x => !x.name.Contains("PolyQA"));

            if (touchscreen == null) return;

            UpdateTouch(touchscreen);
        }

        private void UpdateTouch(Touchscreen touchscreen)
        {
            var touchCount = 0;
            var time = DateTime.Now;

            foreach (var touch in touchscreen.touches)
            {
                var phase = touch.phase.value;
                if (phase is TouchPhase.None) continue;

                var lastTouch = _lastFrameTouches.FirstOrDefault(x => x.TouchId == touch.touchId.value);
                if (phase is TouchPhase.Ended or TouchPhase.Canceled)
                {
                    if (lastTouch == null || lastTouch.Phase is TouchPhase.Ended or TouchPhase.Canceled)
                    {
                        continue;
                    }
                }

                touchCount++;
                var position = touch.position.value;
                var history = new TouchHistory(touch.touchId.value, position, phase, time);
                _currentFrameTouches.Add(history);

                if (phase == lastTouch?.Phase && position == lastTouch.Position)
                {
                    continue;
                }

                _touchHistories.Add(history);
            }

            SwapFrameTouches();

            if (touchCount == 0)
            {
                if (_touchHistories.Count == 0) return;

                using (ListPool<InputTouchRecordAction>.Get(out var actions))
                {
                    DateTime lastActionTime = default;
                    foreach (var history in _touchHistories)
                    {
                        var action = new InputTouchRecordAction(
                            history.TouchId - 1, // New Input System は 1 から始まる
                            MakeTouchPhase(history.Phase),
                            lastActionTime == default ? TimeSpan.Zero : history.Time - lastActionTime,
                            MakeScreenPosition(history.Position)
                        );

                        actions.Add(action);
                        lastActionTime = history.Time;
                    }

                    var sequence = new InputTouchRecordSequence(actions.ToArray());

                    var actionTime = _touchHistories[0].Time;
                    var delay = _lastSentActionTime == default ?
                        TimeSpan.Zero : DateTime.Now - _lastSentActionTime;
                    _lastSentActionTime = actionTime;

                    _context.InputRecordDataSender.SendInputRecord(new IInputActionSequence[]{ sequence }, delay);

                    _touchHistories.Clear();
                }
            }
        }

        private System.Numerics.Vector2 MakeScreenPosition(Vector2 position)
        {
            return new System.Numerics.Vector2(position.x / Screen.width, position.y / Screen.height);
        }

        private string MakeTouchPhase(TouchPhase phase)
        {
            return phase switch
            {
                TouchPhase.Began => PointerAction.Down,
                TouchPhase.Moved => PointerAction.Move,
                TouchPhase.Stationary => PointerAction.Stationary,
                TouchPhase.Ended => PointerAction.Up,
                TouchPhase.Canceled => PointerAction.Cancel,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void SwapFrameTouches()
        {
            (_lastFrameTouches, _currentFrameTouches) = (_currentFrameTouches, _lastFrameTouches);
            _currentFrameTouches.Clear();
        }

        private class TouchHistory
        {
            public int TouchId { get; }
            public Vector2 Position { get; }
            public TouchPhase Phase { get; }
            public DateTime Time { get; }

            public TouchHistory(int touchId, Vector2 position, TouchPhase phase, DateTime time)
            {
                TouchId = touchId;
                Position = position;
                Phase = phase;
                Time = time;
            }
        }
    }
}

#endif