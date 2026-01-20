using Pjfb.Colosseum;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Cysharp.Threading.Tasks;

namespace Pjfb.ClubMatch
{
    public class ClubMatchRecordModal : ColosseumRecordModal
    {

        private class ClubMatchModalParams : ColosseumRecordModal.ModalParams
        {
            public long sColosseumEventId;

            public ClubMatchModalParams(ColosseumEventMasterObject mColosseumEvent, long sColosseumEventId) : base(mColosseumEvent)
            {
                this.sColosseumEventId = sColosseumEventId;
            }
        }

        public static void Open(ColosseumEventMasterObject colosseumEventMasterObject, long sColosseumEventId)
        {
            var modalParams = new ClubMatchModalParams(colosseumEventMasterObject, sColosseumEventId);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubMatchRecord,modalParams);
        }
        
        protected override async UniTask<ColosseumHistory[]> GetHistoryList()
        {
            var value = modalParams as ClubMatchModalParams;
            return await ColosseumManager.RequestGetHistoryListAsyncBySColosseumEventId(value?.sColosseumEventId ?? 0);
        }
    }
}