
using System.Linq;

namespace Pjfb.Master {

    public partial class GuildRankMasterContainer : MasterContainerBase<GuildRankMasterObject> {
        long GetDefaultKey(GuildRankMasterObject masterObject){
            return masterObject.id;
        }

        public long GetRankIdByGradeNumber(long gradeNumber)
        {
            var rankObject = values.FirstOrDefault(v => v.colosseumGradeNumber == gradeNumber);
            return (rankObject != null) ? rankObject.id : 0;
        }
    }
}
