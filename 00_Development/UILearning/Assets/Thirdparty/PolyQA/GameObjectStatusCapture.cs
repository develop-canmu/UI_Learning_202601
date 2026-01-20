using System.Collections.Generic;
using PolyQA.Extensions;
using PolyQA.Network;
using PolyQA.Observer;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#if !POLYQA_DISABLE

namespace PolyQA
{
    public class GameObjectStatusCapture : MonoBehaviour
    {
        public RuntimeContext Context { get; set; }

        private IInputReceiver _inputReceiver;
        private string _path;
        private GameObjectObserver _observer;

        private void Start()
        {
            Context ??= RuntimeContext.Instance;

#if POLYQA_USE_TEXTMESHPRO
            var textMeshPro = gameObject.GetComponent<TMPro.TMP_InputField>();
#endif
            var inputField = gameObject.GetComponent<InputField>();
            var button = gameObject.GetComponent<Button>();
            var pointerClickHandler = gameObject.GetComponent<IPointerClickHandler>();
            var pointerDownHandler = gameObject.GetComponent<IPointerDownHandler>();

#if POLYQA_USE_TEXTMESHPRO
            if (textMeshPro != null)
            {
                _inputReceiver = new TextMeshProInputFieldInputReceiver(textMeshPro);
            }
            else
#endif
            if (inputField != null)
            {
                _inputReceiver = new InputFieldInputReceiver(inputField);
            }
            else if (button != null)
            {
                _inputReceiver = new ButtonInputReceiver(Context, button);
            }
            else if (pointerClickHandler != null)
            {
                _inputReceiver = new PointerClickHandlerInputReceiver(pointerClickHandler);
            }
            else if (pointerDownHandler != null)
            {
                _inputReceiver = new PointerDownHandlerInputReceiver(pointerDownHandler);
            }

            if (_inputReceiver != null)
            {
                var updater = gameObject.AddComponent<UIInputStatusUpdater>();
                updater.Receiver = _inputReceiver;
            }

            _observer = GameObjectObserver.CreateIfNeed(Context, gameObject);
        }

        private string GetPath()
        {
            var path = gameObject.name;
            var parent = transform.parent;
            while (parent != null)
            {
                path = parent.name + "_" + path;
                parent = parent.parent;
            }

            return path;
        }

        private void OnEnable()
        {
            _path = GetPath();
            DataSender.Send($"GOAC_{_path}", true);
            _observer?.OnEnable();
        }

        private void OnDisable()
        {
            DataSender.Send($"GOAC_{_path}", false);
            _observer?.OnDisable();
        }

        private void OnDestroy()
        {
            _observer?.OnDestroy();
        }
    }

    internal class UIInputStatusUpdater : MonoBehaviour
    {
        private bool _currentStatus;
        private string _path;
        private readonly List<RaycastResult> _raycastResults = new();
        private RectTransform _rect;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }

        private void Update()
        {
            var ev = EventSystem.current;
            if (Receiver == null || ev == null)
            {
                return;
            }

            if (!_rect)
            {
                if (!_currentStatus)
                {
                    _path = GetPath();
                    _currentStatus = true;
                    DataSenderInternal.RegisterInputCommand(_path, Receiver);
                }
            }
            else if (IsInteractable(ev, _rect))
            {
                if (!_currentStatus)
                {
                    _path = GetPath();
                    _currentStatus = true;
                    DataSenderInternal.RegisterInputCommand(_path, Receiver);
                }
            }
            else
            {
                if (_currentStatus)
                {
                    _currentStatus = false;
                    DataSenderInternal.UnRegisterInputCommand(_path);
                }
            }
        }

        private void OnDisable()
        {
            if (_currentStatus)
            {
                _currentStatus = false;
                DataSenderInternal.UnRegisterInputCommand(_path);
            }
        }

        public IInputReceiver Receiver { get; set; }

        private string GetPath()
        {
            var path = gameObject.name;
            var parent = transform.parent;
            while (parent != null)
            {
                path = parent.name + "_" + path;
                parent = parent.parent;
            }

            return path;
        }

        private bool IsInteractable(EventSystem eventSystem, RectTransform rect)
        {
            var center = rect.GetScreenPosition();

            var pointData = new PointerEventData(eventSystem)
            {
                position = center
            };
            eventSystem.RaycastAll(pointData, _raycastResults);

            if (_raycastResults.Count == 0)
            {
                return false;
            }

            var top = _raycastResults[0].gameObject;
            // topがrectの子供でない場合は無効
            return top.transform.IsChildOf(rect);
        }
    }
}

#endif