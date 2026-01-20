using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Master;

namespace Pjfb
{
    public class RankingTotalPowerDeckView : MonoBehaviour
    {
        /// <summary>デック情報を表示するために必要なパラメータ</summary>
        public class CharacterData
        {
            /// <summary>キャラのアイコンID</summary>
            public long MCharaId { get; }

            /// <summary>キャラの戦力</summary>
            public long CombatPower { get; }

            /// <summary>キャラの戦力ランク</summary>
            public long CharaRank { get; }

            /// <summary>ポジションのID</summary>
            public long RoleNumber { get; }

            public CharacterData(long mCharaId, long combatPower, long charaRank, long roleNumber)
            {
                MCharaId = mCharaId;
                CombatPower = combatPower;
                CharaRank = charaRank;
                RoleNumber = roleNumber;
            }
        }
        
        /// <summary>チームボタンクリック時にアクティブを切り替えるDeckのRoot</summary>
        [SerializeField]
        private GameObject deckRoot = null;
        
        /// <summary>Deckで表示するアイコンの配列</summary>
        [SerializeField] 
        private CharacterVariableIcon[] characterVariableIcons = null;
        
        /// <summary>チームボタンクリック時に呼ばれるコールバック</summary>
        public event Action<bool> OnClickDeckEvent = null;
        
        /// <summary>デッキ表示をするボタンのtoggle状態を取得するための参照</summary>
        [SerializeField]
        private UIToggle deckToggleButton = null;
        
        /// <summary>トグルボタンクリック時にdeckのアクティブ状態を切り替える</summary>
        public void OnClickDeck(bool isDeckVisible)
        {
            ShowDeck(isDeckVisible);
            OnClickDeckEvent?.Invoke(isDeckVisible);
        }

        /// <summary>外部クラスから呼び出すデッキ部分の表示切替</summary>
        public void ShowDeck(bool isDeckVisible)
        {
            deckRoot.SetActive(isDeckVisible);
            // トグルボタンの状態を更新する。uGUI以外から呼ばれた場合に必要
            deckToggleButton.SetIsOnWithoutNotify(isDeckVisible);
        }

        /// <summary>チームボタンクリック時に表示するDeckの情報をセット</summary>
        public void SetView(CharacterData[] deckInfo)
        {
            for (int i = 0; i < deckInfo.Length; i++)
            {
                CharacterData deckData = deckInfo[i];
                
                // 設定するアイコンを取得
                CharacterVariableIcon charaIcon = characterVariableIcons[i];
                
                // キャラクターアイコンをセット
                charaIcon.SetIconTextureWithEffectAsync(deckData.MCharaId).Forget();
                
                // キャラクターの戦力、ランク、ポジションをセット
                charaIcon.SetIcon(new BigValue(deckData.CombatPower), deckData.CharaRank, (RoleNumber)deckData.RoleNumber);
            }
        }
    }
}