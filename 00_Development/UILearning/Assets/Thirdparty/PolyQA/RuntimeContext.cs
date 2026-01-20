using System;
using PolyQA.Executor;
using PolyQA.Input;
using PolyQA.Network;
using PolyQA.Observer;
using PolyQA.Query;
using PolyQA.Record;
using UnityEngine;

namespace PolyQA
{
    public class RuntimeContext : IDisposable
    {
        public static RuntimeContext Instance { get; private set; }

        public MonoBehaviour UnityBridge { get; }
        public CaptureProcessService CaptureProcessService { get; }

        public GameObject GameObject => UnityBridge.gameObject;
        public GameObjectDataSender GameObjectDataSender { get; }
        public GameObjectDataService GameObjectDataService { get; } = new();
        public GameObjectQuerySender GameObjectQuerySender { get; }
        public GameObjectQueryService GameObjectQueryService { get; }

        public InputService InputService { get; }
        public InputState InputState { get; } = new();
        public InputDataSender InputDataSender { get; }
        public InputActionExecutor InputActionExecutor { get; }

        public InputRecordService InputRecordService { get; }

        public InputRecordDataSender InputRecordDataSender { get; }

        public DataSenderService DataSenderService { get; }
        public UpdateExecutor UpdateExecutor { get; }

        public RuntimeContext(MonoBehaviour unityBridge)
        {
            Instance = this;
            UnityBridge = unityBridge;

            var dataSender = DataSenderInternal.Instance;
            DataSenderService = new DataSenderService(dataSender);
            CaptureProcessService = new CaptureProcessService(this, UnityBridge);

            GameObjectDataSender = new GameObjectDataSender(dataSender, GameObjectDataService);
            GameObjectQueryService = new GameObjectQueryService(GameObjectDataService);
            GameObjectQuerySender = new GameObjectQuerySender(dataSender, GameObjectQueryService);

            var inputActionExecutorComponent = GameObject.AddComponent<InputActionExecutorComponent>();
            InputActionExecutor = new InputActionExecutor(inputActionExecutorComponent);

            InputService = new InputService(InputState);
            InputDataSender = new InputDataSender(dataSender, this, InputActionExecutor);

            InputRecordService = new InputRecordService(this);
            InputRecordDataSender = new InputRecordDataSender(dataSender, this);

            UpdateExecutor = new UpdateExecutor(this);

            MainThreadDispatcher.SetMainThreadContext();
        }

        public void Dispose()
        {
            DataSenderService.Dispose();
            CaptureProcessService.Dispose();
            InputService.Dispose();
        }
    }
}