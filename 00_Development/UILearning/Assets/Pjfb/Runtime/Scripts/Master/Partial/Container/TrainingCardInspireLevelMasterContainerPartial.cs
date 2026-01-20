
using System.Linq;

namespace Pjfb.Master {

    public partial class TrainingCardInspireLevelMasterContainer : MasterContainerBase<TrainingCardInspireLevelMasterObject> {
        long GetDefaultKey(TrainingCardInspireLevelMasterObject masterObject){
            return masterObject.id;
        }
        
        private TrainingCardInspireLevelMasterObject[] orderedByLevel = null;
		/// <summary>
		/// レベル順位ソートした配列
		/// </summary>
        public TrainingCardInspireLevelMasterObject[] OrderedByLevel
        {
            get
            {
                if(orderedByLevel == null)
                {
                    orderedByLevel = values.OrderByDescending(v=>v.level).ToArray();
                }
                return orderedByLevel;
            }
        }
    }
}
