using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Menu;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.Ranking
{
    public class RankingUserInfoView : RankingInfoView
    {
        [SerializeField]
        private CharacterVariableIcon characterIcon = null;
        
        [SerializeField]
        private TMP_Text characterNameText = null;
        
        [SerializeField]
        private TMP_Text scenarioNameText = null;
        
        [SerializeField]
        private GameObject emptyTextObj = null;
        
        [SerializeField]
        private TextMeshProUGUI emptyText = null;
        
        [SerializeField]
        private GameObject emptyIconRoot = null;
        
        [SerializeField]
        private GameObject infoRoot = null;
        
        [SerializeField]
        private GameObject buttonProfile = null;
        
        [SerializeField]
        private Image baseRankingMask = null;
        
        private long userId = 0;
        
        [Header("総戦力ランキングのUI表示だけで使用")]
        [SerializeField]
        private GameObject totalPowerRoot = null;
        
        /// <summary>ユーザーアイコン</summary>
        [SerializeField]
        private UserIcon userIconImage = null;

        /// <summary>総戦力の値</summary>
        [SerializeField]
        private TextMeshProUGUI totalPowerText = null;
        
        /// <summary>総戦力の省略表記用データ</summary>
        [SerializeField]
        private OmissionTextSetter totalPowerOmissionTextSetter = null;
        
        /// <summary>デッキランクアイコン</summary>
        [SerializeField]
        private DeckRankImage deckRankImage = null;

        /// <summary>チームボタンクリック時に表示するデッキ</summary>
        [SerializeField]
        private RankingTotalPowerDeckView deckView = null;

        public RankingTotalPowerDeckView DeckView => deckView;
        
        [Header("選手戦力ランキング固有の表示内容")]
        [SerializeField]
        private GameObject characterPowerRoot = null;
        
        /// <summary>選手戦力における育成済みキャラがいない表示の切り替え</summary>
        public void ShowEmptyCharacterPower(bool isEmpty)
        {
            // Emptyのメッセージの表示切り替え
            ShowEmptyText(isEmpty);
            
            // ランキング情報
            ShowInfoRoot(!isEmpty);
            
            // アイコンのEmpty表示
            emptyIconRoot.SetActive(isEmpty);
            characterIcon.gameObject.SetActive(!isEmpty);
            
            if(isEmpty)
            {
                // 育成済みキャラがいない場合は、現在の順位をブランク文字列にする
                pointText.text = StringValueAssetLoader.Instance["ranking.rank_blank"];
                
                // 育成済みキャラがいない場合のテキストをセット
                emptyText.text = StringValueAssetLoader.Instance["ranking.no_trained_character"];
            }
        }

        /// <summary>総戦力情報がない表示の切り替え</summary>
        public void ShowEmptyTotalPower(bool isEmpty)
        {
            // 総戦力情報がない場合の表示切替
            ShowEmptyText(isEmpty);
            
            // 総戦力におけるユーザーアイコン以外のヘッダー要素のRootの表示切り替え
            ShowInfoRoot(!isEmpty);

            if (isEmpty)
            {
                // 総戦力情報がない場合は、現在の順位をブランク文字列にする
                totalPowerText.text = StringValueAssetLoader.Instance["ranking.rank_blank"];
                
                // 総戦力情報がない場合のテキストをセット
                emptyText.text = StringValueAssetLoader.Instance["ranking.no_total_power"];
            }
        }
        
        /// <summary>ユーザーId</summary>
        public void SetUserId(long userId)
        {
            this.userId = userId;
        }
        
        /// <summary>キャラ名</summary>
        public void SetCharacterName(string name)
        {
            characterNameText.text = name;
        }
        
        public void SetCombatPower(BigValue combatPower)
        {
            pointText.text = combatPower.ToDisplayString(omissionTextSetter.GetOmissionData());
        }

        /// <summary>キャラIdで情報をセット</summary>
        public void SetCharacter(long mCharId, BigValue combatPower, long rank)
        {
            CharaMasterObject mChar = MasterManager.Instance.charaMaster.FindData(mCharId);
            // キャラ名
            SetCharacterName(mChar.nickname + mChar.name);
            // 戦力表示
            SetCombatPower(combatPower);
            // アイコン
            characterIcon.SetIcon( combatPower, rank);
            // 戦力を非表示
            characterIcon.SetActiveCombatPower(false);
            // アイコン画像
            characterIcon.SetIconTextureWithEffectAsync(mCharId).Forget();

        }
        
        /// <summary>チーム総戦力のセット</summary>
        public void SetTotalPower(BigValue totalPower, long deckRankIconId)
        {
            // 総戦力値のセット
            totalPowerText.text = totalPower.ToDisplayString(totalPowerOmissionTextSetter.GetOmissionData());
            
            // デッキランクアイコンのセット
            deckRankImage.SetTexture(deckRankIconId);
        }
        
        /// <summary>選手戦力の個人ランキングのヘッダーをセット</summary>
        public void ShowCharacterPowerUserInfoHeaderView()
        {
            ShowInfoRoot(true);
            ShowCharacterIcon(true);
            ShowCharacterPowerRoot(true);
            
            // 総戦力のヘッダーUIは非表示
            ShowUserIconImage(false);
            ShowTotalPowerRoot(false);
        }

        /// <summary>総戦力個人ランキングのヘッダーをセット</summary>
        public void ShowTotalPowerUserInfoHeaderView()
        {
            ShowInfoRoot(true);
            ShowUserIconImage(true);
            ShowTotalPowerRoot(true);
            
            // 選手戦力関係のヘッダーUIは非表示
            ShowCharacterPowerRoot(false);
            ShowCharacterIcon(false);
            ShowEmptyCharacterIconRoot(false);
        }

        /// <summary>
        /// ユーザー情報表示ヘッダー（総戦力、選手戦力）内のキャラもしくはユーザーアイコン以外のUI要素のRootの表示
        /// </summary>
        private void ShowInfoRoot(bool isVisible)
        {
            infoRoot.SetActive(isVisible);
        }

        /// <summary> 総戦力におけるユーザー情報表示ヘッダー内のユーザーアイコンの表示</summary>
        private void ShowUserIconImage(bool isVisible)
        {
            userIconImage.gameObject.SetActive(isVisible);
        }

        /// <summary>選手戦力におけるユーザー情報表示ヘッダー内のキャラアイコンの表示</summary>
        private void ShowCharacterIcon(bool isVisible)
        {
            characterIcon.gameObject.SetActive(isVisible);
        }

        /// <summary>ユーザー情報表示ヘッダー内のユーザーアイコン以外のUI要素の表示</summary>
        private void ShowTotalPowerRoot(bool isVisible)
        {
            totalPowerRoot.SetActive(isVisible);
        }

        /// <summary>ユーザー情報表示ヘッダー内のキャラアイコン以外のUI要素の表示</summary>
        private void ShowCharacterPowerRoot(bool isVisible)
        {
            characterPowerRoot.SetActive(isVisible);
        }

        /// <summary>選手戦力で育成対象がいない場合のキャラアイコンの表示</summary>
        private void ShowEmptyCharacterIconRoot(bool isVisible)
        {
            emptyIconRoot.SetActive(isVisible);
        }

        private void ShowEmptyText(bool isVisible)
        {
            emptyTextObj.SetActive(isVisible);
        }
        
        /// <summary>ユーザーアイコンのセット</summary>
        public void SetUserIcon(long iconId)
        {
            userIconImage.SetIconIdAsync(iconId).Forget();
        }

        /// <summary>ランキング表示</summary>
        public override void SetRank(long rank, bool isCurrent)
        {
            base.SetRank(rank, isCurrent);
            if (baseRankingMask != null)
            {
                baseRankingMask.color = ColorValueAssetLoader.Instance[isCurrent ? "ranking.my.rank" : "ranking.rank"];
            }
        }

        /// <summary>ユーザー情報表示ヘッダー内の順位をセットする</summary>
        public void SetHeaderRank(long rank)
        {
            if (rank == 0)
            {
                // 順位が0の場合はブランク文字列を表示
                rankText.gameObject.SetActive(true);
                rankText.text = StringValueAssetLoader.Instance["ranking.rank_blank"];
                return;
            }
            
            rankText.text = rank.GetStringNumberWithComma();
        }
        
        
        /// <summary>シナリオ</summary>
        public void SetScenario(long mScenarioId)
        {
            TrainingScenarioMasterObject mScenario = MasterManager.Instance.trainingScenarioMaster.FindData(mScenarioId);

            // デバッグで作ったキャラとかは取得できない
            // 本番環境では多分発生しないけど一応処理しておく
            if(mScenario == null)
            {
                scenarioNameText.text = "-";
                return;
            }
            // 名前
            scenarioNameText.text = mScenario.name;
        }
        
        /// <summary>
        /// プロフィールボタンの表示切り替え
        /// </summary>
        /// <param name="isActive"></param>
        public void SetButtonProfileActive(bool isActive)
        {
            buttonProfile.SetActive(isActive);
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnProfileButton()
        {
            TrainerCardModalWindow.WindowParams param = new TrainerCardModalWindow.WindowParams(userId, false, false);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TrainerCard, param);
        }
        
    }
}