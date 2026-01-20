using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PolyQA.Input
{
    public class InputState
    {
        private readonly ILogger _logger = Logging.CreateLogger<InputState>();

        public bool IsMouseEnabled { get; set; } = true;

        public bool IsTouchEnabled { get; set; } = true;

        public List<Touch> Touches { get; } = new ();

        public event Action<Touch> OnTouched;

        public void Touch(int id, Vector2 position, TouchPhase phase)
        {
            var touch = CreateTouch(id, position, phase);
            for(int i = 0; i < Touches.Count; i++)
            {
                if (Touches[i].fingerId == id)
                {
                    Touches[i] = touch;
                    OnTouched?.Invoke(touch);
                    _logger.LogDebug("({Frame}) Update touch({ID}) {Position} {Phase}",
                        Time.frameCount, id, position, phase);
                    return;
                }
            }
            Touches.Add(touch);
            OnTouched?.Invoke(touch);
            _logger.LogDebug("({Frame}) Add touch({ID}) {Position} {Phase}",
                Time.frameCount, id, position, phase);
        }

        public void RemoveTouch(int id)
        {
            for(int i = 0; i < Touches.Count; i++)
            {
                if (Touches[i].fingerId == id)
                {
                    Touches.RemoveAt(i);
                    _logger.LogDebug("({Frame}) Remove touch({ID})", Time.frameCount, id);
                    return;
                }
            }
        }

        private Touch CreateTouch(int id, Vector2 position, TouchPhase phase)
        {
            return new Touch
            {
                fingerId = id,
                position = position,
                rawPosition = position,
                phase = phase,
                tapCount = 1,
            };
        }
    }
}