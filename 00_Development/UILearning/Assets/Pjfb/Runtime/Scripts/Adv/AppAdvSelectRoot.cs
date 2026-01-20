using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Adv;

namespace Pjfb
{
    public class AppAdvSelectRoot : AdvSelectRoot
    {
        private static readonly string OpenAnimation = "Open";
        private static readonly string CloseAnimation = "Close";
        
        [SerializeField]
        private Animator animator = null;

        [SerializeField]
        private AppAdvSelectButton[] selectButtons = null;
        
        private string[] messages = null;
        private AdvManager manager = null;
        private bool isSelected = false;

        public override void Select(int no)
        {
            SelectAsync(no).Forget();   
        }
        
        public virtual async UniTask SelectAsync(int no)
        {
            if(isSelected)return;
            isSelected = true;
            
            int messageIndex = 0;
            
            if(manager.IsSkip == false)
            {
                List<UniTask> tasks = new List<UniTask>();
                for(int i=0;i<selectButtons.Length;i++)
                {
                    if(selectButtons[i].gameObject.activeSelf == false)continue;
                    
                    if(selectButtons[i].No == no)
                    {
                        messageIndex = i;
                        tasks.Add(selectButtons[i].PlaySelectAnimation());
                    }
                    else
                    {
                        tasks.Add(selectButtons[i].PlayDisappearAnimation());
                    }
                }
                
                await UniTask.WhenAll(tasks);
            }

            
            // ログを追加
            if(manager != null && messages != null)
            {
                manager.AddMessageLog(StringValueAssetLoader.Instance["adv.select.speaker_text"], manager.ReplaceText(messages[messageIndex]), 0);
            }
            
            // １始まり
            ParentManager.SetValue(AdvConstants.SelectDataKey, no);
            
            await CloseAsync();
        }

        public override async UniTask OpenAsync(AdvManager manager, string[] messages, int[] nos)
        {
            isSelected = false;
            this.manager = manager;
            this.messages = messages;
            
            if(nos == null)
            {
                nos = new int[messages.Length];
                for(int i=0;i<nos.Length;i++)
                {
                    nos[i] = i+1;
                }
            }
            
        
            if(messages.Length <= 0)
            {
                CruFramework.Logger.LogError("選択肢のデータ数が0です");
            }

            
            // 不要なボタンを非表示に
            for(int i=messages.Length;i<selectButtons.Length;i++)
            {
                selectButtons[i].gameObject.SetActive(false);
            }
            
            // ボタンのテキスト表示
            for(int i=0;i<messages.Length;i++)
            {
                selectButtons[i].SetMessage( manager.ReplaceText(messages[i]) );
                selectButtons[i].No = nos[i];
                selectButtons[i].gameObject.SetActive(true);
            }
            
            ParentManager.IsStopCommand = true;
        
            if(manager.IsSkip == false)
            {
                await AnimatorUtility.WaitStateAsync(animator, OpenAnimation);
            }
        }

        public override void Close()
        {
            CloseAsync().Forget();
        }
        
        public async UniTask CloseAsync()
        {
            if(manager.IsSkip == false)
            {
                await AnimatorUtility.WaitStateAsync(animator, CloseAnimation);
            }
            // すべてのボタンを非表示に
            foreach(AdvSelectButton selectButton in selectButtons)
            {
                selectButton.gameObject.SetActive(false);
            }
            
            ParentManager.IsStopCommand = false;
        }
    }
}