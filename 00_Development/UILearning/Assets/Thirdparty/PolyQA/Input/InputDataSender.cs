using PolyQA.Input.Actions;
using PolyQA.Network;
using Polyscape.RPCLite;

namespace PolyQA.Input
{
    public class InputDataSender : DataSenderPlugin
    {
        private readonly RuntimeContext _context;
        private readonly InputActionExecutor _inputActionExecutor;

        public InputDataSender(
            DataSenderInternal dataSender,
            RuntimeContext context,
            InputActionExecutor inputActionExecutor) : base(dataSender)
        {
            _context = context;
            _inputActionExecutor = inputActionExecutor;
        }

        public override void OnConnected(ClientService clientService)
        {
            clientService.Register<InputActionRequest>(RPC.InputAction, HandleInputAction);
        }

        private void HandleInputAction(InputActionRequest request, int _)
        {
            _inputActionExecutor.Execute(new InputActionBundle(_context, request));
        }
    }
}