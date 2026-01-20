
using System;

namespace Pjfb.Master {
	public partial class BillingRewardMasterObject : BillingRewardMasterObjectBase, IMasterObject {
		public string GetProductId()
		{
#if UNITY_IOS
			return appleProductKey;
#elif UNITY_ANDROID
			return googleProductKey;
#else
			return appleProductKey;
#endif 
		}

		public enum StoreType
		{
			IOS = 1, // iOS
			Android = 2, // Android
			External = 11,
		}
	}

}
