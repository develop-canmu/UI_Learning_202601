using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Training;
using UnityEditor;
using UnityEngine;

namespace Pjfb.Editor
{
    public class TrainingPageCZoneDebug : TrainingPageDebugBase<TrainingActionPage>
    {
        /// <summary>設定ファイルの格納位置</summary>
        private static readonly string ConfigPath = "Assets/Pjfb/Runtime/AssetBundles/Remote/Training/{type}ZoneEffectConfig";
        private static readonly string LabelConfigPath = "Assets/Pjfb/Runtime/AssetBundles/Remote/Training/{type}LabelConfig";
        private static readonly string EnterConfigPath = "Assets/Pjfb/Runtime/AssetBundles/Remote/Training/Training{type}ZoneEnteringConfig";
        private static readonly string ProlongingConfigPath = "Assets/Pjfb/Runtime/AssetBundles/Remote/Training/TrainingConcentrationZoneProlongingConfig";
        private static readonly string UpgradeConfigPath = "Assets/Pjfb/Runtime/AssetBundles/Remote/Training/TrainingConcentrationZoneUpgradeConfig";
        
        private static readonly string FlowZonePracticeConfigPath = "Assets/Pjfb/Runtime/AssetBundles/Remote/Training/FlowZonePracticeEffectConfig";
        
        private TrainingMainArguments MainArguments{get{return PageObject.MainArguments;}}
        
        [PageDebugCategory("Cゾーン", "Cゾーン突入演出")]
        private void CZonePlay(int id = 1, TrainingConcentrationEffectType type = TrainingConcentrationEffectType.Concentration)
        {
            TrainingEventReward reward = new TrainingEventReward();
            
            reward.mTrainingConcentrationId = id;
            
            
            reward.getAbilityMapList = new TrainingAbility[0];
            reward.inspireList = new TrainingInspire[0];
            reward.conditionEffectRate = 99900;
            reward.mAbilityTrainingPointStatusList = new Networking.App.Request.WrapperIntList[0];
            
            TrainingPointStatus pointStatus = new TrainingPointStatus();
            pointStatus.mTrainingPointStatusEffectIdList = new long[]{};
            TrainingMainArguments args = new TrainingMainArguments(MainArguments.TrainingEvent, MainArguments.Pending, MainArguments.BattlePending, MainArguments.CharacterVariable, reward, pointStatus, MainArguments.ArgumentsKeeps, TrainingMainArguments.Options.ShowBonus);
            
            args.Pending.mTrainingConcentrationId = id;
            args.Pending.isFinishedConcentration = false;
            
            args.TrainingCardId = 1;
            args.JoinSupportCharacters = new long[0];
            // デバコマ用演出Idセット
            args.DebugOverrideConcentrationEffectId = id;
            
            if (type == TrainingConcentrationEffectType.Flow)
            {
                args.DebugConcentrationEffectType = TrainingConcentrationMasterObject.TrainingConcentrationType.Flow;
                // キャッシュ済みId破棄
                ResetConcentrationEffectId<TrainingFlowEffectPage>();
                PageObject.OpenPage(TrainingMainPageType.FlowEffect, args);
            }
            else
            {
                args.DebugConcentrationEffectType = TrainingConcentrationMasterObject.TrainingConcentrationType.CZone;
                // キャッシュ済みId破棄
                ResetConcentrationEffectId<TrainingConcentrationEffectPage>();
                PageObject.OpenPage(TrainingMainPageType.ConcentrationEffect, args);
            }
        }
        
        [PageDebugCategory("Cゾーン", "Cゾーン延長演出")]
        private void CZoneExtendPlay(int id = 1)
        {
            TrainingEventReward reward = new TrainingEventReward();
            
            reward.mTrainingConcentrationId = id;
            // 延長状態に
            reward.isConcentrationExtended = true;
            reward.getAbilityMapList = new TrainingAbility[0];
            reward.inspireList = new TrainingInspire[0];
            reward.conditionEffectRate = 99900;
            reward.mAbilityTrainingPointStatusList = new Networking.App.Request.WrapperIntList[0];
            TrainingPointStatus pointStatus = new TrainingPointStatus();
            pointStatus.mTrainingPointStatusEffectIdList = new long[]{};
            TrainingMainArguments args = new TrainingMainArguments(MainArguments.TrainingEvent, MainArguments.Pending, MainArguments.BattlePending, MainArguments.CharacterVariable, reward, pointStatus, MainArguments.ArgumentsKeeps, TrainingMainArguments.Options.ShowBonus);
            
            args.Pending.mTrainingConcentrationId = id;
            args.Pending.isFinishedConcentration = false;
            
            args.TrainingCardId = 1;
            args.JoinSupportCharacters = new long[0];
            // デバコマ用演出Idセット
            args.DebugOverrideConcentrationEffectId = id;
            args.DebugConcentrationEffectType = TrainingConcentrationMasterObject.TrainingConcentrationType.CZone;
            // キャッシュ済みId破棄
            ResetConcentrationEffectId<TrainingConcentrationEffectPage>();
            PageObject.OpenPage(TrainingMainPageType.ConcentrationEffect, args);
        }
        
        [PageDebugCategory("Cゾーン", "Cゾーンアップグレード演出")]
        private void CZoneUpgradePlay(int id = 1)
        {
            TrainingEventReward reward = new TrainingEventReward();
            
            reward.mTrainingConcentrationId = id;
            
            
            reward.getAbilityMapList = new TrainingAbility[0];
            reward.inspireList = new TrainingInspire[0];
            reward.conditionEffectRate = 99900;
            reward.mAbilityTrainingPointStatusList = new Networking.App.Request.WrapperIntList[0];
            TrainingPointStatus pointStatus = new TrainingPointStatus();
            pointStatus.mTrainingPointStatusEffectIdList = new long[]{};
            TrainingMainArguments args = new TrainingMainArguments(MainArguments.TrainingEvent, MainArguments.Pending, MainArguments.BattlePending, MainArguments.CharacterVariable, reward, pointStatus, MainArguments.ArgumentsKeeps, TrainingMainArguments.Options.ShowBonus);
            
            args.Pending.mTrainingConcentrationId = id;
            args.Pending.isFinishedConcentration = false;
            reward.isConcentrationGradeUp = true;
            
            args.TrainingCardId = 1;
            args.JoinSupportCharacters = new long[0];
            // デバコマ用演出Idセット
            args.DebugOverrideConcentrationEffectId = id;
            args.DebugConcentrationEffectType = TrainingConcentrationMasterObject.TrainingConcentrationType.CZone;
            // キャッシュ済みId破棄
            ResetConcentrationEffectId<TrainingConcentrationEffectPage>();
            PageObject.OpenPage(TrainingMainPageType.ConcentrationEffect, args);
        }
        
        [PageDebugCategory("Cゾーン", "Cゾーン終了演出")]
        private void CZoneEnd()
        {
            TrainingEventReward reward = new TrainingEventReward();
            
            reward.mTrainingConcentrationId = 0;
            
            reward.mTrainingEventId = 300;
            reward.getAbilityMapList = new TrainingAbility[0];
            reward.inspireList = new TrainingInspire[0];
            reward.conditionEffectRate = 99900;
            reward.mAbilityTrainingPointStatusList = new Networking.App.Request.WrapperIntList[0];
            TrainingPointStatus pointStatus = new TrainingPointStatus();
            pointStatus.mTrainingPointStatusEffectIdList = new long[]{};
            TrainingMainArguments args = new TrainingMainArguments(MainArguments.TrainingEvent, MainArguments.Pending, MainArguments.BattlePending, MainArguments.CharacterVariable, reward, pointStatus, MainArguments.ArgumentsKeeps, TrainingMainArguments.Options.ShowBonus);
            
            args.Pending.mTrainingConcentrationId = 0;
            args.Pending.isFinishedConcentration = true;
            
            args.TrainingCardId = 1;
            args.JoinSupportCharacters = new long[0];
            // キャッシュ済みId破棄
            ResetConcentrationEffectId<TrainingConcentrationEffectPage>();
            PageObject.OpenPage(TrainingMainPageType.ConcentrationEffect, args);
        }
        
          /// <summary> Fゾーン中練習演出 </summary>
        [PageDebugCategory("Cゾーン", "Fゾーン練習演出")]
        private void CardUnionEffect(int id = 1)
        {
            TrainingEventReward reward = new TrainingEventReward();
            reward.mAbilityTrainingPointStatusList = new Networking.App.Request.WrapperIntList[0];
            reward.getAbilityMapList = new TrainingAbility[0];
            reward.inspireList = new TrainingInspire[0];
         
            // Flowゾーンステータス獲得演出用に獲得ステータスをセット
            reward.param1 = 100;
            reward.param2 = 100;
            reward.param3 = 100;
            reward.param4 = 100;
            reward.param5 = 100;
            reward.spd = 100;
            reward.tec = 100;
            
            TrainingPointStatus pointStatus = new TrainingPointStatus();
            pointStatus.mTrainingPointStatusEffectIdList = new long[]{};
            TrainingMainArguments args = new TrainingMainArguments(MainArguments.TrainingEvent, MainArguments.Pending, MainArguments.BattlePending, MainArguments.CharacterVariable, reward, pointStatus, MainArguments.ArgumentsKeeps, TrainingMainArguments.Options.ShowBonus);
            args.TrainingCardId = 1;
            args.JoinSupportCharacters = new long[0];
            // Fゾーンとして上書きしておく
            args.DebugOverrideConcentrationEffectId = id;
            args.DebugConcentrationEffectType = TrainingConcentrationMasterObject.TrainingConcentrationType.Flow;
            PageObject.OpenPage(TrainingMainPageType.EventResult, args);
        }

        [PageDebugCategory("Config", "Zone")]
        private void OpenZoneConfig(TrainingConcentrationEffectType type = TrainingConcentrationEffectType.Concentration)
        {
            string configPath = GetReplacePath(ConfigPath, type);
            Object folder = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(configPath);
            Selection.activeObject = folder;
            EditorGUIUtility.PingObject(folder);
        }
        
        [PageDebugCategory("Config", "Label")]
        private void OpenLabelConfig(TrainingConcentrationEffectType type = TrainingConcentrationEffectType.Concentration)
        {
            string configPath = GetReplacePath(LabelConfigPath, type);
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(configPath);
        }

        [PageDebugCategory("Config", "Enter")]
        private void OpenEnterConfig(TrainingConcentrationEffectType type = TrainingConcentrationEffectType.Concentration)
        {
            string configPath = GetReplacePath(EnterConfigPath, type);
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(configPath);
        }
        
        [PageDebugCategory("Config", "Prolonging")]
        private void OpenProlongingConfig()
        {
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(ProlongingConfigPath);
        }
        
        [PageDebugCategory("Config", "Upgrade")]
        private void OpenUpgradeConfig()
        {
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(UpgradeConfigPath);
        }

        [PageDebugCategory("Config", "Practice")]
        private void OpenPracticeConfig()
        {
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(FlowZonePracticeConfigPath);
        }
        
        /// <summary> Typeごとのパス変換 </summary>
        private string GetReplacePath(string path, TrainingConcentrationEffectType type)
        {
            return path.Replace("{type}", type.ToString());
        }

        /// <summary> キャッシュしている演出Idのリセット用(キャッシュ済み演出が流れないので再度流せるようにするための関数) </summary>
        private void ResetConcentrationEffectId<T>() where T : TrainingConcentrationEffectBasePage
        {
            // キャッシュ済みの対象ページを探す
            T concentrationEffectPage = GameObject.FindAnyObjectByType<T>(FindObjectsInactive.Include);
            if (concentrationEffectPage != null)
            {
                concentrationEffectPage.ResetEffectId();
            }
        }
    }
}