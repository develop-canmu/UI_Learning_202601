
namespace Pjfb.Master {

	public enum ExchangeStoreType
	{
		Other,
		Limited,
		Daily,
		Item,
		Material
	}

	public partial class CommonStoreCategoryMasterObject : CommonStoreCategoryMasterObjectBase, IMasterObject {

		public ExchangeStoreType GetExchangeStoreType()
		{
			return (ExchangeStoreType)type;
		}
	}

}
