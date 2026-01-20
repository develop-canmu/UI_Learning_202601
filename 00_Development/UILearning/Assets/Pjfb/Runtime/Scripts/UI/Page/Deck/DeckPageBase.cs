using System.Threading;
using Cysharp.Threading.Tasks;

namespace Pjfb.Deck
{
    public class DeckPageParameters
    {
        public PageType OpenFrom;
        public long InitialPartyNumber;
    }
    
    public class DeckPageBase<T> : PageManager<T> where T : System.Enum
    {
        public static DeckListData DeckListData;
        public static int CurrentDeckIndex;
        public static DeckData CurrentDeckData => DeckListData.DeckDataList[CurrentDeckIndex];
        
        protected override string GetAddress(T page)
        {
            throw new System.NotImplementedException();
        }
        
        protected override async UniTask<bool> OnPreLeave(CancellationToken token)
        {
            bool result = true;
            // 変更が入っているか
            if (IsDeckChanged())
            {
                // 編成キャンセルの確認
                result = await DeckUtility.OnLeaveCurrentDeck(this.GetCancellationTokenOnDestroy());
                if (result)
                {
                    CurrentDeckData.DiscardChanges();
                }
            }
            return result;
        }
        
        private bool IsDeckChanged()
        {
            return CurrentDeckData.IsDeckChanged;
        }
    }
}