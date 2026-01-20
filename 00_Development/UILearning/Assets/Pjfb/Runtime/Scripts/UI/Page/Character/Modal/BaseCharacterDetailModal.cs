using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using Pjfb.Encyclopedia;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace Pjfb.Character
{
    public class BaseCharacterDetailModal : CharacterDetailModal
    {
        [SerializeField] private BaseCharacterNameView nameView = null;
        [SerializeField] private CharacterStatusValuesView statusValuesView = null;
        [SerializeField] private CharacterGrowthRateGroupView growthRateGroupView;
        [SerializeField] private CharacterLiberationButton liberationButton = null; 
        [SerializeField] private UIButton growthButton = null;
        [SerializeField] private UIButton characterEncyclopediaButton = null;


        [SerializeField] private GameObject statusRoot;
        [SerializeField] private GameObject trainingEventTab;
        [SerializeField] private GameObject supportEventTab;
   

        protected override string defaultTitleStringKey => "character.base_chara_detail.title";
   
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            return base.OnPreOpen(args, token);
        }

        protected override void Init()
        {
            base.Init();
            nameView.InitializeUI(objectDetail);
            statusValuesView.SetCharacter(objectDetail.MCharaId, objectDetail.Lv, objectDetail.LiberationLevel);
            growthRateGroupView.SetCharacter(objectDetail.MCharaId, objectDetail.Lv);
            
            // 解放ボタンのViewセット
            bool isActiveLiberationUi = liberationButton.SetView(objectDetail.MChara, modalParams.CanLiberation, SetCloseParameter);
            // 強化ボタンを表示するか(解放ボタンを表示するなら強化ボタンは表示しない)
            bool showGrowthButton = modalParams.CanGrowth && isActiveLiberationUi == false && UserDataManager.Instance.chara.Find(objectDetail.UCharId) != null;
            
            // トレーニング中は表示しない
            if(AppManager.Instance.UIManager.PageManager.CurrentPageType == PageType.Training)
            {
                // 図鑑
                characterEncyclopediaButton.gameObject.SetActive(false);
                // 強化ボタンの表示
                growthButton.gameObject.SetActive(false);
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

            // トレーニングシナリオの指定があるか
            bool hasTrainingScenario = modalParams.TrainingScenarioId >= 0;
            // 育成イベントタブのアクティブ
            trainingEventTab.SetActive(hasTrainingScenario == false);
            // サポートイベントタブのアクティブ
            supportEventTab.SetActive(hasTrainingScenario);
            // ステータス表示(シナリオの指定がある場合は非表示)
            statusRoot.SetActive(hasTrainingScenario == false);
          
        }


        /// <summary>
        /// UGUI
        /// </summary>
        public void OnClickGrowthButton()
        {
            // 編成中キャラから自キャラのリストを取得
            List<long> uCharIdList = new List<long>();
            foreach (CharacterDetailData charaData  in modalParams.SwipeableParams.DetailOrderList)
            {
                if(UserDataManager.Instance.chara.Contains(charaData.UCharId) == false) continue;
                uCharIdList.Add(charaData.UCharId);
            }
            CharaLevelUpBasePage.Data args = new BaseCharaGrowthLiberationPage.Data(objectDetail.UCharId, uCharIdList.IndexOf(objectDetail.UCharId), uCharIdList, null);
            
            // すべてのモーダルを閉じる
            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop((m)=>true);
            // 閉じる
            Close();
            // 強化画面を開く
            AppManager.Instance.UIManager.PageManager.OpenPage(
                PageType.Character,
                true,
                new CharacterPage.Data(CharacterPageType.BaseCharaGrowthLiberation, args)
            );
        }

        public void OnClickLibraryButton()
        {
            // Safe check
            // Todo : delete
            var charaParent =  MasterManager.Instance.charaParentMaster.values.FirstOrDefault(x =>
                x.parentMCharaId == objectDetail.MChara.parentMCharaId);
            if (charaParent is null)
            {
                CruFramework.Logger.LogError("MCharaParentのデータありません");
                return;
            }
            SetCloseParameter(null);
            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => true);
            Close(() => EncyclopediaPage.OpenPage(true, objectDetail.MChara.parentMCharaId));
        }

        public void OnClickCloseButton()
        {
            SetCloseParameter(null);
            Close();
        }
    }
}


