using System.Collections;
using Microsoft.Extensions.Logging;
using PolyQA.Observer;
using UnityEngine.UI;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PolyQA.Input.Actions
{
    public class UnityInputTextAction : IInputAction
    {
        private readonly ILogger _logger = Logging.CreateLogger<UnityInputTextAction>();
        private readonly GameObjectDataService _gameObjectDataService;
        private readonly InputTextSequence _sequence;

        public UnityInputTextAction(RuntimeContext context, InputTextSequence sequence)
        {
            _gameObjectDataService = context.GameObjectDataService;
            _sequence = sequence;
        }

        public IEnumerator Process(IInputActionContext context)
        {
            InputText();
            yield break;
        }

        private void InputText()
        {
            var path = _sequence.Path;
            var go = _gameObjectDataService.FindGameObject(path);
            if (go == null)
            {
                _logger.LogError("GameObject not found: {Path}", path);
                return;
            }
            if (!go.Instance)
            {
                _logger.LogError("GameObject instance not found: {Path}", path);
                return;
            }

            var input = go.Instance.GetComponent<InputField>();
            if (input)
            {
                input.text = _sequence.Text;
                return;
            }

#if POLYQA_USE_TEXTMESHPRO
            var tmpInput = go.Instance.GetComponent<TMPro.TMP_InputField>();
            if (tmpInput)
            {
                tmpInput.text = _sequence.Text;
            }
#endif
        }
    }
}