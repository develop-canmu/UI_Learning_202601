using System;
using PolyQA.Network;
using Polyscape.RPCLite;

namespace PolyQA.Record
{
    public class InputRecordDataSender : DataSenderPlugin
    {
        private readonly RuntimeContext _context;

        public InputRecordDataSender(DataSenderInternal dataSender, RuntimeContext context) : base(dataSender)
        {
            _context = context;
        }

        public override void OnConnected(ClientService clientService)
        {
            clientService.Register<Void>(RPC.StartInputRecording, StartInputRecording);
            clientService.Register<Void>(RPC.StopInputRecording, StopInputRecording);
        }

        private void StartInputRecording(Void _, int clientId)
        {
            _context.InputRecordService.StartRecording();
        }

        private void StopInputRecording(Void _, int clientId)
        {
            _context.InputRecordService.StopRecording();
        }

        public void SendInputRecord(IInputActionSequence[] sequences, TimeSpan delay)
        {
            DataSender.CallRpc(RPC.InputRecordUpdate, new InputRecordUpdate(sequences, delay));
        }
    }
}