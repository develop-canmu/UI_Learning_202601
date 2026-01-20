using System.Collections.Generic;
using System.Linq;

namespace Pjfb
{
    public class RecommendCharaScroll : ItemIconScroll<RecommendCharaData>
    {
        private List<RecommendCharaData> scrollDataList = new();
        
        public void SetItems(IEnumerable<RecommendCharaData> data)
        {
            scrollDataList = data.ToList();
            Refresh();
        }

        protected override List<RecommendCharaData> GetItemList()
        {
            return scrollDataList;
        }
    }
}
