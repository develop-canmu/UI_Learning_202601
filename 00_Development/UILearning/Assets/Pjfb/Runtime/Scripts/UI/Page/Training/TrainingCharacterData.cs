namespace Pjfb
{
    public struct TrainingCharacterData
    {
        private long mCharId;
        /// <summary>mCharId</summary>
        public long MCharId{get{return mCharId;}}
            
        private long lv;
        /// <summary>Lv</summary>
        public long Lv{get{return lv;}}
        
        private long liberationId;
        /// <summary>Id</summary>
        public long LiberationId{get{return liberationId;}}
        
        private long uCharId;
        /// <summary>Id</summary>
        public long UCharId{get{return uCharId;}}

        public TrainingCharacterData(long mCharId, long lv, long liberationId, long uCharId)
        {
            this.mCharId = mCharId;
            this.lv = lv;
            this.liberationId = liberationId;
            this.uCharId = uCharId;
        }
    }
}
