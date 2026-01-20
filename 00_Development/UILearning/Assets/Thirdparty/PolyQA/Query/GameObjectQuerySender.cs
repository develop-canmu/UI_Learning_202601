using System.Threading.Tasks;
using PolyQA.Executor;
using PolyQA.Network;
using Polyscape.RPCLite;
using UnityEngine;

namespace PolyQA.Query
{
    public class GameObjectQuerySender : DataSenderPlugin
    {
        private readonly GameObjectQueryService _gameObjectQueryService;

        public GameObjectQuerySender(DataSenderInternal dataSender, GameObjectQueryService gameObjectQueryService) : base(dataSender)
        {
            _gameObjectQueryService = gameObjectQueryService;
        }

        public override void OnConnected(ClientService clientService)
        {
            clientService.Register<int, FindGameObjectRequest>(RPC.FindGameObject, FindGameObject);
            clientService.Register<int[], FindGameObjectRequest>(RPC.FindGameObjects, FindGameObjects);
            clientService.Register<string, int>(RPC.GetGameObjectPath, GetGameObjectPath);
            clientService.Register<bool, int>(RPC.GetGameObjectActive, GetGameObjectActive);
            clientService.Register<string, int>(RPC.GetGameObjectText, GetGameObjectText);
            clientService.Register<string, int>(RPC.GetGameObjectImageName, GetGameObjectImageName);
            clientService.Register<bool, int>(RPC.GetGameObjectCheckboxValue, GetGameObjectCheckboxValue);
            clientService.Register<Vector2, int>(RPC.GetGameObjectScreenPosition, GetGameObjectScreenPosition);
            clientService.Register<Rect, int>(RPC.GetGameObjectScreenRect, GetGameObjectScreenRect);
            clientService.Register<int>(RPC.ReleaseObjectReference, ReleaseObjectReference);
            clientService.Register<int, GetComponentRequest>(RPC.GetComponent, GetComponent);
            clientService.Register<bool, int>(RPC.IsEnabledComponent, IsEnabledComponent);
            clientService.Register<string, GetComponentValueRequest>(RPC.GetComponentValue, GetComponentValue);
        }

        private async ValueTask<int> FindGameObject(FindGameObjectRequest request, int _)
        {
            if (request.GameObjectInstanceId == 0)
            {
                return await MainThreadDispatcher.Run(() =>
                {
                    return _gameObjectQueryService.FindGameObject(request.Name);
                });
            }

            return await MainThreadDispatcher.Run(() =>
            {
                return _gameObjectQueryService.FindGameObject(
                    request.GameObjectInstanceId,
                    request.Name,
                    request.IsRegex,
                    request.IsChildrenOnly);
            });
        }

        private async ValueTask<int[]> FindGameObjects(FindGameObjectRequest request, int _)
        {
            if (request.GameObjectInstanceId == 0)
            {
                return await MainThreadDispatcher.Run(() =>
                {
                    return _gameObjectQueryService.FindGameObjects(request.Name);
                });
            }

            return await MainThreadDispatcher.Run(() =>
            {
                return _gameObjectQueryService.FindGameObjects(
                    request.GameObjectInstanceId,
                    request.Name,
                    request.IsRegex,
                    request.IsChildrenOnly);
            });
        }

        private async ValueTask<string> GetGameObjectPath(int id, int _)
        {
            return await MainThreadDispatcher.Run(() => _gameObjectQueryService.GetPath(id));
        }

        private async ValueTask<bool> GetGameObjectActive(int id, int _)
        {
            return await MainThreadDispatcher.Run(() => _gameObjectQueryService.GetActive(id));
        }

        private async ValueTask<string> GetGameObjectText(int id, int _)
        {
            return await MainThreadDispatcher.Run(() => _gameObjectQueryService.GetText(id));
        }

        private async ValueTask<string> GetGameObjectImageName(int id, int _)
        {
            return await MainThreadDispatcher.Run(() => _gameObjectQueryService.GetImageName(id));
        }

        private async ValueTask<bool> GetGameObjectCheckboxValue(int id, int _)
        {
            return await MainThreadDispatcher.Run(() => _gameObjectQueryService.GetCheckboxValue(id));
        }

        private async ValueTask<Vector2> GetGameObjectScreenPosition(int id, int _)
        {
            return await MainThreadDispatcher.Run(() => _gameObjectQueryService.GetScreenPosition(id));
        }

        private async ValueTask<Rect> GetGameObjectScreenRect(int id, int _)
        {
            return await MainThreadDispatcher.Run(() => _gameObjectQueryService.GetScreenRect(id));
        }

        private void ReleaseObjectReference(int id, int _)
        {
            MainThreadDispatcher.Post(() => _gameObjectQueryService.ReleaseObjectReference(id));
        }

        private async ValueTask<int> GetComponent(GetComponentRequest request, int _)
        {
            return await MainThreadDispatcher.Run(() =>
                _gameObjectQueryService.GetComponent(request.GameObjectInstanceId, request.ComponentClass));
        }

        private async ValueTask<bool> IsEnabledComponent(int componentInstanceId, int _)
        {
            return await MainThreadDispatcher.Run(() =>
                _gameObjectQueryService.IsEnabledComponent(componentInstanceId));
        }

        private async ValueTask<string> GetComponentValue(GetComponentValueRequest request, int _)
        {
            return await MainThreadDispatcher.Run(() =>
                _gameObjectQueryService.GetComponentValue(request.ComponentInstanceId, request.PropertyName));
        }
    }
}