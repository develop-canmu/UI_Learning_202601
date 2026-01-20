
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using Spine.Unity;
using CruFramework.Adv;
using Pjfb.Training;

namespace Pjfb.Adv
{
	
	public class AdvTrainingEventResult
	{
		private CharacterStatus statusUpValue = new CharacterStatus();
		/// <summary>ステータス上昇量</summary>
		public  CharacterStatus StatusUpValue{get{return statusUpValue;}set{statusUpValue = value;}}
		
		private long performanceLv = 0;
		/// <summary>パフォーマンスレベル</summary>
		public long PerformanceLv{get{return performanceLv;}set{performanceLv = value;}}
		
		private bool isLvMaxPerformance = false;
		/// <summary>パフォーマンスレベルが最大</summary>
		public bool IsLvMaxPerformance{get{return isLvMaxPerformance;}set{isLvMaxPerformance = value;}}
        
		private long practiceId = -1;
		/// <summary>練習Id</summary>
		public long PracticeId{get{return practiceId;}set{practiceId = value;}}
        
		private long practiceLv = 0;
		/// <summary>練習レベル</summary>
		public long PracticeLv{get{return practiceLv;}set{practiceLv = value;}}
		
		private bool isLxMaxPractice = false;
		/// <summary>練習レベルが最大</summary>
		public bool IsLxMaxPractice{get{return isLxMaxPractice;}set{isLxMaxPractice = value;}}
		
		private long tipCount = 0;
		/// <summary>獲得チップ枚数</summary>
		public long TipCount{get{return tipCount;}set{tipCount = value;}}
		
		private long inspirationBoost = 0;
		/// <summary>インスピレーションブースト値</summary>
		public long InspirationBoost{get{return inspirationBoost;}set{inspirationBoost = value;}}
	}
	
    [System.Serializable]
    public class AdvCommandTrainingStatusUp : IAdvCommand, IAdvCommandOnNext, IAdvFastForward
    {
    
    
	    [SerializeField]
        [AdvObjectId(nameof(AdvConfig.MessageWindows))]
        [AdvDocument("メッセージを表示するメッセージウィンドウ。")]
        private int windowId = 0;
        
        [SerializeField]
        [AdvDocument("一度に表示するステータス数。")]
        private int messageLineCount = 3;

        private int messageIndex = 0;
        private List<string> messageList = new List<string>();

        bool IAdvFastForward.OnNext(AdvManager manager)
	    {
		    // メッセージウィンドウの取得
		    AdvMessageWindow messageWindow =  manager.GetMessageWindow(windowId);

		    if(messageWindow.IsEndMesssageAnimation == false)
		    {
			    messageWindow.ForceEndMessageAnimation();
			    return false;
		    }
		    
		    return true;
	    }

        void IAdvCommandOnNext.OnNext(AdvManager manager)
        {
	        NextMessage(manager);	        
        }

        void IAdvCommand.Execute(AdvManager manager)
        {
	        
	        AppAdvManager m = (AppAdvManager)manager;
	        
	        manager.IsStopCommand = true;
            // メッセージウィンドウの取得
            AdvMessageWindow messageWindow =  manager.GetMessageWindow(windowId);
            // アクティブに
            messageWindow.gameObject.SetActive(true);
            // 話者の表示
            messageWindow.ShowSpeaker(false);            
            
            // 変数初期化
            messageIndex = 0;
            messageList.Clear();
            // ブースト値の表示
            string inspirationBoost = TrainingUtility.GetInspirationBoostMessage(m.EventResult.InspirationBoost);
            // ステータス上昇データを作成
            foreach(CharacterStatusType type in TrainingUtility.StatusUpTypes)
            {
	            if(m.EventResult.StatusUpValue[type] > 0)
	            {
		            // ここはインフレ表示対応不要らしい
		            messageList.Add(string.Format(StringValueAssetLoader.Instance["training.status_up_message"], StatusUtility.GetStatusName(type), m.EventResult.StatusUpValue[type].ToString()) + inspirationBoost);
		            
	            }
            }
            
            // チップ獲得枚数
            if(m.EventResult.TipCount > 0)
            {
	            messageList.Add(string.Format(StringValueAssetLoader.Instance["training.tip_get_message"], m.EventResult.TipCount));
	        }
            
            // パフォーマンスLv
            if(m.EventResult.PerformanceLv > 0)
            {
	            messageList.Add(string.Format(StringValueAssetLoader.Instance["training.perfomance_up_message"], m.EventResult.IsLvMaxPerformance ? StringValueAssetLoader.Instance["training.log.performance_lv_max"] : m.EventResult.PerformanceLv));
	        }
            
            // 練習Lv
            if(m.EventResult.PracticeLv > 0)
            {
	            string name = StringValueAssetLoader.Instance[$"practice_name{m.EventResult.PracticeId}"];
	            messageList.Add(string.Format(StringValueAssetLoader.Instance["training.practice_lv_up_message"], name, m.EventResult.IsLxMaxPractice ? StringValueAssetLoader.Instance["training.log.practice_lv_max"] : m.EventResult.PracticeLv.ToString()));
	        }
            
            // メッセージ表示
            NextMessage(manager);
        }
        
        private void NextMessage(AdvManager manager)
        {
	        // 全部表示した
	        if(messageList.Count <= messageIndex)
		    {
			    manager.IsStopCommand = false;
			    return;
			}
	        
	        // 表示する文字列を作成
	        StringBuilder sb = new StringBuilder();
	        for(int i=0;i<messageLineCount;i++)
	        {
		        // すべて表示済み
		        if(messageList.Count <= messageIndex)break;
		        // ステータス取得
		        sb.AppendLine(messageList[messageIndex++]);
	        }
	        
	        // メッセージウィンドウの取得
	        AdvMessageWindow messageWindow =  manager.GetMessageWindow(windowId);
	        messageWindow.SetMessage(sb.ToString());
        }
    }
}
