
using System;
using Pjfb.Extensions;

namespace Pjfb.Master {
	public partial class PointAlternativeMasterObject : PointAlternativeMasterObjectBase, IMasterObject {
		
		private DateTime? startAtDateTime = null;
		/// <summary>開始時間</summary>
		public DateTime StartAtDateTime
		{
			get
			{
				if(startAtDateTime == null)
				{
					startAtDateTime = startAt.TryConvertToDateTime();
				}
				return startAtDateTime.Value;
			}
		}
		
		private DateTime? endAtDateTime = null;
		/// <summary>終了時間</summary>
		public DateTime EndAtDateTime
		{
			get 
			{
				if(endAtDateTime == null)
				{
					endAtDateTime = endAt.TryConvertToDateTime();
				}
				return endAtDateTime.Value;
			}
		}
	}

}
