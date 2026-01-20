using Polyscape.RPCLite;

namespace PolyQA.Network
{
    public abstract class DataSenderPlugin
    {
        protected DataSenderInternal DataSender { get; }

        protected DataSenderPlugin(DataSenderInternal dataSender)
        {
            DataSender = dataSender;
            dataSender.RegisterPlugin(this);
        }

        public virtual void OnConnected(ClientService clientService) {}
    }
}