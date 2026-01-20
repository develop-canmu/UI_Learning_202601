namespace Pjfb.Master {
   
   public partial class PrizeJsonWrap {
	   public PrizeJsonWrap(){}
	   
	   public PrizeJsonWrap(PrizeJsonWrap prizeJsonWrap)
	   {
		   this.type = prizeJsonWrap.type;
		   this.description = prizeJsonWrap.description;
		   this.args = new PrizeJsonArgs(prizeJsonWrap.args);
	   }
   }
   
}