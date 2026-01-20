using System;
using System.Collections;
using Microsoft.Extensions.Logging;
using PolyQA.Extensions;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PolyQA.Input.Actions
{
    public class ClickCommandAction : IInputAction
    {
        private const int FingerId = 0;
        private static readonly TimeSpan ClickTime = TimeSpan.FromMilliseconds(50);

        private readonly ILogger _logger = Logging.CreateLogger<ClickCommandAction>();
        private readonly GameObject _gameObject;
        private readonly InputState _state;
        private Vector2 _position;

        public ClickCommandAction(RuntimeContext context, GameObject gameObject)
        {
            _gameObject = gameObject;
            _state = context.InputState;
        }

        public IEnumerator Process(IInputActionContext context)
        {
            _position = _gameObject.GetScreenPosition();
            _logger.LogDebug("Click object: {Object} at {Position}", _gameObject, _position);
            _state.Touch(FingerId, _position, TouchPhase.Began);
            yield return null;

            _state.Touch(FingerId, _position, TouchPhase.Stationary);
            yield return new WaitForSeconds((float)ClickTime.TotalSeconds);

            _state.Touch(FingerId, _position, TouchPhase.Ended);
            yield return null;

            _state.RemoveTouch(FingerId);
        }
    }
}
