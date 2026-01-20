using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PolyQA.Executor;
using PolyQA.Network;
using Polyscape.RPCLite;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PolyQA.Observer
{
    public class GameObjectDataSender : DataSenderPlugin
    {
        private readonly ILogger _logger = Logging.CreateLogger<GameObjectDataSender>();
        private readonly GameObjectDataService _gameObjectDataService;

        public GameObjectDataSender(DataSenderInternal dataSender, GameObjectDataService gameObjectDataService) : base(dataSender)
        {
            _gameObjectDataService = gameObjectDataService;
        }

        public override void OnConnected(ClientService clientService)
        {
            clientService.Register<GetInteractableGameObjectsResponse, Void>(
                RPC.GetInteractableGameObjects, GetInteractableGameObjects);
        }

        private async ValueTask<GetInteractableGameObjectsResponse> GetInteractableGameObjects(Void _, int __)
        {
            return await MainThreadDispatcher.Run(() =>
            {
                DataSenderInternal.TakeScreenshot();
                return _gameObjectDataService.GetInteractableGameObjects();
            });
        }

        public void SendGameObject(string path, string label, bool isInteractable, string[] availableActions)
        {
            _logger.LogDebug("SendGameObject: {Path} {Label} {IsInteractable} {AvailableActions}",
                path, label, isInteractable, availableActions);
            var data = new GameObjectUpdate(path, label, isInteractable, availableActions, Time.frameCount);
            DataSender.CallRpc(RPC.GameObjectUpdate, data);
        }
    }
}