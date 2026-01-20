using System.Collections.Generic;
using System.Linq;
using PolyQA.Extensions;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace PolyQA.Observer
{
    public class GameObjectDataService
    {
        private readonly GameObjectDataStore _gameObjects = new();

        public string MakeUniquePath(string path)
        {
            if (!_gameObjects.Exists(path)) return path;

            var i = 1;
            var newPath = $"{path}#{i}";
            while (_gameObjects.Exists(newPath))
            {
                i++;
                newPath = $"{path}#{i}";
            }
            return newPath;
        }

        public void AddGameObject(
            string path, string label, GameObject gameObject, bool isInteractable, string[] availableActions)
        {
            if (_gameObjects.TryGet(path, out var data))
            {
                _gameObjects.Remove(path);
            }

            data = new GameObjectData(path, label)
            {
                Instance = gameObject,
                IsInteractable = isInteractable,
                AvailableActions = availableActions ?? System.Array.Empty<string>(),
                ClassNames = gameObject.GetComponents<Component>().Select(x => x.GetType().FullName).ToArray()
            };
            _gameObjects.Add(data);
        }

        public void UpdateGameObjectInteractable(string path, bool isInteractable)
        {
            if (_gameObjects.TryGet(path, out var data))
            {
                data.IsInteractable = isInteractable;
            }
        }

        public void RemoveGameObject(string path)
        {
            _gameObjects.Remove(path);
        }

        public GameObjectData FindGameObject(string path)
        {
            return _gameObjects.Find(path);
        }

        public IEnumerable<GameObjectData> FindGameObjects(string path)
        {
            return _gameObjects.FindAll(path);
        }

        public GetInteractableGameObjectsResponse GetInteractableGameObjects()
        {
            return new GetInteractableGameObjectsResponse(
                Time.frameCount,
                new Vector2(Screen.width, Screen.height),
                _gameObjects.GetAll()
                    .Where(x => x.IsInteractable)
                    .Select(x => new InteractableGameObject(
                        x.Path,
                        x.ClassNames,
                        x.Instance.GetScreenRect(),
                        x.Label
                    ))
                    .ToArray()
            );
        }
    }

    public class GameObjectDataStore
    {
        private readonly Dictionary<string, GameObjectData> _gameObjects = new();
        private readonly Dictionary<string, HashSet<string>> _labelIndex = new();

        public void Add(GameObjectData data)
        {
            if (data == null || string.IsNullOrEmpty(data.Path)) return;

            _gameObjects.Add(data.Path, data);

            if (string.IsNullOrEmpty(data.Label)) return;
            if (!_labelIndex.TryGetValue(data.Label, out var paths))
            {
                paths = new HashSet<string>();
                _labelIndex.Add(data.Label, paths);
            }
            paths.Add(data.Path);
        }

        public bool Exists(string path) => _gameObjects.ContainsKey(path);

        public bool TryGet(string path, out GameObjectData data) => _gameObjects.TryGetValue(path, out data);

        public GameObjectData Find(string key)
        {
            GameObjectData data;

            if (_labelIndex.TryGetValue(key, out var paths) && paths.Count > 0)
            {
                if (_gameObjects.TryGetValue(paths.First(), out data))
                {
                    return data;
                }
            }

            if (_gameObjects.TryGetValue(key, out data))
            {
                return data;
            }

            return null;
        }

        public IEnumerable<GameObjectData> FindAll(string key)
        {
            if (_labelIndex.TryGetValue(key, out var paths))
            {
                return paths.Select(path => _gameObjects[path]);
            }

            return Enumerable.Empty<GameObjectData>();
        }

        public void Remove(string path)
        {
            if (!_gameObjects.Remove(path, out var data)) return;
            if (string.IsNullOrEmpty(data.Label)) return;
            if (_labelIndex.TryGetValue(data.Label, out var paths))
            {
                paths.Remove(path);
                if (paths.Count == 0)
                {
                    _labelIndex.Remove(data.Label);
                }
            }
        }

        public IEnumerable<GameObjectData> GetAll() => _gameObjects.Values;
    }

    public class GameObjectData
    {
        public string Path { get; }
        public string Label { get; }
        public GameObject Instance { get; set; }
        public bool IsInteractable { get; set; }
        public string[] AvailableActions { get; set; } = System.Array.Empty<string>();
        public string[] ClassNames { get; set; } = System.Array.Empty<string>();

        public GameObjectData(string path, string label)
        {
            Path = path;
            Label = label;
        }
    }
}