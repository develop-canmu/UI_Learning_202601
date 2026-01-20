using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CruFramework.Adv
{
	[System.Serializable]
    public sealed class AdvCommandOverrideSpeaker : IAdvCommand
    {
    
	    [SerializeField]
	    [AdvDocument("話者。")]
	    private string speaker = string.Empty;
	    public string Speaker{get{return speaker;}set{speaker = value;}}
	    
	    [SerializeField]
	    [AdvObjectId(nameof(AdvConfig.MessageWindows))]
	    [AdvDocument("書き換えるメッセージウィンドウ。")]
	    private int windowId = 0;

	
	    void IAdvCommand.Execute(AdvManager manager)
	    {
		    // メッセージウィンドウの取得
		    AdvMessageWindow messageWindow =  manager.GetMessageWindow(windowId);
		    messageWindow.SetSpeaker(speaker);
	
	    }
    }
}