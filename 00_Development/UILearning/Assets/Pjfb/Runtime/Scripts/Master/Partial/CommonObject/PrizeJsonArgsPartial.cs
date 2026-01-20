namespace Pjfb.Master {
   
   public partial class PrizeJsonArgs {
	   public PrizeJsonArgs() { }
	   
	   public PrizeJsonArgs(PrizeJsonArgs prizeJsonArgs)
	   {
		   this.mPointId = prizeJsonArgs.mPointId;
		   this.mAvatarId = prizeJsonArgs.mAvatarId;
		   this.mCharaId = prizeJsonArgs.mCharaId;
		   this.pieceMCharaId = prizeJsonArgs.pieceMCharaId;
		   this.mSkillPartId = prizeJsonArgs.mSkillPartId;
		   this.variableMCharaId = prizeJsonArgs.variableMCharaId;
		   this.variableTrainerMCharaId = prizeJsonArgs.variableTrainerMCharaId;
		   this.mIconId = prizeJsonArgs.mIconId;
		   this.mTitleId = prizeJsonArgs.mTitleId;
		   this.mChatStampId = prizeJsonArgs.mChatStampId;
           this.mProfilePartId = prizeJsonArgs.mProfilePartId;
		   this.value = prizeJsonArgs.value;
		   this.get = prizeJsonArgs.get;
		   this.lockId = prizeJsonArgs.lockId;
		   this.message = prizeJsonArgs.message;
	   }
   }
   
}