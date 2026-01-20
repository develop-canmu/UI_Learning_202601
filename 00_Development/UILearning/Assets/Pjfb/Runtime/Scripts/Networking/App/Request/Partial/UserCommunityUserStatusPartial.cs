//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request
{
	public partial class UserCommunityUserStatus
	{
		public override bool Equals(object obj)
		{
			UserCommunityUserStatus userStatus = obj as UserCommunityUserStatus;
			return userStatus != null && userStatus.uMasterId == this.uMasterId;
		}

		public override int GetHashCode()
		{
			//適当にoverride
			return (uMasterId, name).GetHashCode();
		}

	}
}