using System.Collections.Generic;
using System.Linq;
using PolyQA.Executor;
using PolyQA.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace PolyQA.Observer
{
    public class GameObjectObserver : IUpdater
    {
        private readonly RuntimeContext _context;
        private readonly GameObject _gameObject;
        private readonly RectTransform _rectTransform;
        private string _path;
        private string _label;
        private bool _isInteractable;
        private string[] _actions;

        public static GameObjectObserver CreateIfNeed(RuntimeContext context, GameObject gameObject)
        {
            var actions = FindAvailableActions(gameObject);
            var tracker = gameObject.GetComponent<IGameObjectTracker>();
            var label = tracker?.Label;
            if (actions.Length == 0 && string.IsNullOrWhiteSpace(label)) return null;

            var observer = new GameObjectObserver(context, gameObject);
            observer.Start(label, actions);

            return observer;
        }

        private GameObjectObserver(RuntimeContext context, GameObject gameObject)
        {
            _context = context;
            _gameObject = gameObject;
            _rectTransform = gameObject.GetComponent<RectTransform>();
        }

        private void Start(string label, string[] actions)
        {
            _actions = actions;
            _label = label?.Trim() ?? string.Empty;
            var path = MakePath(_gameObject);
            _path = _context.GameObjectDataService.MakeUniquePath(path);
            _isInteractable = IsInteractable(_rectTransform);

            _context.GameObjectDataService.AddGameObject(_path, _label, _gameObject, _isInteractable, actions);
            if (_isInteractable)
            {
                _context.GameObjectDataSender.SendGameObject(_path, _label, _isInteractable, actions);
            }
            _context.UpdateExecutor.Add(this);
        }

        public void Update()
        {
            if (_isInteractable == IsInteractable(_rectTransform)) return;

            _isInteractable = !_isInteractable;
            _context.GameObjectDataService.UpdateGameObjectInteractable(_path, _isInteractable);
            _context.GameObjectDataSender.SendGameObject(_path, _label, _isInteractable, _actions);
        }

        public void OnEnable()
        {
            _isInteractable = IsInteractable(_rectTransform);
            if (_isInteractable)
            {
                _context.GameObjectDataService.UpdateGameObjectInteractable(_path, true);
                _context.GameObjectDataSender.SendGameObject(_path, _label, true, _actions);
            }
            _context.UpdateExecutor.Add(this);
        }

        public void OnDisable()
        {
            if (_isInteractable)
            {
                _isInteractable = false;
                _context.GameObjectDataService.UpdateGameObjectInteractable(_path, false);
                _context.GameObjectDataSender.SendGameObject(_path, _label, false, _actions);
            }
            _context.UpdateExecutor.Remove(this);
        }

        public void OnDestroy()
        {
            _context.GameObjectDataService.RemoveGameObject(_path);
        }

        private static string MakePath(GameObject gameObject)
        {
            using (ListPool<string>.Get(out var hierarchy))
            {
                hierarchy.Add(gameObject.name);
                Transform parent = gameObject.transform.parent;
                while (parent)
                {
                    hierarchy.Add(parent.name);
                    parent = parent.parent;
                }
                return string.Join("/", hierarchy.Reverse<string>());
            }
        }

        private static string[] FindAvailableActions(GameObject gameObject)
        {
            List<string> actions = new List<string>();

            if (gameObject.GetComponent<IPointerClickHandler>() != null)
            {
                actions.Add(EventSystemAction.Click);
            }
            if (gameObject.GetComponent<IPointerDownHandler>() != null)
            {
                actions.Add(EventSystemAction.PointerDown);
            }
            if (gameObject.GetComponent<IPointerUpHandler>() != null)
            {
                actions.Add(EventSystemAction.PointerUp);
            }
            if (gameObject.GetComponent<IPointerMoveHandler>() != null)
            {
                actions.Add(EventSystemAction.PointerMove);
            }
            if (gameObject.GetComponent<IDragHandler>() != null)
            {
                actions.Add(EventSystemAction.Drag);
            }
            if (gameObject.GetComponent<IEndDragHandler>() != null)
            {
                actions.Add(EventSystemAction.EndDrag);
            }
            if (gameObject.GetComponent<IDropHandler>() != null)
            {
                actions.Add(EventSystemAction.Drop);
            }

            if (gameObject.GetComponent<InputField>())
            {
                actions.Add(UIAction.InputText);
            }
#if POLYQA_USE_TEXTMESHPRO
            else if (gameObject.GetComponent<TMPro.TMP_InputField>())
            {
                actions.Add(UIAction.InputText);
            }
#endif

            return actions.ToArray();
        }

        private static bool IsInteractable(RectTransform rect)
        {
            if (!rect) return false;
            var eventSystem = EventSystem.current;
            if (!eventSystem) return false;

            var center = rect.GetScreenPosition();
            var pointData = new PointerEventData(eventSystem)
            {
                position = center
            };

            using (ListPool<RaycastResult>.Get(out var raycastResults))
            {
                eventSystem.RaycastAll(pointData, raycastResults);
                if (raycastResults.Count == 0) return false;

                var top = raycastResults[0].gameObject;
                return top.transform.IsChildOf(rect);
            }
        }
    }
}