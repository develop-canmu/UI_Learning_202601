using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.UserData;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.InGame
{
    public class NewInGameStartPage : Page
    {
        public class Param
        {
            public PageType OpenPage;

            public BigValue FixedEnemyCombatPower;
            public Param(PageType openPage, BigValue fixedEnemyCombatPower)
            {
                OpenPage = openPage;
                FixedEnemyCombatPower = fixedEnemyCombatPower;
            }
        }
        
        [SerializeField] private Animator animator;
        [SerializeField] private UserIcon playerUserIcon;
        [SerializeField] private UserIcon enemyUserIcon;
        [SerializeField] private CharacterIcon enemyCharacterIcon; // player, enemyでIFが違うが, 敵側の情報を定義する術がないのでエースキャラクターのアイコンを表示する.
        [SerializeField] private TextMeshProUGUI[] userNames;
        [SerializeField] private UserTitleImage userTitleImage;
        [SerializeField] private DeckRankImage[] userRankImages;
        [SerializeField] private TextMeshProUGUI[] userRankTexts;
        [SerializeField] private TextMeshProUGUI[] plateName;
        [SerializeField] private TextMeshProUGUI[] kickoffPlate;
        [SerializeField] private OmissionTextSetter[] omissionTextSetters;

        private const string OpenKey = "Open";
        private const string CloseKey = "Close";

        private Param args = null;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            this.args = (Param)args;
            SetPlayerView();
            return base.OnPreOpen(args, token);
        }

        protected override void OnOpened(object args)
        {
            animator.SetTrigger(OpenKey);
            base.OnOpened(args);
        }

        private void SetPlayerView()
        {
            var player = BattleDataMediator.Instance.Players[(int)BattleDataMediator.Instance.PlayerSide];
            var enemy = BattleDataMediator.Instance.Players[(int)BattleDataMediator.Instance.EnemySide];
            var playerDeck = BattleDataMediator.Instance.Decks[(int)BattleDataMediator.Instance.PlayerSide];
            var enemyDeck = BattleDataMediator.Instance.Decks[(int)BattleDataMediator.Instance.EnemySide];
            var enemyAceCharacter = enemyDeck.FirstOrDefault(character => character.IsAceCharacter);
            var playerTotalPower = playerDeck.Sum(chara => chara.CombatPower );
            var enemyTotalPower = args.FixedEnemyCombatPower > 0 ? args.FixedEnemyCombatPower : enemyDeck.Sum(chara => chara.CombatPower);
            var playerRank = StatusUtility.GetPartyRank(playerTotalPower);
            var enemyRank = StatusUtility.GetPartyRank(enemyTotalPower);

            userNames[0].text = player.Name;
            userNames[1].text = enemy.Name;

            userRankImages[0].SetTextureAsync(playerRank).Forget();
            userRankImages[1].SetTextureAsync(enemyRank).Forget();
            userRankTexts[0].text = playerTotalPower.ToDisplayString(omissionTextSetters[0].GetOmissionData());
            userRankTexts[1].text = enemyTotalPower.ToDisplayString(omissionTextSetters[1].GetOmissionData());
            
            // リーグマッチのリプレイ表示だった場合、先行プレイヤーのプレートにKICKOFFを表示し、後攻は何も表示しない
            bool leagueMatchFlg = BattleDataMediator.Instance.BattleType == BattleConst.BattleType.ReplayLeagueMatch;
            plateName[0].gameObject.SetActive(!leagueMatchFlg);
            plateName[1].gameObject.SetActive(!leagueMatchFlg);
            int displayKickoffPlateNum = (long)BattleDataMediator.Instance.PlayerSide == 
                                         BattleDataMediator.Instance.OriginalBattleV2ClientData.firstAttackIndex ? 0 : 1;
            kickoffPlate[displayKickoffPlateNum].gameObject.SetActive(leagueMatchFlg);

            playerUserIcon.SetIconIdAsync(BattleDataMediator.Instance.Players[(int)BattleDataMediator.Instance.PlayerSide].IconId).Forget();
            var enemyUserIconId = BattleDataMediator.Instance.Players[(int)BattleDataMediator.Instance.EnemySide].IconId;
            enemyUserIcon.gameObject.SetActive(false);
            enemyCharacterIcon.gameObject.SetActive(false);
            if (enemyUserIconId > 0)
            {
                enemyUserIcon.gameObject.SetActive(true);
                enemyUserIcon.SetIconIdAsync(enemyUserIconId).Forget();
            }
            else
            {
                var enemyAceCharacterId = enemyAceCharacter.MCharaId;
                enemyCharacterIcon.gameObject.SetActive(true);
                enemyCharacterIcon.SetTextureAsync(enemyAceCharacterId).Forget();
            }
        }

        public void OnTapStart()
        {
            animator.SetTrigger(CloseKey);
            BattleEventDispatcher.Instance.OnBattleStartCallback();
        }
    }
}