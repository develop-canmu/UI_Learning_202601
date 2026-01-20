#if POLYQA_USE_UGUI

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using PolyQA.Input;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PolyQA.Record
{
    public class UnityLegacyInputRecorder : IInputRecorder
    {
        private readonly ILogger _logger = Logging.CreateLogger<UnityLegacyInputRecorder>();
        private readonly RuntimeContext _context;
        private readonly List<TouchHistory> _touchHistories = new();
        private List<TouchHistory> _lastFrameTouches = new();
        private List<TouchHistory> _currentFrameTouches = new();
        private DateTime _lastSentActionTime;

        public UnityLegacyInputRecorder(RuntimeContext context)
        {
            _context = context;
        }

        public void Update()
        {
            var eventSystem = EventSystem.current;
            if (!eventSystem) return;
            var module = eventSystem.currentInputModule;
            if (!module) return;
            var input = module.input;
            if (input is UnityLegacyInputModifier modifier)
            {
                input = modifier.OriginalInput;
            }

            UpdateTouch(input);
        }

        private void UpdateTouch(BaseInput input)
        {
            if (input.touchCount > 0)
            {
                var time = DateTime.Now;
                for(var i = 0; i < input.touchCount; i++)
                {
                    var touch = input.GetTouch(i);
                    var history = new TouchHistory(touch.fingerId, touch.position, touch.phase, time);
                    _currentFrameTouches.Add(history);

                    var lastTouch = _lastFrameTouches.FirstOrDefault(x => x.FingerId == touch.fingerId);

                    if (touch.phase == lastTouch?.Phase && touch.position == lastTouch.Position)
                    {
                        continue;
                    }

                    _touchHistories.Add(history);
                }

                SwapFrameTouches();
            }
            else
            {
                if (_touchHistories.Count == 0) return;

                using (ListPool<InputTouchRecordAction>.Get(out var actions))
                {
                    DateTime lastActionTime = default;
                    foreach (var history in _touchHistories)
                    {
                        var action = new InputTouchRecordAction(
                            history.FingerId,
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
                }

                _touchHistories.Clear();
                _lastFrameTouches.Clear();
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
            public int FingerId { get; }
            public Vector2 Position { get; }
            public TouchPhase Phase { get; }
            public DateTime Time { get; }

            public TouchHistory(int fingerId, Vector2 position, TouchPhase phase, DateTime time)
            {
                FingerId = fingerId;
                Position = position;
                Phase = phase;
                Time = time;
            }
        }

    }
}
#endif