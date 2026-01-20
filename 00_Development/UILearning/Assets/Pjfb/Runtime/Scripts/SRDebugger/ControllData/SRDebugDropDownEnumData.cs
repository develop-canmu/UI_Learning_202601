#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT

using System;

namespace Pjfb.SRDebugger
{
    //// <summary> SRDebuggerに表示するドロップダウン対応Enumデータ </summary>
    public struct SRDebugDropDownEnumData
    {
        private Enum value;
        public Enum Value => value;

        // 表示したくないEnum値の定義
        private int[] hideTypes;
        public int[] HideTypes => hideTypes;
        
        public SRDebugDropDownEnumData(Enum value, int[] hideTypes = null)
        {
            this.value = value;
            this.hideTypes = hideTypes;
        }
    }
}
#endif