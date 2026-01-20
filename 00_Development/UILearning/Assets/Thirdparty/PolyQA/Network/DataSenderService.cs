using System;

namespace PolyQA.Network
{
    public class DataSenderService : IDisposable
    {
        private readonly DataSenderInternal _dataSender;

        public DataSenderService(DataSenderInternal dataSender)
        {
#if !POLYQA_DISABLE
            _dataSender = dataSender;
            _dataSender.Start();
#endif
        }

        public void Dispose()
        {
#if !POLYQA_DISABLE
            _dataSender.Stop();
#endif
        }
    }
}