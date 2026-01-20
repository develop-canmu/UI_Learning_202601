using System;
using MagicOnion;
using Pjfb.Networking.App.Request;

namespace Pjfb
{
    public class GuildBattleMapSpotModel : GuildBattleCommonMapSpotModel
    {
        public GuildBattleMapSpotModel(int id, BattleV2BattleConquestFieldSpot spotData) : base(id, (int)spotData.positionY, (int)spotData.positionX, (GuildBattleCommonConst.GuildBattleTeamSide)spotData.occupyingSide, spotData.isBase)
        {
        }

        public float GetHpRatio()
        {
            return (float)RemainHP / MaxHP;
        }

#if !MAGIC_ONION_SERVER
        public string GetSpotName()
        {
            var playerSide = PjfbGuildBattleDataMediator.Instance.PlayerSide == OccupyingSide;
            if (IsBase)
            {
                return playerSide ? StringValueAssetLoader.Instance["clubroyalingame.spot_name_A"] : StringValueAssetLoader.Instance["clubroyalingame.spot_name_Z"];
            }
            else
            {
                switch (LaneNumber)
                {
                    /*
                     BaseがLaneNumber=1になっている想定.
                    case 1:
                        return playerSide ? "ゴールB" : "ゴールX";
                        */
                    case 2:
                        return playerSide ? StringValueAssetLoader.Instance["clubroyalingame.spot_name_B"] : StringValueAssetLoader.Instance["clubroyalingame.spot_name_X"];
                    case 3:
                        return playerSide ? StringValueAssetLoader.Instance["clubroyalingame.spot_name_C"] : StringValueAssetLoader.Instance["clubroyalingame.spot_name_Y"];
                }
            }

            return string.Empty;
        }
#endif
        
        public int TakeDamage(int brokenSpotCount, int ballCount)
        {
            var damage = ballCount;
            if (IsBase && brokenSpotCount > 0)
            {
                var brokenSpotCountDamageCoef = 1;
                switch (brokenSpotCount)
                {
                    case 1:
                        brokenSpotCountDamageCoef += GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleAdditionalSpotDamageCoefficientOneSpot;
                        break;
                    case 2:
                        brokenSpotCountDamageCoef += GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleAdditionalSpotDamageCoefficientTwoSpot;
                        break;
                    default:
                        brokenSpotCountDamageCoef = 1;
                        break;
                }
                damage *= brokenSpotCountDamageCoef;
            }

            var dealtDamage = Math.Min(RemainHP, damage);
            RemainHP = Math.Max(RemainHP - damage, 0);
            
            var winningPoint = IsBase ? PjfbGuildBattleDataMediator.Instance.GuildBattleSetting.GuildBattleWinningPointOccupyBaseSpot : PjfbGuildBattleDataMediator.Instance.GuildBattleSetting.GuildBattleWinningPointOccupySpot;
            var getWinningPoint = (int)(winningPoint * ((float)damage / MaxHP));
            
            // floatによる計算誤差が発生するので, 拠点を制圧した時点で帳尻を合わせる.
            if (RemainHP <= 0)
            {
                getWinningPoint = winningPoint - AddedWinningPoint;
            }
            
            AddedWinningPoint += getWinningPoint;
            PjfbGuildBattleDataMediator.Instance.AddWinningPoint(GuildBattleCommonLogic.GetOtherTeamSide(OccupyingSide), getWinningPoint);

            // isBrokenの設定をしたいところだけど, このターンのIsBrokenの変化を取りたいので一括で別途更新するフローで.
            return dealtDamage;
        }
    }
}