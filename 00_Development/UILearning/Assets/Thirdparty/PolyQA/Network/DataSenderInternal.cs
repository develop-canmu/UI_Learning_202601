using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PolyQA.Agent;
using PolyQA.Extensions;
using Polyscape.RPCLite;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.Scripting;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Object = UnityEngine.Object;

#if POLYQA_DISABLE
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#endif

#if !POLYQA_DISABLE
[Preserve]
internal sealed class MarkerForInternet : UnityWebRequest { }
#endif

namespace PolyQA.Network
{
    public class DataSenderInternal : IDisposable
    {
        private static DataSenderInternal _instance;

        internal static DataSenderInternal Instance => _instance ??= new DataSenderInternal();
        private static Material BlitMaterial;

#if !POLYQA_DISABLE
        private enum ConnectionStatus
        {
            Init,
            Connecting,
            Connected,
            Disconnecting,
            Disconnected,
            EndSession,
        }

        private enum ScreenshotMode
        {
            Stopped,
            EveryFrame,
            OneShot,
        }

        private const int QueueSize = 1000;
        private static readonly ILogger Logger = Logging.CreateLogger<DataSenderInternal>();

        internal static bool IsSessionAlive =>
            Instance != null && Instance._connectionStatus != ConnectionStatus.EndSession;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RuntimeInitialize()
        {
            if (BlitMaterial == null)
            {
                var shader = Shader.Find("Hidden/PolyQA/ReverseResolve");
                BlitMaterial = new Material(shader);
            }

            if (_instance != null)
            {
                _instance.Disconnect().HandleException(Logger);
            }
        }

        public void Start()
        {
            Connect().Forget(Logger);
        }

        private async Task Connect()
        {
            try
            {
                _connectionStatus = ConnectionStatus.Connecting;
                var agent = await _agentResolver.Resolve();
                _agentAddress = agent.IPAddress;
                _clientService =
                    await ClientService.CreateAsync(
                        _agentAddress,
                        Const.NetworkPort,
                        TimeSpan.FromSeconds(5),
                        Logging.CreateLogger<ClientService>());

                await OnConnected();
                _connectionStatus = ConnectionStatus.Connected;
            }
            catch (Exception e)
            {
                _connectionStatus = ConnectionStatus.Disconnected;
                throw new ConnectException($"Failed to connect to {_agentAddress}", e);
            }
        }

        private async ValueTask OnConnected()
        {
            _clientService.OnDisconnected += OnDisconnected;
            _clientService.Register<string>(RPC.InvokeCommand, (command, _) =>
            {
                IInputReceiver receiver;
                lock (_inputReceivers)
                {
                    _inputReceivers.TryGetValue(command, out receiver);
                }

                if (receiver != null)
                {
                    _commandDispatcher.Post(() => { receiver.Execute(); });
                }
                else
                {
                    Logger.LogError("Command {command} is not registered", command);
                }
            });
            _clientService.Register<Void>(RPC.EndSession, (_, _) => EndSession().Forget(Logger));
            _clientService.Register<Void>(RPC.StartScreenRecording, (_, _) => StartScreenRecording());
            _clientService.Register<Void>(RPC.ScreenshotRequest, (_, _) => ScreenshotRequest());
            _clientService.Register<Void, Void>(RPC.Heartbeat, (_, _) => Void.Default);

            foreach (var p in _plugins)
            {
                p.OnConnected(_clientService);
            }

            string os = string.Empty;
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    os = OS.Windows;
                    break;
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXEditor:
                    os = OS.MacOS;
                    break;
                case RuntimePlatform.LinuxPlayer:
                case RuntimePlatform.LinuxEditor:
                    os = OS.Linux;
                    break;
                case RuntimePlatform.Android:
                    os = OS.Android;
                    break;
                case RuntimePlatform.IPhonePlayer:
                    os = OS.IOS;
                    break;
                default:
                    os = OS.Unknown;
                    break;
            }

            var request = new StartSessionRequest(
                _sessionId,
                Const.Version,
                Const.ProtocolVersion,
                os,
                Application.identifier);
            await _clientService.CallRpcAsync(RPC.StartSession, request, default);
            await _clientService.FlushAsync(default); // 他のメッセージより先に送信するため
        }

        private async Task EndSession()
        {
            if (_clientService == null) return;

            Logger.LogInformation("Request end session");
            _connectionStatus = ConnectionStatus.EndSession;
            _screenshotMode = ScreenshotMode.Stopped;

            try
            {
                await _clientService.FlushAsync(default);
            }
            catch (Exception e)
            {
                Logger.LogDebug(e, "Failed to flush");
            }

            try
            {
                _clientService.Dispose();
            }
            finally
            {
                _clientService = null;
            }
        }

        private void StartScreenRecording()
        {
            _screenshotMode = ScreenshotMode.EveryFrame;
        }

        private void ScreenshotRequest()
        {
            _screenshotMode = ScreenshotMode.OneShot;
        }
#endif

#if POLYQA_DISABLE
        [System.Diagnostics.Conditional("YOUR_PROJECT_NAME_NEVER_DEFINED_SYMBOL")]
#endif
        internal static void Update()
        {
#if !POLYQA_DISABLE
            if (IsSessionAlive == false) return;
            if (Instance._updateTask != null && !Instance._updateTask.IsCompleted) return;

            Instance._updateTask = Instance.UpdateInternal();
            Instance._updateTask.HandleException(Logger);
#endif
        }

#if !POLYQA_DISABLE
        private async Task UpdateInternal()
        {
            if (_connectionStatus == ConnectionStatus.Connected)
            {
                await SendItemsInQueue();
                await _clientService.FlushAsync(default);
            }
            else if (_connectionStatus == ConnectionStatus.Disconnected)
            {
                await Connect();
            }
        }

        private async ValueTask SendItemsInQueue()
        {
            for(var i = 0; i < 10; i++)
            {
                if (!_sendQueue.TryDequeue(out var item)) break;
                await item.Send();
            }
        }

        public void Stop()
        {
            StopInternal().Forget(Logger);
        }

        private async Task StopInternal()
        {
            await Instance.Disconnect();
            Instance.Dispose();
        }

        private async Task Disconnect()
        {
            if (_clientService == null) return;

            try
            {
                _connectionStatus = ConnectionStatus.Disconnecting;
                await _clientService.FlushAsync(default);
            }
            catch (Exception e)
            {
                Logger.LogDebug(e, "Failed to flush");
            }

            try
            {
                _clientService.Dispose();
            }
            finally
            {
                _clientService = null;
                _connectionStatus = ConnectionStatus.Disconnected;
            }
        }
#endif

#if POLYQA_DISABLE
        [System.Diagnostics.Conditional("YOUR_PROJECT_NAME_NEVER_DEFINED_SYMBOL")]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Send(string key) => Send(key, true);

#if POLYQA_DISABLE
        [System.Diagnostics.Conditional("YOUR_PROJECT_NAME_NEVER_DEFINED_SYMBOL")]
#endif
        public static void Send<T>(string key, T value)
        {
#if! POLYQA_DISABLE
            if (value is string or int or float or bool or Vector2 or Vector3 or Vector4 or Quaternion or Color
                or byte[])
            {
                InternalSend(key, value);
            }
            else if (value is byte byteValue)
            {
                InternalSend(key, (int)byteValue);
            }
            else if (value is sbyte sbyteValue)
            {
                InternalSend(key, (int)sbyteValue);
            }
            else if (value is short shortValue)
            {
                InternalSend(key, (int)shortValue);
            }
            else if (value is ushort ushortValue)
            {
                InternalSend(key, (int)ushortValue);
            }
            else if (value is uint uintValue)
            {
                InternalSend(key, (int)uintValue);
            }
            else if (value is long longValue)
            {
                InternalSend(key, (int)longValue);
            }
            else if (value is ulong ulongValue)
            {
                InternalSend(key, (int)ulongValue);
            }
            else if (value is double doubleValue)
            {
                InternalSend(key, (float)doubleValue);
            }
            else
            {
                throw new NotSupportedException($"Type {value.GetType()} is not supported");
            }
#endif
        }

#if POLYQA_DISABLE
        [System.Diagnostics.Conditional("YOUR_PROJECT_NAME_NEVER_DEFINED_SYMBOL")]
#endif
        internal static void TakeScreenshot()
        {
#if! POLYQA_DISABLE
            if (Instance._screenshotMode == ScreenshotMode.Stopped)
            {
                Instance._screenshotMode = ScreenshotMode.OneShot;
            }
#endif
        }

#if! POLYQA_DISABLE
        internal static void SendTexture(string key)
        {
            if (Instance._screenshotMode == ScreenshotMode.Stopped) return;
            if (!Instance.CanSend) return;

            var frame = Time.frameCount;

            if (Instance._commandBuffer == null)
            {
                Instance._commandBuffer = new CommandBuffer();
                Instance._commandBuffer.name = "ScreenCapture";
            }

            var cb = Instance._commandBuffer;
            cb.Clear();
            var backBuffer = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
            var target = RenderTexture.GetTemporary(Screen.width / 2, Screen.height / 2, 0, RenderTextureFormat.ARGB32);
            ScreenCapture.CaptureScreenshotIntoRenderTexture(backBuffer);
            cb.BeginSample("ScreenToRt");
            cb.Blit(backBuffer, target, BlitMaterial);
            cb.EndSample("ScreenToRt");
            Graphics.ExecuteCommandBuffer(cb);
            RenderTexture.ReleaseTemporary(backBuffer);
            cb.Clear();

            if (Instance._screenshotMode == ScreenshotMode.OneShot)
            {
                Instance._screenshotMode = ScreenshotMode.Stopped;
            }

            AsyncGPUReadback.Request(target, 0, request =>
            {
                if (request.done)
                {
                    var bytes = request.GetData<byte>();
                    var work = new NativeArray<byte>(bytes, Allocator.Persistent);
                    var width = (uint)target.width;
                    var height = (uint)target.height;

                    Task.Run(() =>
                    {
                        using (var jpgData = ImageConversion.EncodeNativeArrayToJPG(work,
                                   GraphicsFormat.R8G8B8A8_UNorm, width, height))
                        {
                            if (Instance.CanSend)
                            {
                                Instance._clientService.CallRpcAsync("DataSendByteArray",
                                    new DataSendArgs<byte[]>(key, frame, jpgData.ToArray()), default);
                            }
                        }

                        work.Dispose();
                    }).HandleException(Logger);
                }

                RenderTexture.ReleaseTemporary(target);
            });
        }
#endif

#if POLYQA_DISABLE
        [System.Diagnostics.Conditional("YOUR_PROJECT_NAME_NEVER_DEFINED_SYMBOL")]
#endif
        internal static async void RegisterInputCommand(string command, IInputReceiver receiver)
        {
#if! POLYQA_DISABLE
            if (Instance == null) return;
            await Instance.CallRpc(RPC.RegisterInputCommand, command, default);
            lock (Instance._inputReceivers)
            {
                Instance._inputReceivers[command] = receiver;
            }
#endif
        }

#if POLYQA_DISABLE
        [System.Diagnostics.Conditional("YOUR_PROJECT_NAME_NEVER_DEFINED_SYMBOL")]
#endif
        internal static async void UnRegisterInputCommand(string command)
        {
#if! POLYQA_DISABLE
            if (Instance == null) return;
            await Instance.CallRpc(RPC.UnRegisterInputCommand, command, default);
            lock (Instance._inputReceivers)
            {
                Instance._inputReceivers.Remove(command);
            }
#endif
        }

#if! POLYQA_DISABLE
        private AgentResolver _agentResolver = new();
        private CommandBuffer _commandBuffer;
        private ClientService _clientService;
        private readonly CommandDispatcher _commandDispatcher;
        private readonly Dictionary<string, IInputReceiver> _inputReceivers = new();
        private readonly ConcurrentQueue<ISendQueueItem> _sendQueue = new();
        private string _agentAddress;
        private ConnectionStatus _connectionStatus;
        private Task _updateTask;
        private readonly string _sessionId = Guid.NewGuid().ToString();
        private readonly List<DataSenderPlugin> _plugins = new();
        private ScreenshotMode _screenshotMode;

        private bool CanSend => _connectionStatus == ConnectionStatus.Connected;

        private DataSenderInternal()
        {
            var dummy = new GameObject("CommandDispatcher");
            Object.DontDestroyOnLoad(dummy);

            _commandDispatcher = dummy.AddComponent<CommandDispatcher>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InternalSend<T>(string key, T value)
        {
            switch (value)
            {
                case string:
                    InternalInternalSend(RPC.DataSendString, key, value);
                    break;
                case int:
                    InternalInternalSend(RPC.DataSendInt, key, value);
                    break;
                case float:
                    InternalInternalSend(RPC.DataSendFloat, key, value);
                    break;
                case bool:
                    InternalInternalSend(RPC.DataSendBool, key, value);
                    break;
                case Vector2:
                    InternalInternalSend(RPC.DataSendVector2, key, value);
                    break;
                case Vector3:
                    InternalInternalSend(RPC.DataSendVector3, key, value);
                    break;
                case Vector4:
                    InternalInternalSend(RPC.DataSendVector4, key, value);
                    break;
                case Quaternion:
                    InternalInternalSend(RPC.DataSendQuaternion, key, value);
                    break;
                case Color:
                    InternalInternalSend(RPC.DataSendColor, key, value);
                    break;
                case byte[]:
                    InternalInternalSend(RPC.DataSendByteArray, key, value);
                    break;
                default:
                    throw new NotSupportedException($"Type {value.GetType()} is not supported");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async void InternalInternalSend<T>(string rpcName, string key, T value)
        {
            if (Instance == null) return;
            await Instance.CallRpc(rpcName, new DataSendArgs<T>(key, Time.frameCount, value), default);
        }
#endif

#if POLYQA_DISABLE
        [System.Diagnostics.Conditional("YOUR_PROJECT_NAME_NEVER_DEFINED_SYMBOL")]
#endif
        public void CallRpc<T>(string rpcName, T args)
        {
#if !POLYQA_DISABLE
            CallRpc(rpcName, args, default).HandleException(Logger);
#endif
        }

#if !POLYQA_DISABLE
        public async Task CallRpc<T>(string rpcName, T args, CancellationToken token = default)
        {
            if (IsSessionAlive == false) return;

            if (CanSend)
            {
                try
                {
                    await _clientService.CallRpcAsync(rpcName, args, token);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Failed to call RPC {rpcName}", rpcName);
                }
                return;
            }

            while (_sendQueue.Count >= QueueSize)
            {
                Logger.LogWarning("Send queue is full. Drop the oldest item. size: {size}", _sendQueue.Count);
                _sendQueue.TryDequeue(out _);
            }
            _sendQueue.Enqueue(new SendQueueItem<T>(rpcName, args));
        }

        private void OnDisconnected()
        {
            _clientService = null;

            if (_connectionStatus != ConnectionStatus.EndSession)
            {
                _connectionStatus = ConnectionStatus.Disconnected;
            }

            _screenshotMode = ScreenshotMode.Stopped;
        }

#endif

#if POLYQA_DISABLE
        [System.Diagnostics.Conditional("YOUR_PROJECT_NAME_NEVER_DEFINED_SYMBOL")]
#endif
        public void RegisterPlugin(DataSenderPlugin plugin)
        {
#if !POLYQA_DISABLE
            _plugins.AddUnique(plugin);
#endif
        }

#if !POLYQA_DISABLE
        private interface ISendQueueItem
        {
            ValueTask Send();
        }

        private class SendQueueItem<T> : ISendQueueItem
        {
            private string _rpcName;
            private T _args;

            public SendQueueItem(string rpcName, T args)
            {
                _rpcName = rpcName;
                _args = args;
            }

            public ValueTask Send()
            {
                return Instance._clientService.CallRpcAsync(_rpcName, _args, default);
            }
        }
#endif

        public void Dispose()
        {
#if! POLYQA_DISABLE
            _clientService?.Dispose();
            if (_commandDispatcher)
            {
                Object.Destroy(_commandDispatcher.gameObject);
            }
#endif
        }
    }

#if! POLYQA_DISABLE
    internal class CommandDispatcher : MonoBehaviour
    {
        private readonly ConcurrentQueue<Action> _commandList = new();

        public void Post(Action command)
        {
            _commandList.Enqueue(command);
        }

        private void Update()
        {
            while (_commandList.TryDequeue(out var command))
            {
                command();
            }
        }
    }
#endif
}