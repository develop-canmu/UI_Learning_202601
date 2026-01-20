using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Training;
using Pjfb.UserData;

namespace Pjfb.Editor
{
    
    public abstract class TrainingPageDebugBase<T> : DebugPage<T> where T : CruFramework.Page.Page
    {
        
    }
    
    public class TrainingActionPageDebug : TrainingPageDebugBase<TrainingActionPage>
    {
        
        private TrainingMainArguments MainArguments{get{return PageObject.MainArguments;}}
        
        // デバッグ用に追加したマスタのリスト(キーに追加先、バリューに追加オブジェクトリスト)
        private Dictionary<IMasterContainer, List<IMasterValueObject>> additionalMasterList= new Dictionary<IMasterContainer, List<IMasterValueObject>>();

        
        [PageDebug("リザルト画面へ")]
        private void GotoResult()
        {
            // リザルトページを開く
            TrainingEventReward reward = new TrainingEventReward();
            reward.getAbilityMapList = new TrainingAbility[0];
            MainArguments.CharacterVariable.combatPower = 1323;
            
            MainArguments.CharacterVariable.abilityList = new TrainingAbility[1];
            MainArguments.CharacterVariable.abilityList[0] = new TrainingAbility();
            MainArguments.CharacterVariable.abilityList[0].id = 1;
            MainArguments.CharacterVariable.abilityList[0].level = 3;
            
            TrainingFriend friend = new TrainingFriend();
            friend.relationType = (int)TrainingUtility.FriendFollowType.None;
            
            friend.communityUserStatus = new UserCommunityUserStatus();
            friend.communityUserStatus.name = "あいうえお";
            friend.communityUserStatus.uMasterId = UserDataManager.Instance.user.uMasterId;
            friend.communityUserStatus.maxCombatPower = "1234567";
            
            // イベント
            FestivalEffectStatus effectStatus = new FestivalEffectStatus();
            FestivalPointProgress pointProgress = new FestivalPointProgress();
            
            pointProgress.beforeValue = 123;
            pointProgress.afterValue = 1234;
            pointProgress.valueDelta = pointProgress.afterValue - pointProgress.beforeValue;
            
            pointProgress.valueDeltaOriginalPointList = new FestivalOriginalPoint[2];
            pointProgress.valueDeltaOriginalPointList[0] = new FestivalOriginalPoint();
            pointProgress.valueDeltaOriginalPointList[0].factor = 1;
            pointProgress.valueDeltaOriginalPointList[0].value = 12345678;
            pointProgress.valueDeltaOriginalPointList[1] = new FestivalOriginalPoint();
            pointProgress.valueDeltaOriginalPointList[1].factor = 2;
            pointProgress.valueDeltaOriginalPointList[1].value = 987654;
            
            
            pointProgress.specificCharaBonusRateList = new FestivalSpecificCharaBonusRate[3];
            pointProgress.specificCharaBonusRateList[0] = new FestivalSpecificCharaBonusRate();
            pointProgress.specificCharaBonusRateList[0].bonusRate = 50;

            pointProgress.specificCharaBonusRateList[1] = new FestivalSpecificCharaBonusRate();
            pointProgress.specificCharaBonusRateList[1].bonusRate = 150;
            
            pointProgress.specificCharaBonusRateList[2] = new FestivalSpecificCharaBonusRate();
            pointProgress.specificCharaBonusRateList[2].bonusRate = 3;
            
            pointProgress.effectStatusBonusRate = 123;
            
            effectStatus.isCausedNow = true;
            
            effectStatus.expireAt = "2023-03-28 15:00:00";
            effectStatus.value = 120;

            MissionUserAndGuild[] missionList = new MissionUserAndGuild[0];

            TrainingMainArguments args = new TrainingMainArguments(MainArguments.TrainingEvent, MainArguments.Pending, MainArguments.BattlePending, MainArguments.CharacterVariable, reward, friend, pointProgress, effectStatus, missionList, MainArguments.ArgumentsKeeps);
            PageObject.OpenPage(TrainingMainPageType.TrainingResult, args);
        }
        
        [PageDebug("インスピレーションブースト演出")]
        private void BounusEffect1()
        {
            TrainingEventReward reward = new TrainingEventReward();
                
            reward.getAbilityMapList = new TrainingAbility[0];
            reward.inspireList = new TrainingInspire[0];
            reward.inspireEnhanceRate  = 99900;
            reward.conditionEffectRate = 99900;
            reward.mAbilityTrainingPointStatusList = new Networking.App.Request.WrapperIntList[0];
            TrainingPointStatus pointStatus = new TrainingPointStatus();
            pointStatus.mTrainingPointStatusEffectIdList = new long[]{};
            TrainingMainArguments args = new TrainingMainArguments(MainArguments.TrainingEvent, MainArguments.Pending, MainArguments.BattlePending, MainArguments.CharacterVariable, reward, pointStatus, MainArguments.ArgumentsKeeps, TrainingMainArguments.Options.ShowBonus);
            args.TrainingCardId = 1;
            args.JoinSupportCharacters = new long[0];
            PageObject.OpenPage(TrainingMainPageType.EventResult, args);
        }
        
        [PageDebug("スキル獲得演出")]
        private void BounusEffect2()
        {
            // 1, 2, 1005
            TrainingEventReward reward = new TrainingEventReward();
                
            reward.getAbilityMapList = new TrainingAbility[0];
                
                
            reward.getAbilityMapList = new TrainingAbility[3];
                
            TrainingAbility ability1 = new TrainingAbility();
            ability1.id = 1;
            ability1.level = 1;
            reward.getAbilityMapList[0] = ability1;
                
            TrainingAbility ability2 = new TrainingAbility();
            ability2.id = 2;
            ability2.level = 2;
            reward.getAbilityMapList[1] = ability2;
                
            TrainingAbility ability3 = new TrainingAbility();
            ability3.id = 1005;
            ability3.level = 5;
            reward.getAbilityMapList[2] = ability3;
                
            reward.inspireList = new TrainingInspire[0];
            reward.isInspireLevelUp = true;
            reward.conditionEffectRate = 550000;
            reward.mAbilityTrainingPointStatusList = new Networking.App.Request.WrapperIntList[0];
            TrainingPointStatus pointStatus = new TrainingPointStatus();
            pointStatus.mTrainingPointStatusEffectIdList = new long[]{};
            TrainingMainArguments args = new TrainingMainArguments(MainArguments.TrainingEvent, MainArguments.Pending, MainArguments.BattlePending, MainArguments.CharacterVariable, reward, pointStatus, MainArguments.ArgumentsKeeps, TrainingMainArguments.Options.ShowBonus);
            args.TrainingCardId = 1;
            args.JoinSupportCharacters = new long[0];
            PageObject.OpenPage(TrainingMainPageType.EventResult, args);
        }
        
        [PageDebug("インスピレーション獲得演出")]
        private void BounusEffect3()
        { 
            TrainingEventReward reward = new TrainingEventReward();
                
            reward.inspireEnhanceRate = 990000;
            reward.conditionEffectRate = 990000;
            
            reward.condition = 10;
            
            reward.getAbilityMapList = new TrainingAbility[0];
            
            reward.inspireList = new TrainingInspire[5];
            reward.inspireList[0] = new TrainingInspire();
            reward.inspireList[0].id = 1;
            reward.inspireList[0].type = 1;
            reward.inspireList[0].mCharaId = 10001001;
            reward.inspireList[0].mTrainingCardId = 1;
            
            reward.inspireList[1] = new TrainingInspire();
            reward.inspireList[1].id = 2;
            reward.inspireList[1].type = 1;
            reward.inspireList[1].mCharaId = 10001002;
            reward.inspireList[1].mTrainingCardId = 1;
            
            reward.inspireList[2] = new TrainingInspire();
            reward.inspireList[2].id = 3;
            reward.inspireList[2].type = 2;
            reward.inspireList[2].mCharaId = 10001001;
            reward.inspireList[2].mTrainingCardId = 1;
            
            reward.inspireList[3] = new TrainingInspire();
            reward.inspireList[3].id = 1;
            reward.inspireList[3].type = 2;
            reward.inspireList[3].mCharaId = 10001003;
            reward.inspireList[3].mTrainingCardId = 1;
            
            reward.inspireList[4] = new TrainingInspire();
            reward.inspireList[4].id = 1;
            reward.inspireList[4].type = 1;
            reward.inspireList[4].mCharaId = 10010001;
            reward.inspireList[4].mTrainingCardId = 1;
            
            MainArguments.Pending.inspireList = new TrainingInspire[2];
            MainArguments.Pending.inspireList[0] = new TrainingInspire();
            MainArguments.Pending.inspireList[0].id = 1;
            MainArguments.Pending.inspireList[0].mCharaId = 10003001;
            MainArguments.Pending.inspireList[0].mTrainingCardId = 1;
            
            MainArguments.Pending.inspireList[1] = new TrainingInspire();
            MainArguments.Pending.inspireList[1].id = 1;
            MainArguments.Pending.inspireList[1].mCharaId = 10001001;
            MainArguments.Pending.inspireList[1].mTrainingCardId = 1;
            
            reward.getAbilityMapList = new TrainingAbility[2];
            
            TrainingAbility ability1 = new TrainingAbility();
            ability1.id = 1;
            ability1.level = 1;
            reward.getAbilityMapList[0] = ability1;
            
            // Flowスキル
            TrainingAbility ability2 = new TrainingAbility();
            long flowAbilityId = MasterManager.Instance.abilityMaster.values.First(x => x.CategoryEnum == AbilityMasterObject.AbilityCategory.Flow).id;
            ability2.id = flowAbilityId;
            ability2.level = 3;
            reward.getAbilityMapList[1] = ability2;
            
            reward.isInspireLevelUp = true;
            
            reward.param1 = 100;

            reward.mAbilityTrainingPointStatusList = new Networking.App.Request.WrapperIntList[0];
            TrainingPointStatus pointStatus = new TrainingPointStatus();
            pointStatus.mTrainingPointStatusEffectIdList = new long[]{};
            TrainingMainArguments args = new TrainingMainArguments(MainArguments.TrainingEvent, MainArguments.Pending, MainArguments.BattlePending, MainArguments.CharacterVariable, reward, pointStatus, MainArguments.ArgumentsKeeps, TrainingMainArguments.Options.ShowBonus);
            args.TrainingCardId = 1;
            args.JoinSupportCharacters = new long[0];
            PageObject.OpenPage(TrainingMainPageType.EventResult, args);
        }
        
                
        [PageDebug("ターン延長演出")]
        private void ExtraTurnEffect1()
        {
            var args = new TrainingMainArguments(MainArguments, MainArguments.ArgumentsKeeps);
            args.Pending.goalList = new TrainingGoal[1];
            args.Pending.goalList[0] = new TrainingGoal();
            args.Pending.nextGoalIndex = 0;
            args.CurrentTarget.restTurnCount = 0;
            args.CurrentTarget.addedTurn = 5;
            args.CurrentTarget.firstAddedTurn = 2;
            args.CurrentTarget.restAllAddedTurnCount = args.CurrentTarget.restTurnCount + args.CurrentTarget.addedTurn;
            args.CurrentTarget.restFirstAddedTurnCount = args.CurrentTarget.restTurnCount + args.CurrentTarget.firstAddedTurn;
                
            TrainingEventReward reward = args.Reward;
            reward = new TrainingEventReward();
            reward.mTrainingConcentrationId = 1;
            reward.isConcentrationGradeUp = true;
            reward.isConcentrationExtended = true;
            args.Pending.isFinishedConcentration = true;
            PageObject.OpenPage(TrainingMainPageType.Top, args);
        }
        
        [PageDebug("ターン延長継続")]
        private void ExtraTurnEffect2()
        {
            var args = new TrainingMainArguments(MainArguments, MainArguments.ArgumentsKeeps);
            args.CurrentTarget.restTurnCount = 0;
            args.CurrentTarget.addedTurn = 4;
            args.CurrentTarget.firstAddedTurn = 2;
            args.CurrentTarget.restAllAddedTurnCount = args.CurrentTarget.addedTurn - args.CurrentTarget.firstAddedTurn;
            args.CurrentTarget.restFirstAddedTurnCount = 0;
            PageObject.OpenPage(TrainingMainPageType.ExtraTurnLottery, args);
        }
        
        [PageDebug("スキル追加獲得")]
        private void PointEffect2()
        {
            TrainingEventReward reward = new TrainingEventReward();
            reward.mAbilityTrainingPointStatusList = new Networking.App.Request.WrapperIntList[7];
                
            for (int i = 0; i < 7; i++)
            { 
                Networking.App.Request.WrapperIntList ability = new Networking.App.Request.WrapperIntList();
                // 3.5弾実装時、データがないので偶数ならid = 1、奇数ならid = 2
                ability.l = new long[] { i % 2 == 0 ? 1 : 2 , i }; 
                reward.mAbilityTrainingPointStatusList[i] = ability;
            }
                
            reward.conditionEffectRate = 0;
            reward.getAbilityMapList = new TrainingAbility[0];
            reward.inspireList = new TrainingInspire[0];
            TrainingPointStatus pointStatus = new TrainingPointStatus();
            pointStatus.mTrainingPointStatusEffectIdList = new long[]{};
            pointStatus.value = 0;
            pointStatus.level = 1;
            pointStatus.mTrainingPointStatusEffectIdList = new long[0];
            TrainingMainArguments args = new TrainingMainArguments(MainArguments.TrainingEvent, MainArguments.Pending, MainArguments.BattlePending, MainArguments.CharacterVariable, reward, pointStatus, MainArguments.ArgumentsKeeps, TrainingMainArguments.Options.ShowBonus);
            args.TrainingCardId = 1;
            args.JoinSupportCharacters = new long[0];
            PageObject.OpenPage(TrainingMainPageType.EventResult, args);
        }
        
        
        [PageDebug("ターン延長変換演出")]
        private void ExchangePointEffect1()
        {
            TrainingEventReward reward = new TrainingEventReward();
            reward.mAbilityTrainingPointStatusList = new Networking.App.Request.WrapperIntList[0];
            reward.getAbilityMapList = new TrainingAbility[0];
            reward.inspireList = new TrainingInspire[0];
            reward.turnAddValue = 0;
            reward.pointConvertAddedTurnValue = 1;
            reward.pointConvertConditionTier = 1;
            reward.addedTurnPointValue = 1000;
            reward.conditionTierPointValue = 1000;
            TrainingPointStatus pointStatus = new TrainingPointStatus();
            pointStatus.mTrainingPointStatusEffectIdList = new long[]{};
            pointStatus.value = 2000;
            pointStatus.level = 1;
            TrainingMainArguments args = new TrainingMainArguments(MainArguments.TrainingEvent, MainArguments.Pending, MainArguments.BattlePending, MainArguments.CharacterVariable, reward, pointStatus, MainArguments.ArgumentsKeeps, TrainingMainArguments.Options.ShowBonus);
            args.TrainingCardId = 1;
            args.JoinSupportCharacters = new long[0];
            PageObject.OpenPage(TrainingMainPageType.EventResult, args);
        }
        
        [PageDebug("トータルボーナス演出")]
        private void BoostBonusEffect(long conditionRate)
        {
            TrainingEventReward reward = new TrainingEventReward();
            reward.conditionEffectRate = conditionRate * 100;
            reward.baseConditionEffectRate = 10000;
            reward.isGradeUp = true;
            reward.mAbilityTrainingPointStatusList = new Networking.App.Request.WrapperIntList[0];
            reward.getAbilityMapList = new TrainingAbility[0];
            reward.inspireList = new TrainingInspire[0];
            reward.pointStatusEffectRate = 20000;
            reward.pointStatusEffectRateLabelType = 1;
            TrainingPointStatus pointStatus = new TrainingPointStatus();
            pointStatus.mTrainingPointStatusEffectIdList = new long[]{};
            TrainingMainArguments args = new TrainingMainArguments(MainArguments.TrainingEvent, MainArguments.Pending, MainArguments.BattlePending, MainArguments.CharacterVariable, reward, pointStatus, MainArguments.ArgumentsKeeps, TrainingMainArguments.Options.ShowBonus);
            args.TrainingCardId = 1;
            args.JoinSupportCharacters = new long[0];
            PageObject.OpenPage(TrainingMainPageType.EventResult, args);
        }
        
        [PageDebug("ブーストボーナス演出")]
        private async UniTask BoostChanceEffect()
        {
            // モーダルを開く
            PageObject.OnBoostLevelUpButton();
            // 開くまで待つ
            await UniTask.Delay(1000);
            
            // レスポンスを作る
            TrainingUpdatePointLevelAPIResponse response = new TrainingUpdatePointLevelAPIResponse();
            
            response.mTrainingPointStatusEffectIdList = new long[]{1};
            response.pointStatus = new TrainingPointStatus();
            response.pointStatus.mTrainingPointStatusEffectIdList = new long[]{};
            response.additionMTrainingPointStatusEffectIdList = new long[]{1, 2, 3};
            response.charaMTrainingPointStatusEffectIdList = new long[] { 4, 5 };
            var charaEffectMaster = MasterManager.Instance.trainingPointStatusEffectCharaMaster.values.First();
            // 適当に最初のマスタのデータでセット
            response.mTrainingPointStatusEffectCharaId = charaEffectMaster.id;

            // デバック用に一時的にデータ入れとく(編成に入ってないとエラーになるので)
            TrainingCharacterData debugCharacterData = new TrainingCharacterData(charaEffectMaster.mCharaId, 0,0, -1);
            MainArguments.SupportAndFriendCharacterDatas.Add(debugCharacterData);
            
            // Viewの取得
            TrainingBoostChanceView view = Get<TrainingBoostChanceView>("boostChanceView");
            // 演出開始
            await view.StartBoostAsync(response);

            // 一時追加データ削除
            MainArguments.SupportAndFriendCharacterDatas.Remove(debugCharacterData);
        }

        [PageDebug("リミットブレイク演出")]
        private void LimitBreakEffect(long rate, bool isTotalBonusCheck, TrainingConcentrationEffectType type = TrainingConcentrationEffectType.Concentration)
        {
            TrainingEventReward reward = new TrainingEventReward();
            reward.conditionEffectRate = rate * 100;
            reward.baseConditionEffectRate = 100 * 100;
            reward.isGradeUp = true;
            reward.mAbilityTrainingPointStatusList = new Networking.App.Request.WrapperIntList[0];
            reward.getAbilityMapList = new TrainingAbility[0];
            reward.inspireList = new TrainingInspire[0];

            if (isTotalBonusCheck)
            {
                reward.pointStatusEffectRate = 20000;
                reward.pointStatusEffectRateLabelType = 1;
            }

            TrainingPointStatus pointStatus = new TrainingPointStatus();
            pointStatus.mTrainingPointStatusEffectIdList = new long[]{};
            TrainingMainArguments args = new TrainingMainArguments(MainArguments.TrainingEvent, MainArguments.Pending, MainArguments.BattlePending, MainArguments.CharacterVariable, reward, pointStatus, MainArguments.ArgumentsKeeps, TrainingMainArguments.Options.ShowBonus);
            args.TrainingCardId = 1;
            args.JoinSupportCharacters = new long[0];

            switch (type)
            {
                // 通常Cゾーンの時は何もしない
                case TrainingConcentrationEffectType.Concentration:
                    break;
                
                // Flowゾーン
                case TrainingConcentrationEffectType.Flow:
                    args.DebugOverrideConcentrationEffectId = 1;
                    args.DebugConcentrationEffectType = TrainingConcentrationMasterObject.TrainingConcentrationType.Flow;
                    break;
            }
            
            PageObject.OpenPage(TrainingMainPageType.EventResult, args);
        }

        [PageDebug("カードコンボ演出")]
        private void CardComboEffect(long comboValue, long statusRate, long comboBonusRate)
        {
            CardComboEffectAsync(comboValue, statusRate, comboBonusRate).Forget();
        }

        // カードコンボ演出
        private async UniTask CardComboEffectAsync(long comboValue, long statusRate, long comboBonusRate)
        {
            TrainingEventReward reward = new TrainingEventReward();
            reward.mAbilityTrainingPointStatusList = new Networking.App.Request.WrapperIntList[0];
            reward.getAbilityMapList = new TrainingAbility[0];
            reward.inspireList = new TrainingInspire[0];
            reward.mTrainingCardComboIdList = new long[] { -1 };
            
            long comboId = -1;
            // 最小コンボ数は２コンボ
            comboValue = Math.Max(2, comboValue);
            long baseStatusRate = statusRate * 100;
            comboBonusRate *= 10000;
            
            // コンボマスターのJson
            string comboMasterJson = 
            $@"{{
                ""id"": {comboId},
                ""name"": """",
                ""groupId"": 0,
                ""baseStatusRate"": {baseStatusRate},
                ""comboBonusRate"": {comboBonusRate},
                ""forceRate"": 0,
                ""forceLimit"": 0,
                ""priority"": 0,
                ""effectNumber"": 0,
                ""deleteFlg"": false
            }}";

            TrainingCardComboMasterValueObject cardComboMaster = JsonUtility.FromJson<TrainingCardComboMasterValueObject>(comboMasterJson);
            
            // 追加したマスタを記録
            additionalMasterList.Add(MasterManager.Instance.trainingCardComboMaster, new List<IMasterValueObject>(){cardComboMaster});
            // デバッグ用に一時的にマスタを追加
            MasterManager.Instance.trainingCardComboMaster.UpdateLocalData(cardComboMaster);
         
            // カードコンボエレメントのJson
            string cardComboElementJson =
            @"{{
                ""id"": {0},
                ""mTrainingCardComboId"": -1,
                ""mTrainingCardId"": 1,
                ""deleteFlg"": false
            }}";

            // 追加したマスタを保持するリスト
            List<IMasterValueObject> cardComboElementMasterList = new List<IMasterValueObject>();
            
            // コンボ数分マスタを追加
            for (int i = 0; i < comboValue; i++)
            {
                // idは通常のマスタを書き換えないようにマイナスを振っておく
                string json = string.Format(cardComboElementJson, (i + 1) * -1);
                TrainingCardComboElementMasterValueObject cardComboElementMaster = JsonUtility.FromJson<TrainingCardComboElementMasterValueObject>(json);
                // 追加したマスタリストに入れる
                cardComboElementMasterList.Add(cardComboElementMaster);
                // マスタを追加
                MasterManager.Instance.trainingCardComboElementMaster.UpdateLocalData(cardComboElementMaster);
            }
            // 追加したマスタを記録
            additionalMasterList.Add(MasterManager.Instance.trainingCardComboElementMaster, cardComboElementMasterList);

            
            TrainingPointStatus pointStatus = new TrainingPointStatus();
            pointStatus.mTrainingPointStatusEffectIdList = new long[]{};
            TrainingMainArguments args = new TrainingMainArguments(MainArguments.TrainingEvent, MainArguments.Pending, MainArguments.BattlePending, MainArguments.CharacterVariable, reward, pointStatus, MainArguments.ArgumentsKeeps, TrainingMainArguments.Options.ShowBonus);
            args.TrainingCardId = 1;
            args.JoinSupportCharacters = new long[0];
            await PageObject.OpenPageAsync(TrainingMainPageType.EventResult, args);
            
            // デバッグ用に追加したマスタを消す
            ClearDebugMaster();
        }
        
        //// <summary> カードコンボハイライト表示 </summary>
        [PageDebug("カードコンボハイライト演出")]
        private void CardComboHighlightEffect(bool isSubHighlight, bool isSwitch)
        {
            List<TrainingCardComboHighlightView.CardComboData> comboDataList = new List<TrainingCardComboHighlightView.CardComboData>();
            TrainingCard[] hand = MainArguments.Pending.handList;
            comboDataList.Add(new TrainingCardComboHighlightView.CardComboData(-1, new List<long>() { hand[0].mTrainingCardId, hand[1].mTrainingCardId, hand[3].mTrainingCardId }));
            // サブハイライト表示
            if (isSubHighlight)
            {
                comboDataList.Add(new TrainingCardComboHighlightView.CardComboData(-2, new List<long>() { hand[2].mTrainingCardId, hand[4].mTrainingCardId }));
            }
            // 切り替え表現を確認するなら
            if (isSwitch)
            {
                comboDataList.Add(new TrainingCardComboHighlightView.CardComboData(-3, new List<long>() { hand[1].mTrainingCardId, hand[2].mTrainingCardId, hand[4].mTrainingCardId }));
            }

            Dictionary<long, List<TrainingCardComboHighlightView.CardComboData>> comboDictionary = new Dictionary<long, List<TrainingCardComboHighlightView.CardComboData>>();

            foreach (TrainingCardComboHighlightView.CardComboData comboData in comboDataList)
            {
                foreach (long cardId in comboData.ActivateCardIdList)
                {
                    // まだキーにないなら
                    if (comboDictionary.ContainsKey(cardId) == false)
                    {
                        comboDictionary.Add(cardId, new List<TrainingCardComboHighlightView.CardComboData>());
                    }
                    comboDictionary[cardId].Add(comboData);
                }
            }
            
            // Viewを取得
            TrainingCardComboHighlightView view = Get<TrainingCardComboHighlightView>("cardComboHighlightView");
            // カードコンボのデータをセットしエフェクトを再生
            view.UpdateComboData(comboDataList, comboDictionary);
            view.ShowHighlightAsync().Forget();
        }

        /// <summary> カードユニオン演出 </summary>
        [PageDebug("カードユニオン演出")]
        private void CardUnionEffect(bool isCardOver = false)
        {
            TrainingEventReward reward = new TrainingEventReward();
            reward.mAbilityTrainingPointStatusList = new Networking.App.Request.WrapperIntList[0];
            reward.getAbilityMapList = new TrainingAbility[0];
            reward.inspireList = new TrainingInspire[0];
            
            TrainingUnionCardReward unionCardReward = new TrainingUnionCardReward();
            // いったん手札のカードにしとく
            List<TrainingCard> unionCardList = MainArguments.Pending.handList.ToList();
            // 適当に複製しとく
            if (isCardOver)
            {
                unionCardList.AddRange(unionCardList);
            }
            unionCardReward.trainingCardList = unionCardList.Select(card => new TrainingTrainingCardData { id = card.mTrainingCardId, mCharaId = card.mCharaId, mCharaIdList = Array.Empty<long>()}).ToArray();
            long unionBaseCardId = MasterManager.Instance.trainingCardCharaMaster.values.First(x => x.mCharaId == MainArguments.TrainingCharacter.MCharId).mTrainingCardId;
            unionCardReward.baseTrainingData = new TrainingTrainingCardData();
            unionCardReward.baseTrainingData.id = unionBaseCardId;
            unionCardReward.baseTrainingData.mCharaId = MainArguments.TrainingCharacter.MCharId;
            unionCardReward.baseTrainingData.mCharaIdList = new long[] { MainArguments.SupportAndFriendCharacterDatas.First().MCharId };
            
            unionCardReward.effectRate = 10000;
            unionCardReward.param1 = 100;
            unionCardReward.param2 = 100;
            unionCardReward.param3 = 100;
            unionCardReward.param4 = 100;
            unionCardReward.param5 = 100;
            unionCardReward.spd = 100;
            unionCardReward.tec = 100;
            
            reward.concentrationUnionCard = unionCardReward;
            
            // Flowゾーンステータス獲得演出用に獲得ステータスをセット
            reward.param1 = 100;
            reward.param2 = 100;
            reward.param3 = 100;
            reward.param4 = 100;
            reward.param5 = 100;
            reward.spd = 100;
            reward.tec = 100;

            reward.mTrainingConcentrationId = 1;
            
            reward.inspireList = new TrainingInspire[1];
            reward.inspireList[0] = new TrainingInspire();
            reward.inspireList[0].id = 1;
            reward.inspireList[0].type = 1;
            reward.inspireList[0].mCharaId = 10001001;
            reward.inspireList[0].mTrainingCardId = 1;

            reward.flowInspireList = new TrainingInspire[1];
            reward.flowInspireList[0] = new TrainingInspire();
            reward.flowInspireList[0].id = 3;
            reward.flowInspireList[0].type = 2;
            reward.flowInspireList[0].mCharaId = 10001001;
            reward.flowInspireList[0].mTrainingCardId = 1;
            
            TrainingPointStatus pointStatus = new TrainingPointStatus();
            pointStatus.mTrainingPointStatusEffectIdList = new long[]{};

            // インスピレーション表示用に手札のカードIdを入れとく
            TrainingPending pending = MainArguments.Pending;
            pending.inspireList = new TrainingInspire[3];
            
            pending.inspireList[0] = new TrainingInspire();
            pending.inspireList[0].id = 1;
            pending.inspireList[0].mCharaId = unionCardReward.baseTrainingData.mCharaId;
            pending.inspireList[0].mTrainingCardId = unionCardReward.baseTrainingData.id;
            pending.inspireList[0].mTrainingCardCharaId = MasterManager.Instance.trainingCardCharaMaster.values.First(x => x.mTrainingCardId == unionCardReward.baseTrainingData.id).id;
         
            pending.inspireList[1] = new TrainingInspire();
            pending.inspireList[1].id = 1;
            pending.inspireList[1].mCharaId = unionCardReward.trainingCardList[2].mCharaId;
            pending.inspireList[1].mTrainingCardId = unionCardReward.trainingCardList[2].id;
            pending.inspireList[1].mTrainingCardCharaId = unionCardList[2].mTrainingCardCharaId;
     
            pending.inspireList[2] = new TrainingInspire();
            pending.inspireList[2].id = 1;
            pending.inspireList[2].mCharaId = unionCardReward.trainingCardList[4].mCharaId;
            pending.inspireList[2].mTrainingCardId = unionCardReward.trainingCardList[4].id;
            pending.inspireList[2].mTrainingCardCharaId =  unionCardList[4].mTrainingCardCharaId;
            
            TrainingMainArguments args = new TrainingMainArguments(MainArguments.TrainingEvent, pending, MainArguments.BattlePending, MainArguments.CharacterVariable, reward, pointStatus, MainArguments.ArgumentsKeeps);
            // Fゾーンとして上書きしておく
            args.DebugOverrideConcentrationEffectId = 1;
            args.DebugConcentrationEffectType = TrainingConcentrationMasterObject.TrainingConcentrationType.Flow;
            PageObject.OpenPage(TrainingMainPageType.EventResult, args);
        }

        [PageDebug("Flowポイント獲得演出")]
        private void GetFlowPointEffect(long getExp = 100, long addTurnExp = 100, long conditionTierExp = 100)
        {
            TrainingEventReward reward = new TrainingEventReward();
            reward.mAbilityTrainingPointStatusList = new Networking.App.Request.WrapperIntList[0];
            reward.getAbilityMapList = new TrainingAbility[0];
            reward.inspireList = new TrainingInspire[0];
            
            // 獲得ステータスをセット
            reward.param1 = 100;
            reward.param2 = 100;
            reward.param3 = 100;
            reward.param4 = 100;
            reward.param5 = 100;
            reward.spd = 100;
            reward.tec = 100;
            
            reward.concentrationExp = getExp;
            reward.concentrationExpAddTurn = addTurnExp;
            reward.concentrationExpConditionTier = conditionTierExp;
            // 変換ターン数
            reward.pointConvertAddedTurnValue = 2;
            
            // もともと持ってるExp
            long baseExp = 10;
            MainArguments.Pending.concentrationExp = baseExp + getExp + addTurnExp + conditionTierExp;
            
            TrainingPointStatus pointStatus = new TrainingPointStatus();
            pointStatus.mTrainingPointStatusEffectIdList = new long[]{};
            TrainingMainArguments args = new TrainingMainArguments(MainArguments.TrainingEvent, MainArguments.Pending, MainArguments.BattlePending, MainArguments.CharacterVariable, reward, pointStatus, MainArguments.ArgumentsKeeps, TrainingMainArguments.Options.ShowBonus);
            args.TrainingCardId = 1;
            args.JoinSupportCharacters = new long[0];
            PageObject.OpenPage(TrainingMainPageType.EventResult, args);
        }
        
        
        //// <summary> デバッグ用に追加したマスタを削除 </summary>
        private void ClearDebugMaster()
        {
            foreach (var keyValuePair in additionalMasterList)
            {
                IMasterContainer masterContainer = keyValuePair.Key;
                foreach (var master in keyValuePair.Value)
                {
                    masterContainer.DeleteLocalData(master);
                }
            }
            
            additionalMasterList.Clear();
        }
    }
}