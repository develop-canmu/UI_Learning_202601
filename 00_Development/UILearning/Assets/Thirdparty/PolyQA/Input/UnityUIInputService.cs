#if POLYQA_USE_UGUI
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

#if POLYQA_USE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace PolyQA.Input
{
    public class UnityUIInputService : IInputServiceImpl, IInputServiceUpdate
    {
        private readonly ILogger _logger = Logging.CreateLogger<UnityUIInputService>();
        private readonly InputState _state;
        private readonly HashSet<BaseInputModule> _modules = new ();
        private readonly Dictionary<BaseInput, UnityLegacyInputModifier> _modifiers = new ();

        public UnityUIInputService(InputState state)
        {
            _state = state;
        }

        public void Update()
        {
            CleanList();

            EventSystem eventSystem = EventSystem.current;
            if (!eventSystem) return;
            BaseInputModule inputModule = eventSystem.currentInputModule;
            if (!inputModule) return;
            if (!_modules.Add(inputModule)) return;

#if POLYQA_USE_INPUT_SYSTEM
            if (inputModule is InputSystemUIInputModule)
            {
                _logger.LogDebug("InputSystemUIInputModule is used. Skip override");
                return;
            }
#endif

            var gameInput = inputModule.input;
            if (gameInput is UnityLegacyInputModifier) return;

            if (_modifiers.TryGetValue(gameInput, out UnityLegacyInputModifier modifier) && modifier)
            {
                inputModule.inputOverride = modifier;
                return;
            }

            modifier = inputModule.gameObject.AddComponent<UnityLegacyInputModifier>();
            modifier.Init(gameInput, _state);
            inputModule.inputOverride = modifier;
            _modifiers[gameInput] = modifier;
            _logger.LogDebug("Legacy input system override. module: {Module} input: {Input}", inputModule, gameInput);
        }

        private void CleanList()
        {
            using (ListPool<BaseInputModule>.Get(out var removeModules))
            {
                foreach (BaseInputModule m in _modules)
                {
                    if (!m) removeModules.Add(m);
                }
                foreach (BaseInputModule m in removeModules)
                {
                    _modules.Remove(m);
                }
            }

            using (ListPool<BaseInput>.Get(out var removeInputs))
            {
                foreach (var p in _modifiers)
                {
                    if (!p.Key)
                    {
                        removeInputs.Add(p.Key);
                    }
                }
                foreach (BaseInput k in removeInputs)
                {
                    _modifiers.Remove(k);
                }
            }
        }
    }
}

#endif