using System.Collections;
using System.Collections.Generic;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine;

using CruFramework.UI;
using UnityEngine.UI;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Pjfb.Training
{

    public class TrainingActionCardView : MonoBehaviour
    {
        
        private  static readonly string SelectedAnimation = "Selected";
        private  static readonly string DeselectedAnimation = "Deselected";
        

        
        [SerializeField]
        private Animator animator = null;
        
        [SerializeField]
        private PracticeCardView cardView = null;
        /// <summary>カード</summary>
        public PracticeCardView CardView{get{return cardView;}}
        
        [SerializeField]
        private GameObject specialCardEffect = null;
        
        [SerializeField]
        private GameObject specialBaloon = null;
        
        [SerializeField]
        private GameObject lvRoot = null;
        [SerializeField]
        private GameObject lvMaxRoot = null;
        [SerializeField]
        private  GameObject lvTextRoot = null;
        [SerializeField]
        private TMP_Text lvText = null;
        
        
        [SerializeField]
        private GameObject expRoot = null;
        [SerializeField]
        private Slider expSlider = null;
        [SerializeField]
        private Slider nextExpSlider = null;
        
        [SerializeField]
        private TrainingInspirationCardEffect inspirationEffect = null;
       
        [SerializeField]
        private TrainingActionCardCharaView[] joinCharacterIcons = null;
        
        [SerializeField]
        private TrainingInspirationIconCrossfade inspirationIcons = null;

        [SerializeField] 
        private TrainingCardComboEffect cardComboEffect;
        
        private int index = 0;
        /// <summary>カード位置</summary>
        public int Index{get{return index;}}
        
        private TrainingCard card = null;
        public TrainingCard Card => card;
        
        // 選択時コールバック
        private Action<TrainingActionCardView> onSelected = null;
        
        private bool isSelected = false;
        
        /// <summary>カード情報のセット</summary>
        public void SetCard(int index, TrainingCard[] cards, long trainingCharacterId, long[] joinSupportCharacters, long[] inspirationIds, Action<TrainingActionCardView> onSelected)
        {
            this.index = index;
            this.card = cards[index];
            this.onSelected = onSelected;
            
            isSelected = false;
            
            cardView.SetCard(card.mTrainingCardId,
                card.mTrainingCardCharaId,
                card.mCharaId,
                -1,
                PracticeCardView.DisplayEnhanceUIFlags.DetailLabel);
            
            bool isSpecial = false;
            
            
            // 参加キャラの表示
            for(int i=0;i<joinCharacterIcons.Length;i++)
            {
                if(i >= joinSupportCharacters.Length)
                {
                    joinCharacterIcons[i].gameObject.SetActive(false);
                }
                else
                {
                    joinCharacterIcons[i].gameObject.SetActive(true);
                    joinCharacterIcons[i].SetCharaId(joinSupportCharacters[i]);
                    joinCharacterIcons[i].SetCharacterType(joinSupportCharacters[i] == trainingCharacterId);
                }
            }
            
            // スペシャルカード演出
            if(card.cardGroupType == (long)TrainingCardGroup.Special)
            {
                // 参加キャラをチェック
                foreach(long id in joinSupportCharacters)
                {
                    if(this.card.mCharaId == id)
                    {
                        isSpecial = true;
                        break;
                    }
                }                
            }
            
            // インスピレーションエフェクト
            long inspirationEffectNumber = 0;
            long inspirationEffectPriority = int.MinValue;
            
            // インスピレーションアイコン表示
            inspirationIcons.SetIds(inspirationIds);
            // 参加キャラの表示
            for(int i=0;i<inspirationIds.Length;i++)
            {
                TrainingCardInspireMasterObject mCard = MasterManager.Instance.trainingCardInspireMaster.FindData(inspirationIds[i]);
                
                // エフェクト
                if(mCard.grade > inspirationEffectPriority)
                {
                    inspirationEffectPriority = mCard.grade;
                    inspirationEffectNumber = mCard.effectNumber;
                }
            }
            
            // バルーンの表示
            specialBaloon.SetActive(isSpecial);
            // スペシャルレクチャーエフェクト
            specialCardEffect.SetActive(card.cardGroupType == (long)TrainingCardGroup.Special);

            // インスピレーションエフェクトを再生
            if(inspirationEffectNumber > 0)
            {
                PlayInspirationEffect(inspirationEffectNumber, isSpecial);
            }
            // 停止
            else
            {
                // インスピレーションエフェクトを停止
                StopInspirationEffect();
            }
        }
        
        /// <summary>インスピレーションエフェクト再生</summary>
        public void PlayInspirationEffect(long effectNumber, bool isSpecial)
        {
            inspirationEffect.PlayEffect(effectNumber, isSpecial);
        }
        
        /// <summary>インスピレーションエフェク停止生</summary>
        public void StopInspirationEffect()
        {
            inspirationEffect.PlayCloseEffect();
        }

        //// <summary> カードコンボエフェクト再生 </summary>
        public void PlayCardComboEffect(Color color, bool isHighlight, bool isKeep)
        {
            cardComboEffect.SetHighlightColor(color);
            cardComboEffect.SetComboLineEffectColor(color);
            cardComboEffect.PlayCardComboFrameEffect(isHighlight, isKeep);
        }

        //// <summary> カードコンボエフェクトを止める </summary>
        public async UniTask StopCardComboEffectAsync(CancellationToken token, bool immediate)
        {
            // 終了ステートに移行し完全に終わるまで待機
            await cardComboEffect.CloseCardComboFrameEffectAsync(token, immediate);
        }
        
        //// <summary> カードコンボの連結エフェクト調整 </summary>
        public void AdjustCardComboConnectLineEffect(float distance, Quaternion rotation)
        {
            // 連結エフェクトをカード同士がつながるように調整
            cardComboEffect.AdjustCardComboConnectLine(distance, rotation);
        }

        //// <summary> カードコンボの連結エフェクト再生 </summary>
        public void PlayCardComboConnectLineEffect()
        {
            cardComboEffect.PlayCardComboConnectLineEffect();
        }
        
        //// <summary> カードコンボエフェクトを止める </summary>
        public async UniTask StopCardComboConnectLineEffectAsync(CancellationToken token, bool immediate)
        {
            // 終了ステートに移行し完全に終わるまで待機
            await cardComboEffect.CloseCardComboConnectLineEffectAsync(token, immediate);
        }
        
        //// <summary> カードコンボの連結エフェクトの接続基準点を取得 </summary>
        public RectTransform GetCardComboConnectRoot()
        {
            return cardComboEffect.LineEffectTop;
        }

        //// <summary> カードコンボの連結用球体オブジェクトのアクティブ切り替え </summary>
        public void SetActiveCardComboSphere(bool isActive)
        {
            cardComboEffect.SetActiveComboSphere(isActive);
        }
        
        /// <summary>参加キャラに出ているエフェクトを非表示に</summary>
        public void DisableJoinCharacterEffect()
        {
            for(int i=0;i<joinCharacterIcons.Length;i++)
            {
                joinCharacterIcons[i].SetCharacterType(false);
            }
        }
        
        /// <summary>Lv表示</summary>
        public void SetCardLv(long lv, bool isLvMax)
        {
            if(lv > 0)
            {
                lvRoot.SetActive(true);
                expRoot.SetActive(true);
                // 最大レベル
                lvMaxRoot.gameObject.SetActive(isLvMax);
                // 最大レベル未満
                lvTextRoot.gameObject.SetActive(isLvMax == false);
                // レベル表示
                lvText.text = string.Format( StringValueAssetLoader.Instance["training.card_lv"], lv );
            }
            else
            {
                lvRoot.SetActive(false);
                expRoot.SetActive(false);
            }
        }
        
        /// <summary>Lv表示</summary>
        public void SetExpProgress(float value, float next)
        {
            expSlider.value = value;
            nextExpSlider.value = next;
        }
        
        public void Deslect()
        {
            isSelected = false;
            animator.SetTrigger(DeselectedAnimation);
        }
        
        public void Select()
        {
            isSelected = true;
            animator.SetTrigger(SelectedAnimation);
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnSelected()
        {
            onSelected?.Invoke(this);
        }
        
        public void OnLontapSelected()
        {
            if(isSelected)return;
            onSelected?.Invoke(this);
        }
        
        private void Start()
        {
            // チュートリアル時の表示調整
            if(AppManager.Instance.UIManager.PageManager.CurrentPageType == PageType.TutorialTraining)
            {
                foreach(TrainingActionCardCharaView joinChar in joinCharacterIcons)
                {
                    // アニメーターで非表示になってしまうので初期化
                    joinChar.SetCharacterType(false);
                    // エフェクト表示用のキャンバスがチュートリアルより奥にあるので削除（チュートリアルではエフェクトは出ないので不要
                    GameObject.Destroy(joinChar.EffectCanvas);
                }
            }
        }
    }
}
