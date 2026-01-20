using System;
using System.Collections;
using System.Collections.Generic;
using PolyQA.Network;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace PolyQA.Observer
{
    public class CaptureProcessService : IDisposable
    {
        private readonly RuntimeContext _context;
        private readonly MonoBehaviour _unityBridge;

        public CaptureProcessService(RuntimeContext context, MonoBehaviour unityBridge)
        {
            _context = context;
            _unityBridge = unityBridge;

#if !POLYQA_DISABLE
            SceneManager.sceneLoaded += OnSceneLoaded;
            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            Application.logMessageReceived += OnLogMessageReceived;

            unityBridge.StartCoroutine(CaptureCoroutine());
            unityBridge.StartCoroutine(AddGameObjectStatusCapture());
#endif
        }

#if !POLYQA_DISABLE
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            switch (mode)
            {
                case LoadSceneMode.Single:
                    DataSender.Send("scene.loaded", scene.name);
                    break;
                case LoadSceneMode.Additive:
                    DataSender.Send("scene.additive_loaded", scene.name);
                    break;
                default:
                    DataSender.Send($"scene.loaded_{mode}", scene.name);
                    break;
            }
        }

        private void OnSceneUnloaded(Scene scene)
        {
            DataSender.Send("scene.unloaded", scene.name);
        }

        private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
                DataSenderInternal.TakeScreenshot();
                // conditionとstackTraceを分離しやすくするために改行を入れる
                DataSender.Send("unity.error", $"{condition} \n\n\n\n\n {stackTrace}");
            }
        }

        private IEnumerator AddGameObjectStatusCapture()
        {
            HashSet<int> gameObjectIds = new();
            while (true)
            {
                var objects = Object.FindObjectsByType(
                    typeof(GameObject), FindObjectsInactive.Exclude, FindObjectsSortMode.None);
                yield return null;

                var addedObjectCount = 0;
                foreach (var o in objects)
                {
                    if (o == null)
                    {
                        continue;
                    }

                    var instanceId = o.GetInstanceID();
                    if (!gameObjectIds.Add(instanceId))
                    {
                        continue;
                    }

                    var go = (GameObject)o;
                    if (!go.GetComponent<GameObjectStatusCapture>())
                    {
                        var comp = go.AddComponent<GameObjectStatusCapture>();
                        comp.Context = _context;
                    }
                    addedObjectCount++;

                    if (addedObjectCount % 100 == 99)
                    {
                        yield return null;
                    }
                }

                yield return null;
            }
        }

        private IEnumerator CaptureCoroutine()
        {
            var eof = new WaitForEndOfFrame();
            while (true)
            {
                yield return eof;
                DataSenderInternal.SendTexture("system.screenshot.unity");
            }
        }
#endif

        public void Dispose()
        {
#if !POLYQA_DISABLE
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;

            Application.logMessageReceived -= OnLogMessageReceived;

            _unityBridge.StopAllCoroutines();
#endif
        }
    }
}