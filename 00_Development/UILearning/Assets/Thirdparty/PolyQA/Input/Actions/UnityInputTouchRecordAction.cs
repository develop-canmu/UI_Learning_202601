using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyQA.Input.Actions
{
    public class UnityInputTouchRecordAction : IInputAction
    {
        private readonly InputTouchRecordSequence _sequence;
        private readonly InputState _state;
        private readonly HashSet<int> _removeTouches = new();

        public UnityInputTouchRecordAction(RuntimeContext context, InputTouchRecordSequence sequence)
        {
            _state = context.InputState;
            _sequence = sequence;
        }

        public IEnumerator Process(IInputActionContext context)
        {
            foreach (var action in _sequence.Actions)
            {
                if (action.Delay > TimeSpan.Zero)
                {
                    yield return Wait(action.Delay);
                }

                var position = new Vector2(Screen.width * action.Position.X, Screen.height * action.Position.Y);
                var phase = ToTouchPhase(action.Action);
                _state.Touch(action.TouchId, position, phase);

                if(phase is TouchPhase.Ended || phase == TouchPhase.Canceled)
                {
                    _removeTouches.Add(action.TouchId);
                }
            }

            yield return null;
            RemoveTouches();
        }

        private IEnumerator Wait(TimeSpan delay)
        {
            var time = DateTime.Now;
            while (DateTime.Now - time < delay)
            {
                yield return null;
                RemoveTouches();
            }
        }

        private void RemoveTouches()
        {
            if (_removeTouches.Count > 0)
            {
                foreach (var touchId in _removeTouches)
                {
                    _state.RemoveTouch(touchId);
                }
                _removeTouches.Clear();
            }
        }

        private TouchPhase ToTouchPhase(string phase)
        {
            return phase switch
            {
                PointerAction.Down => TouchPhase.Began,
                PointerAction.Move => TouchPhase.Moved,
                PointerAction.Stationary => TouchPhase.Stationary,
                PointerAction.Up => TouchPhase.Ended,
                PointerAction.Cancel => TouchPhase.Canceled,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}