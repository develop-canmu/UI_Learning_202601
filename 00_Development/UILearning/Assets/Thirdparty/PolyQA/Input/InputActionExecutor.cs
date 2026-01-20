using System;
using System.Collections;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using PolyQA.Extensions;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PolyQA.Input
{
    public interface IInputActionContext
    {
        Coroutine StartSubAction(IInputAction action);
    }

    public interface IInputAction
    {
        IEnumerator Process(IInputActionContext context);
    }

    public class InputActionExecutor : IInputActionContext
    {
        private readonly ILogger _logger = Logging.CreateLogger<InputActionExecutor>();
        private readonly ConcurrentQueue<IInputAction> _actions = new();
        private readonly InputActionExecutorComponent _component;
        private IInputAction _currentAction;

        public InputActionExecutor(InputActionExecutorComponent component)
        {
            _component = component;
            _component.StartCoroutine(Process());
        }

        public void Execute(IInputAction action)
        {
            _logger.LogDebug("Enqueue action: {Action}", action);
            _actions.Enqueue(action);
        }

        private IEnumerator Process()
        {
            while (true)
            {
                if (_actions.TryDequeue(out var action))
                {
                    _logger.LogDebug("Start action: {Action}", action);
                    yield return _component.StartCoroutine(() => action.Process(this), OnActionError);
                    _logger.LogDebug("End action: {Action}", action);
                }

                yield return null;
            }
        }

        private void OnActionError(Exception exception)
        {
            _logger.LogError(exception, "Action error: {Exception}", exception);
        }

        public Coroutine StartSubAction(IInputAction action)
        {
            return _component.StartCoroutine(action.Process(this));
        }
    }
}