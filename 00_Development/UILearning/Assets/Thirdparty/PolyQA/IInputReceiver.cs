using System;
using System.Threading.Tasks;
using PolyQA.Input.Actions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;


namespace PolyQA
{
    public interface IInputReceiver
    {
        string Name { get; }
        void Execute();

        public static PointerEventData DummyPointerEventData = new PointerEventData(null);
    }

#if !POLYQA_DISABLE
    public class ButtonInputReceiver : IInputReceiver
    {
        private readonly RuntimeContext _context;
        private readonly Button _button;

        public ButtonInputReceiver(RuntimeContext context, Button button)
        {
            if (button == null)
            {
                throw new ArgumentNullException(nameof(button));
            }

            _context = context;
            _button = button;
            _button.onClick.AddListener(() =>
            {
                DataSender.Send("ButtonClicked", _button.name);
            });
        }

        public string Name => _button.name;

        public void Execute()
        {
            if(_button == null)
            {
                return;
            }

            var inputAction = new ClickCommandAction(_context, _button.gameObject);
            _context.InputActionExecutor.Execute(inputAction);
        }
    }

    public class PointerClickHandlerInputReceiver : IInputReceiver
    {
        private readonly IPointerClickHandler _pointerClickHandler;

        public PointerClickHandlerInputReceiver(IPointerClickHandler pointerClickHandler)
        {
            _pointerClickHandler = pointerClickHandler;
        }

        public string Name
        {
            get
            {
                if(_pointerClickHandler is Object obj)
                {
                    return obj.name;
                }
                return "UnknownPointerClickHandlerInputReceiver";
            }
        }
        public void Execute()
        {
            if (_pointerClickHandler != null)
            {
                _pointerClickHandler.OnPointerClick(IInputReceiver.DummyPointerEventData);
            }
        }
    }

    public class PointerDownHandlerInputReceiver : IInputReceiver
    {
        private readonly IPointerDownHandler _pointerDownHandler;

        public PointerDownHandlerInputReceiver(IPointerDownHandler pointerDownHandler)
        {
            _pointerDownHandler = pointerDownHandler;
        }

        public string Name
        {
            get
            {
                if(_pointerDownHandler is Object obj)
                {
                    return obj.name;
                }
                return "UnknownPointerDownHandlerInputReceiver";
            }
        }
        public void Execute()
        {
            if(_pointerDownHandler == null)
            {
                return;
            }

            _pointerDownHandler.OnPointerDown(IInputReceiver.DummyPointerEventData);

            if(_pointerDownHandler is IPointerUpHandler pointerUpHandler)
            {
                pointerUpHandler.OnPointerUp(IInputReceiver.DummyPointerEventData);
            }
        }
    }

    public class InputFieldInputReceiver : IInputReceiver
    {
        private readonly InputField _inputField;

        public InputFieldInputReceiver(InputField inputField)
        {
            _inputField = inputField;
        }

        public string Name => _inputField.name;

        public async void Execute()
        {
            if(_inputField == null)
            {
                return;
            }
            _inputField.ActivateInputField();

            await Task.Delay(100);
            if(_inputField == null)
            {
                return;
            }
            _inputField.text = StringUtil.GetRandomName();

            await Task.Delay(100);
            if(_inputField == null)
            {
                return;
            }
            _inputField.DeactivateInputField();
        }
    }

    #if POLYQA_USE_TEXTMESHPRO

    public class TextMeshProInputFieldInputReceiver : IInputReceiver
    {
        private readonly TMPro.TMP_InputField _inputField;

        public TextMeshProInputFieldInputReceiver(TMPro.TMP_InputField inputField)
        {
            _inputField = inputField;
        }

        public string Name => _inputField.name;
        public async void Execute()
        {
            if(_inputField == null)
            {
                return;
            }
            _inputField.ActivateInputField();

            await Task.Delay(100);
            if(_inputField == null)
            {
                return;
            }
            _inputField.text = StringUtil.GetRandomName();

            await Task.Delay(100);
            if(_inputField == null)
            {
                return;
            }
            _inputField.DeactivateInputField();
        }
    }

    #endif

#endif
}
