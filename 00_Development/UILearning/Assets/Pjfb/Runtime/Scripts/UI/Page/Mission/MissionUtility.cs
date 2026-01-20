using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Pjfb
{
    public static class MissionUtility
    {
	    public static readonly string StringValueReceive = "mission.receive";
	    public static readonly string StringValueReceived = "mission.received";
	    public static readonly string StringValueChallenge = "mission.challenge";
	    public static readonly string StringValueProgress = "mission.progress";
	    
	    
	    public static string GetProgressString(int progress, int progressMax)
	    {
		    return string.Format(StringValueAssetLoader.Instance[StringValueProgress], progress, progressMax);
	    }
    }
}