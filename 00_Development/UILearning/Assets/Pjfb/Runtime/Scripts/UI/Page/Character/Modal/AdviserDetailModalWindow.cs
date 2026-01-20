using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Encyclopedia;
using Pjfb.UserData;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Character
{
    public class AdviserDetailModalWindow : CharacterDetailModalBase
    {
        [SerializeField]
        private AdviserDetailTabSheetManager sheetManager = null;

        [SerializeField]
        private CharacterLiberationButton liberationButton = null;
        [SerializeField]
        private UIButton growthButton = null;
        
        // 選手名鑑ボタン
        [SerializeField] 
        private UIButton characterEncyclopediaButton = null;
        
        [SerializeField]
        private AdviserNameView nameView = null;

        // エールスキル表示
        [SerializeField]
        private AdviserSkillListView yellSkillList = null;

        // サポートスキル表示
        [SerializeField]
        private AdviserSkillListView supportSkillList = null;
        
        // それぞれのシート毎に初期化されたかのフラグ
        private Dictionary<AdviserDetailTabSheetType, bool> isInitializeListOnSheet = new Dictionary<AdviserDetailTabSheetType, bool>();

        /// <summary> モーダルタイトル </summary>
        protected override string defaultTitleStringKey => "character.detail_modal.adviser.title";
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            // リストのクリア
            isInitializeListOnSheet.Clear();
            // 初期化状況のセット
            foreach (AdviserDetailTabSheetType sheetType in Enum.GetValues(typeof(AdviserDetailTabSheetType)))
            {
                isInitializeListOnSheet.Add(sheetType, false);
            }
            
            // シートを開いた際の処理を登録
            sheetManager.OnOpenSheet -= OnOpenSheet;
            sheetManager.OnOpenSheet += OnOpenSheet;
            
            await base.OnPreOpen(args, token);
            
            // 初期シートを開く(データのセットが継承元のOnPreOpenで行われるので最後に開く処理をする)
            await sheetManager.OpenSheetAsync(sheetManager.FirstSheet, null);
        }

        protected override void Init()
        {
            base.Init();
            
            // 名前表示部分
            nameView.InitializeUI(objectDetail);
            
            // 解放ボタンのViewセット
            bool isActiveLiberationUi = liberationButton.SetView(objectDetail.MChara, modalParams.CanLiberation, SetCloseParameter);
            // 強化ボタンを表示するか(解放ボタンを表示するなら強化ボタンは表示しない)
            bool showGrowthButton = modalParams.CanGrowth && isActiveLiberationUi == false && UserDataManager.Instance.chara.Find(objectDetail.UCharId) != null;

            // トレーニング中は表示しない
            if (AppManager.Instance.UIManager.PageManager.CurrentPageType == PageType.Training)
            {
                // 強化ボタンの表示
                growthButton.gameObject.SetActive(false);
                characterEncyclopediaButton.gameObject.SetActive(false);
            }
            else
            {
                
                // 選手名鑑の表示
                characterEncyclopediaButton.gameObject.SetActive(modalParams.CanOpenCharacterEncyclopediaPage && CharacterEncyclopediaUtility.ShowEncyclopediaButton(objectDetail.MCharaId));
                // 強化ボタンの表示
                growthButton.gameObject.SetActive(showGrowthButton);
                // 最大強化の場合はグレーアウト
                growthButton.interactable = CharacterUtility.IsMaxGrowthLevel(objectDetail.MCharaId, objectDetail.Lv, objectDetail.LiberationLevel) == false;

            }
        }

        /// <summary> 次の詳細に移動時の処理 </summary>
        public override void NextDetail()
        {
            base.NextDetail();
            ResetInitializeOnSheet();
            // スワイプ時に現在開いているシートを更新する
            OnOpenSheet(sheetManager.CurrentSheetType);
        }

        /// <summary> 前の詳細に移動時の処理 </summary>
        public override void PrevDetail()
        {
            base.PrevDetail();
            ResetInitializeOnSheet();
            // スワイプ時に現在開いているシートを更新する
            OnOpenSheet(sheetManager.CurrentSheetType);
        }

        /// <summary> シートタブを開いた際の処理 </summary>
        private void OnOpenSheet(AdviserDetailTabSheetType sheetType)
        {
            // すでに初期化済みならViewのセット処理は行わない
            if (isInitializeListOnSheet[sheetType])
            {
                return;
            }
            
            // 自動レイアウトを使用しているのでContentのサイズを再計算してからアイテムをセットする
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            
            switch (sheetType)
            {
                // 練習能力をセット
                case AdviserDetailTabSheetType.TrainingPracticeAbility:
                {
                    SetPracticeAbilityList();
                    break;
                }
                // トレーニングカードをセット
                case AdviserDetailTabSheetType.TrainingCard:
                {
                    SetTrainingPracticeCardList();
                    break;
                }
                // エールスキルのセット
                case AdviserDetailTabSheetType.YellSkill:
                {
                    yellSkillList.SetView(BattleConst.AbilityType.GuildBattleManual, objectDetail.MCharaId, objectDetail.Lv, objectDetail.LiberationLevel);
                    break;
                }
                // サポートスキルのセット
                case AdviserDetailTabSheetType.SupportSkill:
                {
                    supportSkillList.SetView(BattleConst.AbilityType.GuildBattleAuto, objectDetail.MCharaId, objectDetail.Lv, objectDetail.LiberationLevel);
                    break;
                }
                // トレーニングイベントのセット
                case AdviserDetailTabSheetType.TrainingEvent:
                {
                    SetTrainingEventList();
                    break;
                }
            }
            // 初期化済みにセット
            isInitializeListOnSheet[sheetType] = true;
        }

        /// <summary> シート毎の初期化状況を一括でリセット </summary>
        private void ResetInitializeOnSheet()
        {
            foreach (AdviserDetailTabSheetType sheetType in Enum.GetValues(typeof(AdviserDetailTabSheetType)))
            {
                isInitializeListOnSheet[sheetType] = false;
            }
        }

        /// <summary> 強化ボタン </summary>
        public void OnClickGrowthButton()
        {
            // 所持しているユーザキャラIdリスト
            List<long> uCharaIdList = new List<long>();
            
            foreach (CharacterDetailData charaData in modalParams.SwipeableParams.DetailOrderList)
            {
                // 持っているキャラのみに絞る
                if (UserDataManager.Instance.chara.Contains(charaData.UCharId) == false)
                {
                    continue;
                }
                uCharaIdList.Add(charaData.UCharId);
            }

            CharaLevelUpBasePage.Data args = new AdviserGrowthLiberationPage.Data(objectDetail.UCharId, uCharaIdList.IndexOf(objectDetail.UCharId), uCharaIdList, null);
            // すべてのモーダルを閉じる
            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop((m)=>true);
            // 閉じる
            Close();
            // アドバイザー強化画面を開く
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Character, true, new CharacterPage.Data(CharacterPageType.AdviserGrowthLiberation, args));
        }

        /// <summary> 選手名鑑遷移ボタン </summary>
        public void OpenEncyclopedia()
        {
            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => true);
            
            Close(onCompleted:()=>
            {
                EncyclopediaPage.OpenPage(true, MChara.parentMCharaId);
            });
        }
    }
}