using MagicOnion;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine;

namespace Pjfb.Community
{
    public class GActionLogItem : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI bodyText;

        public void Init(ChatInfo info)
        {
            bodyText.text = info.MessageText;
        }
        
        public void Init(GuildBattleCommonChatData chatData)
        {
            var message = string.Empty;
            PjfbGuildBattleDataMediator.Instance.PjfbBattlePlayers.TryGetValue(chatData.SystemLogUserId1, out var playerA);
            PjfbGuildBattleDataMediator.Instance.PjfbBattlePlayers.TryGetValue(chatData.SystemLogUserId2, out var playerB);
            PjfbGuildBattleDataMediator.Instance.BattleField.MapSpotsDictionary.TryGetValue((int)chatData.SystemLogSpotId, out var spot);

            switch ((GuildBattleCommonChatData.SystemLogType)chatData.LogType)
            {
                case GuildBattleCommonChatData.SystemLogType.AllyWinStreakLog:// = 1, // [Player]がn連撃破!!
                    message = StringValueAssetLoader.Instance["clubroyalingame.system_log_win_streak_ally"].Format(playerA?.Name, chatData.SystemLogWinStreakCount);
                    break;
                case GuildBattleCommonChatData.SystemLogType.EnemyWinStreakLog:// = 2, // [Player]がn連撃破!!
                    message = StringValueAssetLoader.Instance["clubroyalingame.system_log_win_streak_enemy"].Format(playerA?.Name, chatData.SystemLogWinStreakCount);
                    break;
                case GuildBattleCommonChatData.SystemLogType.AllyStopWinStreakLog:// = 3,  // [PlayerA]が[PlayerB]の連撃を阻止!
                    message = StringValueAssetLoader.Instance["clubroyalingame.system_log_block_win_streak_ally"].Format(playerA?.Name, playerB?.Name);
                    break;
                case GuildBattleCommonChatData.SystemLogType.EnemyStopWinStreakLog:// = 4, // [PlayerA]に[PlayerB]の連撃を阻止されました…
                    message = StringValueAssetLoader.Instance["clubroyalingame.system_log_block_win_streak_enemy"].Format(playerA?.Name, playerB?.Name);
                    break;
                case GuildBattleCommonChatData.SystemLogType.AllyOccupySpotLog:// = 5, // [Player]が[Spot]を制圧!!
                    message = StringValueAssetLoader.Instance["clubroyalingame.system_log_occupy_spot_ally"].Format(playerA?.Name, spot?.GetSpotName());
                    break;
                case GuildBattleCommonChatData.SystemLogType.EnemyOccupySpotLog:// = 6, // [Player]に[Spot]が制圧されました…
                    message = StringValueAssetLoader.Instance["clubroyalingame.system_log_occupy_spot_enemy"].Format(playerA?.Name, spot?.GetSpotName());
                    break;
                case GuildBattleCommonChatData.SystemLogType.PartyStartFighting:// = 7, // あなたの軍と[Player]の軍が会敵しました!
                    message = StringValueAssetLoader.Instance["clubroyalingame.system_log_matchup"].Format(playerA?.Name);
                    break;
                case GuildBattleCommonChatData.SystemLogType.AllyPartyBeatEnemy:// = 8, // [PlayerA]の軍が[PlayerB]の軍を撃破!
                    message = StringValueAssetLoader.Instance["clubroyalingame.system_log_beat_enemy"].Format(playerA?.Name, playerB?.Name);
                    break;
                case GuildBattleCommonChatData.SystemLogType.ReinforcementsArrived:// = 9, // 援軍が到着しました!
                    message = StringValueAssetLoader.Instance["clubroyalingame.system_log_add_ball_count"];
                    break;
                case GuildBattleCommonChatData.SystemLogType.OnUseItem:// = 10, // [PlayerA]が[ItemName]を使用しました！!
                    var point = MasterManager.Instance.pointMaster.FindData(chatData.SystemLogUserId2);
                    message = StringValueAssetLoader.Instance["clubroyalingame.system_log_use_item"].Format(playerA?.Name, point?.name);
                    break;
                case GuildBattleCommonChatData.SystemLogType.OnUseYellAbility:// = 11, // [PlayerA]が[AbilityName]を使用しました！
                    AbilityMasterObject ability = MasterManager.Instance.abilityMaster.FindData(chatData.SystemLogUserId2);
                    message = StringValueAssetLoader.Instance["clubroyalingame.system_log_use_yell_ability"].Format(playerA?.Name, ability?.name);
                    break;
            }

            bodyText.text = message;
        }

    }
}