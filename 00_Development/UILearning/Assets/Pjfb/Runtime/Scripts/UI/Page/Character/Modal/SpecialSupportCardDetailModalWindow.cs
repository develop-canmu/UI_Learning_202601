using UnityEngine;
using Pjfb.Master;
using System.Linq;

using System.Collections.Generic;
using Pjfb.UserData;

namespace Pjfb.Character
{
    public class SpecialSupportCardDetailModalWindow : CharacterDetailModal
    {

        [SerializeField] private SpecialSupportCardNameView nameView;
        [SerializeField] private UIButton growthButton = null;

        protected override string defaultTitleStringKey => "character.detail_modal.special_support_info";

        protected override void Init()
        {
            base.Init();
            // 強化ボタン
            var showGrowthButton = modalParams.CanGrowth && UserDataManager.Instance.chara.Find(objectDetail.UCharId) != null;
            growthButton.gameObject.SetActive(showGrowthButton);
            // 最大強化の場合はグレーアウト
            growthButton.interactable = CharacterUtility.IsMaxGrowthLevel(objectDetail.MCharaId, objectDetail.Lv, objectDetail.LiberationLevel) == false;

            nameView.InitializeUI(MChara, objectDetail.Lv, objectDetail.LiberationLevel, objectDetail.GetMaxGrowthLevel());
        }
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnClickGrowthButton()
        {
            List<long> uCharIdList = modalParams.SwipeableParams.DetailOrderList.Where((v)=>v.UCharId >= 0).Select(v=>v.UCharId).ToList();
            CharaLevelUpBasePage.Data args = new BaseCharaGrowthLiberationPage.Data(objectDetail.UCharId, uCharIdList.IndexOf(objectDetail.UCharId), uCharIdList, null);
            
            // すべてのモーダルを閉じる
            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop((m)=>true);
            // 閉じる
            Close();
            // 強化画面を開く
            AppManager.Instance.UIManager.PageManager.OpenPage(
                PageType.Character,
                true,
                new CharacterPage.Data(CharacterPageType.SpecialSupportCardLevelUp, args)
            );
        }
        
        public void OpenCharacterIllustrator()
        {
            CharacterIllustratorModalWindow.Open(
                new CharacterIllustratorModalWindow.WindowParams(MChara.id, CardType.SpecialSupportCharacter));
        }
        
        public void OnClickClose()
        {
            Close();
        }
    }
}
