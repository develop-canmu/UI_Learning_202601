using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CruFramework.Adv
{
	[System.Serializable]
    public sealed class AdvCommandMessage : IAdvCommand, IAdvResource, IAdvFastForward, IAdvEnableSkipButton
    {
    
	    [SerializeField]
	    [AdvObjectId(nameof(AdvConfig.CharacterDatas), AdvObjectIdAttribute.WindowType.SearchWindow)]
	    [AdvDocument("話者。")]
	    private int speaker = 0;
	    public int Speaker{get{return speaker;}set{speaker = value;}}
	    
	    [SerializeField]
	    [AdvDocument("話者を手前に移動させる。")]
	    private bool front = true;

	    [SerializeField]
	    [AdvObjectId(nameof(AdvConfig.MessageWindows))]
	    [AdvDocument("メッセージを表示するメッセージウィンドウ。")]
	    private int windowId = 0;

	    [SerializeField]
	    [AdvDocument("クリック待ち状態にする。")]
	    private bool isWait = true;
	    
	    [SerializeField]
	    [AdvDocument("再生するボイスId。")]
	    private string voicePath = string.Empty;

	    
	    [SerializeField][TextArea]
	    [AdvDocument("表示する文字列")]
	    private string message = string.Empty;
	    public string Message{get{return message;}set{message = value;}}
	    
	    /// <summary>リソースの事前読み込み</summary>
	    async UniTask IAdvResource.PreLoadResources(AdvManager manager)
	    {
		    if( string.IsNullOrEmpty(voicePath) == false)
		    {
			    await manager.PreLoadResourceAsync(manager.Config.VoiceResourcePathId, voicePath);
		    }
	    }
	    
	    bool IAdvFastForward.OnNext(AdvManager manager)
	    {
		    // メッセージウィンドウの取得
		    AdvMessageWindow messageWindow =  manager.GetMessageWindow(windowId);
		    //bool isEndMessage = true;

		    if(messageWindow.IsEndMesssageAnimation == false)
		    {
			    messageWindow.ForceEndMessageAnimation();
			    return false;
		    }
		    
		    return true;
	    }
	    
	    bool IAdvEnableSkipButton.EnableSkipButton()
	    {
		    return isWait;
	    }
	    
	    void IAdvCommand.Execute(AdvManager manager)
	    {
		    // クリック待機
		    if(isWait)
		    {
			    manager.WaitClick();
		    }
		    // 話者の取得
		    AdvCharacter speakerObject = speaker <= 0 ? null : manager.GetAdvCharacter<AdvCharacter>(speaker);
		    // メッセージウィンドウの取得
		    AdvMessageWindow messageWindow =  manager.GetMessageWindow(windowId);
		    // アクティブに
		    messageWindow.gameObject.SetActive(true);
		    
		    // メッセージ
		    string msg = manager.ReplaceText(message);
		    // 話者
		    string speakerName = string.Empty;
		    // メッセージの表示
		    messageWindow.SetMessage(msg);
		    // 話者の表示
		    messageWindow.ShowSpeaker(speakerObject != null);
		    // 話者の設定
		    if(speakerObject != null)
			{
				speakerName = manager.ReplaceText(speakerObject.DataName);
				// メッセージウィンドウに表示
				messageWindow.SetSpeaker(speakerName);
				messageWindow.SetSpeakerRuby(speakerObject.Data.NameRuby);
				// 手前に
				if(front)
				{
					speakerObject.ToFront();
				}
				
				// キャラクタのハイライト
				manager.GrayoutCharacters();
				speakerObject.Highlight();
			}

		    // ボイスの再生 スキップ中はボイス再生しない
		    if( string.IsNullOrEmpty(voicePath) == false && manager.IsFastMode == false && manager.IsSkip == false)
		    {
			    // ボイス再生
			    manager.PlayVoice(speakerObject, manager.Config.VoiceResourcePathId, voicePath);
			    messageWindow.SetVoicePlayer( manager.VoicePlayer );
		    }
		    else
		    {
			    messageWindow.SetVoicePlayer(null);
		    }
		    
		    // ログに追加
		    manager.AddMessageLog(speakerName, msg, speaker);
	    }
    }
}