using UnityEngine;

namespace Pjfb.InGame
{
    public class BattleUIMediator
    {
        public BattleLogMessageScroll LogMessageScroller;
        public NewInGameMatchUpUi MatchUpUi;
        public InGameActivateAbilityUI ActivateAbilityUI;
        public NewInGameDialogueUI DialogueUi;
        public InGameFieldRadarUI RadarUI;
        public NewInGameHeaderUI HeaderUI;
        public NewInGameFooterUI FooterUI;
        public GameObject BlackOverLay;

        public static BattleUIMediator Instance { get; private set; }

        public BattleUIMediator()
        {
            Instance = this;
        }

        /// <summary>
        /// 画面下部の内容群の表示
        /// </summary>
        public void SetVisibleLowerElements(bool isVisible)
        {
            LogMessageScroller.SetVisible(isVisible);
            FooterUI.SetVisible(isVisible);
            RadarUI.SetActive(isVisible);
        }

        public void SetActiveAllElements()
        {
            LogMessageScroller.SetVisible(true);
            MatchUpUi.gameObject.SetActive(true);
            ActivateAbilityUI.gameObject.SetActive(true);
            DialogueUi.gameObject.SetActive(true);
            RadarUI.gameObject.SetActive(true);
            HeaderUI.gameObject.SetActive(true);
            FooterUI.gameObject.SetActive(true);
        }

        public void SetNonActiveAllElements()
        {
            AppManager.Instance.UIManager.System.TouchGuard.Show();
            AppManager.Instance.UIManager.System.Loading.Show();
            
            LogMessageScroller.SetVisible(false);
            MatchUpUi.gameObject.SetActive(false);
            ActivateAbilityUI.gameObject.SetActive(false);
            DialogueUi.gameObject.SetActive(false);
            RadarUI.gameObject.SetActive(false);
            HeaderUI.gameObject.SetActive(false);
            FooterUI.gameObject.SetActive(false);
            BlackOverLay.gameObject.SetActive(false);
        }
    }
}