using System;
using System.Collections;
using System.IO;
using Microsoft.Extensions.Logging;
using PolyQA.Extensions;
using PolyQA.Observer;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PolyQA.Input.Actions
{
    public class UnityInputTouchAction : IInputAction
    {
        private readonly ILogger _logger = Logging.CreateLogger<UnityInputTouchAction>();
        private readonly GameObjectDataService _gameObjectDataService;
        private readonly InputTouchActionSequence _sequence;
        private readonly InputState _state;
        private TimeSpan _delay;
        private Vector2 _position;
        private Vector2 _targetPosition;
        private bool _isTouch;
        private TouchPhase _phase;
        private string _nextAction;

        public UnityInputTouchAction(RuntimeContext context, InputTouchActionSequence sequence)
        {
            _gameObjectDataService = context.GameObjectDataService;
            _state = context.InputState;
            _sequence = sequence;
        }

        private void StartAction(IInputPointerAction action)
        {
            _position = _targetPosition;
            PointerAction pointerAction = null;
            switch (action)
            {
                case GameObjectPointerAction x:
                    pointerAction = x;
                    UpdateTargetPosition(x);
                    break;
                case PixelPointerAction x:
                    pointerAction = x;
                    UpdateTargetPosition(x);
                    break;
                case ScreenPointerAction x:
                    pointerAction = x;
                    UpdateTargetPosition(x);
                    break;
                case PointerAction x:
                    pointerAction = x;
                    break;
            }

            if (pointerAction == null) throw CreateInvalidDataException();

            _delay = pointerAction.Delay;
            _nextAction = pointerAction.Action;
        }

        private void UpdateTargetPosition(GameObjectPointerAction action)
        {
            var path = action.Path;
            GameObjectData data = _gameObjectDataService.FindGameObject(path);
            if (data == null)
            {
                _logger.LogError("GameObject not found: {Path}", path);
                return;
            }
            if (!data.Instance)
            {
                _logger.LogError("GameObject instance not found: {Path}", path);
                return;
            }
            _targetPosition = data.Instance.GetScreenPosition();
        }

        private void UpdateTargetPosition(PixelPointerAction action)
        {
            _targetPosition = new Vector2(action.Position.X, action.Position.Y);
        }

        private void UpdateTargetPosition(ScreenPointerAction action)
        {
            _targetPosition = new Vector2(Screen.width * action.Position.X, Screen.height * action.Position.Y);
        }

        private Exception CreateInvalidDataException() =>
            // TODO test
            new InvalidDataException($"Invalid InputPointerActionSequence {JsonUtility.ToJson(_sequence)}");

        public IEnumerator Process(IInputActionContext context)
        {
            foreach (var action in _sequence.Actions)
            {
                StartAction(action);
                var startTime = DateTime.Now;

                while (true)
                {
                    var time = DateTime.Now - startTime;

                    if (time < _delay)
                    {
                        ProcessTouch(time);
                        yield return null;
                    }
                    else
                    {
                        TouchAction();
                        yield return null;
                        break;
                    }
                }
            }

            RemoveTouch();
        }

        private void ProcessTouch(TimeSpan time)
        {
            if (!_isTouch) return;
            if (_phase is TouchPhase.Ended or TouchPhase.Canceled)
            {
                _state.RemoveTouch(_sequence.FingerId);
                return;
            }

            if (_phase is TouchPhase.Stationary) return;


            if (_nextAction == PointerAction.Move || VectorUtils.Approximately(_position, _targetPosition))
            {
                // Moveの場合は細かい座標情報があるので補完しない
                _phase = TouchPhase.Stationary;
                _state.Touch(_sequence.FingerId, _position, _phase);
                return;
            }

            var newPosition = Vector2.Lerp(_position, _targetPosition, (float)(time / _delay));
            _phase = TouchPhase.Moved;
            _state.Touch(_sequence.FingerId, newPosition, _phase);
        }

        private void TouchAction()
        {
            _isTouch = true;
            switch (_nextAction)
            {
                case PointerAction.Down:
                    _state.Touch(_sequence.FingerId, _targetPosition, TouchPhase.Began);
                    break;
                case PointerAction.Up:
                    _state.Touch(_sequence.FingerId, _targetPosition, TouchPhase.Ended);
                    break;
                case PointerAction.Move:
                    _state.Touch(_sequence.FingerId, _targetPosition, TouchPhase.Moved);
                    break;
                case PointerAction.Cancel:
                    _state.Touch(_sequence.FingerId, _targetPosition, TouchPhase.Canceled);
                    break;
            }
        }

        private void RemoveTouch()
        {
            if (_isTouch)
            {
                _state.RemoveTouch(_sequence.FingerId);
                _isTouch = false;
            }
        }
    }
}
