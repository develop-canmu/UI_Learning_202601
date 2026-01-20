using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using MagicOnion;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameSpotUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text spotNameText;
        [SerializeField] private Animator animator;
        [SerializeField] private UIButton button;
        [SerializeField] private Sprite[] multipleSprites;
        [SerializeField] private Sprite[] allyNumberSprites;
        [SerializeField] private Sprite[] enemyNumberSprites;
        [SerializeField] private Image multipleImage;
        [SerializeField] private Image[] numberImages;

        private int lastSyncedHp = -1;
        private GuildBattleMapSpotModel spotModel;
        private const string OccupiedTrigger = "OpenBreakthrough";
        private const string DamagedTriggerOwn = "DamageOwn";
        private const string DamagedTriggerEnemy = "DamageOpponent";
        private GuildBattleCommonConst.GuildBattleTeamSide viewSide = GuildBattleCommonConst.GuildBattleTeamSide.Left;
        
        public void Initialize(GuildBattleMapSpotModel _spotModel)
        {
            spotModel = _spotModel;
            spotNameText.text = spotModel.GetSpotName();
            viewSide = spotModel.OccupyingSide == PjfbGuildBattleDataMediator.Instance.PlayerSide
                ? GuildBattleCommonConst.GuildBattleTeamSide.Left
                : GuildBattleCommonConst.GuildBattleTeamSide.Right;
            SetHpUI(spotModel.RemainHP);
        }

        public void UpdateView(GuildBattleMapSpotModel spotModel)
        {
            if (spotModel.RemainHP <= 0 && lastSyncedHp is -1 or > 0)
            {
                animator.SetTrigger(OccupiedTrigger);
                if (spotModel.OccupyingSide == PjfbGuildBattleDataMediator.Instance.PlayerSide)
                {
                    button.interactable = false;
                }
            }

            lastSyncedHp = spotModel.RemainHP;
            SetHpUI(spotModel.RemainHP);
        }

        private void SetHpUI(int hp)
        {
            multipleImage.sprite = multipleSprites[(int)viewSide];
            var sprites = viewSide == GuildBattleCommonConst.GuildBattleTeamSide.Left ? allyNumberSprites : enemyNumberSprites;

            var number100 = hp / 100;
            var number10 = (hp % 100) / 10;
            var number1 = hp % 10;
            numberImages[0].sprite = sprites[number1];
            numberImages[1].gameObject.SetActive(number10 > 0 || number100 > 0);
            numberImages[1].sprite = sprites[number10];
            numberImages[2].gameObject.SetActive(number100 > 0);
            numberImages[2].sprite = sprites[number100];
        }

        public void OnClickSpot()
        {
            if (!PjfbGuildBattleDataMediator.Instance.IsClientOperatable())
            {
                return;
            }
            
            var args = new ClubRoyalInGameSpotModal.Arguments(spotModel.Id);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubRoyalInGameSpot, args);
        }

        public Vector3 GetBallAbsorbStartPosition()
        {
            return button.transform.position;
        }

        public void PlayDamagedAnimation()
        {
            animator.SetTrigger(viewSide == GuildBattleCommonConst.GuildBattleTeamSide.Left ? DamagedTriggerOwn : DamagedTriggerEnemy);
        }
    }
}