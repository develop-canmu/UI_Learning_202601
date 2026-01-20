using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using Cysharp.Threading.Tasks;
using UnityEngine;

using Pjfb.Adv;
using Pjfb.Master;
using Pjfb.Voice;

namespace Pjfb.Training
{
    public class TrainingPageBase : Page
    {
        public TrainingMain MainPageManager{get{return (TrainingMain)Manager;}}
        
        /// <summary>ヘッダーのショートカット</summary>
        public TrainingHeader Header{get{return MainPageManager.Header;}}
        
        /// <summary>フッターのショートカット</summary>
        public AdvFooter Footer{get{return MainPageManager.Adv.Footer;}}
        
        public TrainingMainArguments MainArguments{get{return (TrainingMainArguments)OpenArguments;}}
        
        
        /// <summary>Advのショートカット</summary>
        public AppAdvManager Adv{get{return MainPageManager.Adv;}}
        
        /// <summary>早送り</summary>
        public bool IsFastMode{get{return Adv.AppAutoMode == AppAdvAutoMode.Fast || Adv.AppAutoMode == AppAdvAutoMode.Skip3 || Adv.AppAutoMode == AppAdvAutoMode.Skip4;}}
        /// <summary>超高速</summary>
        public bool IsSkipMode{get{return Adv.AppAutoMode == AppAdvAutoMode.Skip3 || Adv.AppAutoMode == AppAdvAutoMode.Skip4;}}
        
        /// <summary>シナリオスキップ</summary>
        public bool IsScenarioSkip{get{return Adv.AppAutoMode == AppAdvAutoMode.Skip4;}}
        /// <summary>休憩キップ</summary>
        public bool IsRestSkip{get{return Adv.AppAutoMode == AppAdvAutoMode.Skip4;}}
        /// <summary>目標達成演出スキップ</summary>
        public bool IsTargetEffectSkip{get{return Adv.AppAutoMode == AppAdvAutoMode.Skip4;}}
        /// <summary>インスピ取得演出スキップ</summary>
        public bool IsGetInspirationEffectSkip{get{return Adv.AppAutoMode == AppAdvAutoMode.Skip4;}}
        /// <summary>コンセントレーション演出スキップ</summary>
        public bool IsConcentrationEffectSkip{get{return Adv.AppAutoMode == AppAdvAutoMode.Skip4;}}
        /// <summary>インスピブースト演出スキップ</summary>
        public bool IsInspirationBoostEffectSkip{get{return Adv.AppAutoMode == AppAdvAutoMode.Skip4;}}
        /// <summary>トータルボーナス演出スキップ</summary>
        public bool IsTotalBonusEffectSkip{get{return Adv.AppAutoMode == AppAdvAutoMode.Skip4;}}
        //// <summary> カードコンボ演出スキップ </summary>
        public bool IsCardComboEffectSkip{get{return Adv.AppAutoMode == AppAdvAutoMode.Skip4;}}
        //// <summary> カードコンボ演出が自動で進むか </summary>
        public bool IsCardComboEffectAuto{get{return Adv.AppAutoMode == AppAdvAutoMode.Fast || Adv.AppAutoMode == AppAdvAutoMode.Skip3;}}
        
        private bool footerActiveState = false;

        public void PlayTrainingCharacterVoice(VoiceResourceSettings.LocationType locationType)
        {
            PlayTrainingCharacterVoiceAsync(locationType).Forget();
        }
        
        public UniTask PlayTrainingCharacterVoiceAsync(VoiceResourceSettings.LocationType locationType)
        {
            return VoiceManager.Instance.PlaySystemVoiceForLocationTypeAsync( MasterManager.Instance.charaMaster.FindData(MainArguments.TrainingCharacter.MCharId), locationType);
        }

        public UniTask PlayTrainingCharacterInVoiceAsync(VoiceResourceSettings.LocationType locationType)
        {
            return VoiceManager.Instance.PlayInVoiceForLocationTypeAsync( MasterManager.Instance.charaMaster.FindData(MainArguments.TrainingCharacter.MCharId), locationType);
        }

        public virtual void OpenAbordModal()
        {
            OpenAbordModalAsync().Forget();
        }
        
        public async UniTask OpenAbordModalAsync()
        {
            // Advを停止させる
            Adv.IsPause = true;
            CruFramework.Page.ModalWindow modal = await TrainingUtility.OpenAbordModalAsync();
            // モーダルが閉じるまで待つ
            bool movePage = (bool)await modal.WaitCloseAsync();
            
            // Advを再開させる
            if(movePage == false)
            {
                Adv.IsPause = false;
            }
        }
        
        public void SetTouchGurad(bool enable)
        {
            if(enable)
            {
                // タッチガード
                AppManager.Instance.UIManager.System.TouchGuard.Show();
                // フッターを非表示に
                footerActiveState = Footer.gameObject.activeSelf;
                Footer.gameObject.SetActive(false);
            }
            else
            {
                // タッチガード
                AppManager.Instance.UIManager.System.TouchGuard.Hide();
                // フッターを非表示に
                Footer.gameObject.SetActive(footerActiveState);
            }
        }

        
        /// <summary>メッセージの表示</summary>
        public void SetMessage(string msg)
        {
            MainPageManager.SetMessage(msg);
        }
        
        public void ReservationMessage(string msg)
        {
            MainPageManager.ReservationMessage(msg);
        }
        
        /// <summary>予約したメッセージの表示</summary>
        public void SetReservationMessage()
        {
            MainPageManager.SetReservationMessage();
        }
        
        public bool IsShowMessage()
        {
            return MainPageManager.IsShowMessage();
        }
        
        public virtual void OnBackkey()
        {
            SEManager.PlaySE(SE.se_common_tap);
            OpenAbordModal(); 
        }
        
        /// <summary>ページを開く</summary>
        public void OpenPage(TrainingMainPageType pageType, object args)
        {
            MainPageManager.OpenPage(pageType, false, args);
        }
        public async UniTask OpenPageAsync(TrainingMainPageType pageType, object args)
        {
            await MainPageManager.OpenPageAsync(pageType, false, args);
        }
    }
}