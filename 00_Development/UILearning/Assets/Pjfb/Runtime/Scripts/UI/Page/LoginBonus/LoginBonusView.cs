using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Voice;
using TMPro;

namespace Pjfb.LoginBonus
{
    public class LoginBonusView : MonoBehaviour {

        #region Parameters
        [SerializeField] private ScrollBanner _prizeScrollBanner;
        [SerializeField] TextMeshProUGUI _message = null;
        [SerializeField] TextMeshProUGUI _periodText = null;
        [SerializeField] TextMeshProUGUI _weekIndexText= null;
        [SerializeField] LoginBonusListItem _todayPrize= null;
        [SerializeField] GameObject _messageObj= null;
        [SerializeField] GameObject _periodObj= null;
        [SerializeField] GameObject _weekTextObj= null; 
        [SerializeField] GameObject _oneWeekObj= null; 
        [SerializeField] CancellableRawImage _backgroundImage= null;
        [SerializeField] Animator animator = null;

        public System.Action onFinished{get;set;} = null;
        private List<LoginBonusRewardView> _rewardViews = new List<LoginBonusRewardView>();
        private List<object> _voiceList = new List<object>();
        private long defaultPageIndex = 0;
        private bool isShowEnd = false;
        #endregion
        
        private void Awake()
        {
            // ページ変更通知
            _prizeScrollBanner.onChangedPage += OnChangePage;
        }
        public async UniTask Init( long masterId, long currentDay ){
            var master = MasterManager.Instance.loginStampMaster.FindData(masterId);
            if( master == null ) {
                CruFramework.Logger.LogError("not find loginStampMaster : " + masterId);
                return;
            }
            isShowEnd = false;
            //開催時間、メッセージ設定
            DateTime endDateTime = master.endAt.TryConvertToDateTime();
            string endDate = master.endAt.TryConvertToDateTime().ToString("yyyy年MM月dd日 HH時mm分");
            _periodText.text = String.Format(StringValueAssetLoader.Instance["common.end_at"], endDate); 
            var prizeMaster = MasterManager.Instance.loginStampPrizeMaster.FindData( master.id ,currentDay);
            if( prizeMaster != null ) {
                _message.text = prizeMaster.dialog;
                _todayPrize.Init(prizeMaster.prizeJson.First(),currentDay,false);
                _voiceList = string.IsNullOrEmpty(master.voiceList) ? new () : (List<object>)MiniJSON.Json.Deserialize(master.voiceList);
            }
            
            //N週目設定
            UpdateDisplay(defaultPageIndex);
            
            //Prize スクロールデータ設定
            SetScrollBanner(masterId, master.prizeCount, currentDay);
            
            //背景設定
            var path = CruFramework.ResourcePathManager.GetPath("LoginBonusBackground", masterId);
            await _backgroundImage.SetTextureAsync(path);
            
            //表示Object設定
            _weekTextObj.SetActive(master.prizeCount > 7);
            _oneWeekObj.SetActive(master.prizeCount > 7);
            bool showPeriod = (endDateTime != DateTime.MinValue && (endDateTime - AppTime.Now).TotalDays < 365);
            _periodObj.transform.localScale = showPeriod ? Vector3.one : Vector3.zero;
            _messageObj.SetActive(prizeMaster != null && !String.IsNullOrEmpty(prizeMaster.dialog));
            _prizeScrollBanner.ScrollGrid.enabled = master.prizeCount > 7; 
            //Open演出
            animator.SetTrigger("Open");
        }
        
        private async void SetScrollBanner(long id,long prizeCount,long currentDay)
        {
            var bannerData = new List<LoginBonusRewardView.Info>();
            defaultPageIndex = (currentDay - 1) / 7;
            int pageIndex = 0;
            int startDay = 0;
            long diff = prizeCount;
            while (diff > 0)
            {
                long viewPrizeCount = (diff >= 7) ? 7 : diff;
                bannerData.Add(new LoginBonusRewardView.Info
                {
                    count = viewPrizeCount,
                    stampId = id,
                    startDay = startDay,
                    currentDay = currentDay,
                    index = pageIndex,
                    IsShowEnd = () => isShowEnd
                });
                startDay += 7;
                pageIndex++;
                diff = prizeCount - startDay;
            }
            
            _prizeScrollBanner.SetBannerDatas(bannerData);
            //ScrollView設定まで1Frame待ち
            await UniTask.NextFrame();
            _prizeScrollBanner.ScrollGrid.SetPage((int)defaultPageIndex,true);
            _rewardViews.Clear();
            foreach (Transform child in _prizeScrollBanner.ScrollGrid.content)
            {
                var item = child.GetComponent<LoginBonusRewardView>();
                if(item != null) _rewardViews.Add(item);
            }
        }

        public void OnClickNextButton(){
            var currentState = animator.GetCurrentAnimatorStateInfo(0);
            if (currentState.IsName("Open"))
            {
                //スキップ処理
                animator.SetTrigger("Idle");
                _rewardViews.FirstOrDefault(v => v.gameObject.activeSelf && v.info?.index == defaultPageIndex)?.ShowGetStampAnimation(true);
                OnAnimationEventPlayVoice();
            }
            else if (currentState.IsName("Idle"))
            {
                //Close演出
                animator.SetTrigger("Close");
            }
        }

        /// <summary>
        /// アニメーションのEventから再生、Stamp獲得演出
        /// </summary>
        public void OnAniEventCallShowStamp()
        {
            _rewardViews.FirstOrDefault(v => v.gameObject.activeSelf && v.info?.index == defaultPageIndex)?.ShowGetStampAnimation();
        }
        
        /// <summary>
        /// アニメーションIdleに入った
        /// </summary>
        public void OnAnimationEventIdle()
        {
            isShowEnd = true;
        }

        /// <summary>
        /// アニメーションのEventから再生、Close演出終了後
        /// </summary>
        public void OnAnimationEventClosed()
        {
            onFinished?.Invoke();
        }
        
        /// <summary>
        /// アニメーションのEventから再生、ボイスランダム再生
        /// </summary>
        public void OnAnimationEventPlayVoice()
        {
            if (!_voiceList.Any()) return;
            var index = UnityEngine.Random.Range(0, _voiceList.Count);
            VoiceManager.Instance.StopVoice();
            VoiceManager.Instance.PlayVoiceAsync(_voiceList[index].ToString()).Forget();
            _voiceList.Clear();
        }
        
        private void UpdateDisplay(long index)
        {
            if(_oneWeekObj.activeSelf) _weekIndexText.text = (index+1).ToString();
        }
        
        private void OnChangePage(int index)
        {
            UpdateDisplay(index);
        }
        
    }
}