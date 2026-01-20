using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using PolyQA.Extensions;
using PolyQA.Observer;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PolyQA.Query
{
    public partial class GameObjectQueryService
    {
        private readonly ILogger _logger = Logging.CreateLogger<GameObjectQueryService>();
        private readonly ObjectRegistry _objectRegistry = new ();
        private readonly GameObjectDataService _gameObjectDataService;

        public GameObjectQueryService(GameObjectDataService gameObjectDataService)
        {
            _gameObjectDataService = gameObjectDataService;
        }

        public int FindGameObject(string name)
        {
            var data = _gameObjectDataService.FindGameObject(name);
            if (data != null)
            {
                return _objectRegistry.Register(data.Instance);
            }

            var go = GameObject.Find(name);
            if (!go) return 0;

            return _objectRegistry.Register(go);
        }

        public int FindGameObject(int gameObjectInstanceId, string name, bool isRegex, bool isChildrenOnly)
        {
            var go = _objectRegistry.Get<GameObject>(gameObjectInstanceId);
            if (!go)
            {
                _logger.LogWarning("GameObject not found. Instance ID: {id}", gameObjectInstanceId);
                return 0;
            }

            var matcher = new GameObjectMatcher(name, isRegex);
            var found = isChildrenOnly
                ? FindChildTransform(go.transform, matcher)
                : FindTransformRecursive(go.transform, matcher);
            return found ? _objectRegistry.Register(found.gameObject) : 0;
        }

        private static Transform FindTransformRecursive(Transform transform, GameObjectMatcher matcher)
        {
            foreach (Transform child in transform)
            {
                if (matcher.IsMatch(child.gameObject))
                {
                    return child;
                }

                var result = FindTransformRecursive(child, matcher);
                if (result)
                {
                    return result;
                }
            }

            return null;
        }

        private static Transform FindChildTransform(Transform transform, GameObjectMatcher matcher)
        {
            foreach (Transform child in transform)
            {
                if (matcher.IsMatch(child.gameObject))
                {
                    return child;
                }
            }

            return null;
        }

        public int[] FindGameObjects(string name)
        {
            return _gameObjectDataService.FindGameObjects(name)
                .Select(data => _objectRegistry.Register(data.Instance))
                .ToArray();
        }

        public int[] FindGameObjects(int gameObjectInstanceId, string name, bool isRegex, bool isChildrenOnly)
        {
            var go = _objectRegistry.Get<GameObject>(gameObjectInstanceId);
            if (!go)
            {
                _logger.LogWarning("GameObject not found. Instance ID: {id}", gameObjectInstanceId);
                return Array.Empty<int>();
            }

            var matcher = new GameObjectMatcher(name, isRegex);
            var transforms = isChildrenOnly
                ? FindChildTransforms(go.transform, matcher)
                : FindTransformsRecursive(go.transform, matcher);
            return transforms
                .Select(t => _objectRegistry.Register(t.gameObject))
                .ToArray();
        }

        private static IEnumerable<Transform> FindTransformsRecursive(Transform transform, GameObjectMatcher matcher)
        {
            foreach (Transform child in transform)
            {
                if (matcher.IsMatch(child.gameObject))
                {
                    yield return child;
                }

                foreach (var result in FindTransformsRecursive(child, matcher))
                {
                    yield return result;
                }
            }
        }

        private static IEnumerable<Transform> FindChildTransforms(Transform transform, GameObjectMatcher matcher)
        {
            foreach (Transform child in transform)
            {
                if (matcher.IsMatch(child.gameObject))
                {
                    yield return child;
                }
            }
        }

        public string GetPath(int gameObjectInstanceId)
        {
            var go = _objectRegistry.Get<GameObject>(gameObjectInstanceId);
            if (!go)
            {
                _logger.LogWarning("GameObject not found. Instance ID: {id}", gameObjectInstanceId);
                return null;
            }

            var path = go.name;
            var parent = go.transform.parent;

            while (parent)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }

            return path;
        }

        public bool GetActive(int gameObjectInstanceId)
        {
            var go = _objectRegistry.Get<GameObject>(gameObjectInstanceId);
            if (!go)
            {
                _logger.LogWarning("GameObject not found. Instance ID: {id}", gameObjectInstanceId);
                return false;
            }

            return go.activeInHierarchy;
        }

        public string GetText(int gameObjectInstanceId)
        {
            var go = _objectRegistry.Get<GameObject>(gameObjectInstanceId);
            if (!go)
            {
                _logger.LogWarning("GameObject not found. Instance ID: {id}", gameObjectInstanceId);
                return null;
            }

#if POLYQA_USE_UGUI
            var text = go.GetComponent<UnityEngine.UI.Text>();
            if (text)
            {
                return text.text;
            }
#endif
#if POLYQA_USE_TEXTMESHPRO
            var textMeshPro = go.GetComponent<TMPro.TextMeshProUGUI>();
            if (textMeshPro)
            {
                return textMeshPro.text;
            }
#endif

            _logger.LogWarning("Text component not found. Instance ID: {id}", gameObjectInstanceId);
            return string.Empty;
        }

        public string GetImageName(int gameObjectInstanceId)
        {
            var go = _objectRegistry.Get<GameObject>(gameObjectInstanceId);
            if (!go)
            {
                _logger.LogWarning("GameObject not found. Instance ID: {id}", gameObjectInstanceId);
                return null;
            }

#if POLYQA_USE_UGUI
            var image = go.GetComponent<UnityEngine.UI.Image>();
            if (image)
            {
                return image.sprite.name;
            }

            var rawImage = go.GetComponent<UnityEngine.UI.RawImage>();
            if (rawImage)
            {
                return rawImage.texture.name;
            }
#endif

            _logger.LogWarning("Image component not found. Instance ID: {id}", gameObjectInstanceId);
            return string.Empty;
        }

        public bool GetCheckboxValue(int gameObjectInstanceId)
        {
            var go = _objectRegistry.Get<GameObject>(gameObjectInstanceId);
            if (!go)
            {
                _logger.LogWarning("GameObject not found. Instance ID: {id}", gameObjectInstanceId);
                return false;
            }

#if POLYQA_USE_UGUI
            var toggle = go.GetComponent<UnityEngine.UI.Toggle>();
            if (toggle)
            {
                return toggle.isOn;
            }
#endif

            _logger.LogWarning("Checkbox component not found. Instance ID: {id}", gameObjectInstanceId);
            return false;
        }

        public Vector2 GetScreenPosition(int gameObjectInstanceId)
        {
            var go = _objectRegistry.Get<GameObject>(gameObjectInstanceId);
            if (!go)
            {
                _logger.LogWarning("GameObject not found. Instance ID: {id}", gameObjectInstanceId);
                return Vector2.zero;
            }

            var v = go.GetScreenPosition();
            return new Vector2(v.x / Screen.width, v.y / Screen.height);
        }

        public Rect GetScreenRect(int gameObjectInstanceId)
        {
            var go = _objectRegistry.Get<GameObject>(gameObjectInstanceId);
            if (!go)
            {
                _logger.LogWarning("GameObject not found. Instance ID: {id}", gameObjectInstanceId);
                return Rect.zero;
            }

            var rect = go.GetScreenRect();
            return new Rect(rect.x / Screen.width, rect.y / Screen.height, rect.width / Screen.width, rect.height / Screen.height);
        }

        public void ReleaseObjectReference(int id)
        {
            _objectRegistry.Unregister(id);
        }

        private class GameObjectMatcher
        {
            private readonly string _name;
            private readonly Regex _regex;

            public GameObjectMatcher(string name, bool isRegex)
            {
                _name = name;
                if (isRegex)
                {
                    _regex = new Regex(name, RegexOptions.Compiled);
                }
            }

            public bool IsMatch(GameObject go)
            {
                return _regex?.IsMatch(go.name) ?? go.name == _name;
            }
        }
    }
}