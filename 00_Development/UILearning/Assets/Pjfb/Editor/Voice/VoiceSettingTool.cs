using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Pjfb.Voice
{
    public static class VoiceSettingTool
    {
        
        private const string VoiceRemoteDirectoryPath = "Assets/Pjfb/Runtime/AssetBundles/Remote/Voice";
        private const string VoiceLocalDirectoryPath = "Assets/Pjfb/Runtime/AssetBundles/Local/Voice";
        private const string VoiceResourceSettingsName = "VoiceResourceSettings.asset";
        [MenuItem("Pjfb/Voice/UpdateVoiceList")]
        private static void CreateVoiceList()
        {
            var settingPath = $"{VoiceRemoteDirectoryPath}/{VoiceResourceSettingsName}";
            var settingAsset = AssetDatabase.LoadAssetAtPath<VoiceResourceSettings>(settingPath);
            if (settingAsset == null)
            {
                CruFramework.Logger.Log("Missing Voice Resources Settings");
                return;
            }
            settingAsset.charaVoiceList = new List<VoiceResourceSettings.CharaData>();
            settingAsset.scenarioVoiceList = new List<VoiceResourceSettings.VoiceDetail>();

            AddVoice(VoiceRemoteDirectoryPath,settingAsset);
            AddVoice(VoiceLocalDirectoryPath,settingAsset);
            
            foreach (var setting in settingAsset.charaVoiceList)
            {
                setting.UpdateUsageTypeCount();
            }
            EditorUtility.SetDirty(settingAsset);
            AssetDatabase.SaveAssets();
        }
        
        private static void AddVoice(string directoryPath, VoiceResourceSettings settingAsset)
        {
            var guids = AssetDatabase.FindAssets("t:AudioClip", new string[] {directoryPath});
            var paths = guids.Select(AssetDatabase.GUIDToAssetPath).ToArray();

            if (paths == null || paths.Length == 0)
            {
                CruFramework.Logger.Log("Missing Voice Assets");
                return;
            }
            foreach (var path in paths)
            {
                var name = Path.GetFileName(path);
                name = name.Replace(Path.GetExtension(path), "");
                var param = name.Split("_");

                var personalId = 0;
                var useType = 0;
                var voiceType = VoiceResourceSettings.VoiceType.Other;
                var locationType = VoiceResourceSettings.LocationType.Unknown;
                var insp_number = 0;
                var personalUniqueId = 0;

                var type = param[0];
                switch (type)
                {
                    case "in":
                        voiceType = VoiceResourceSettings.VoiceType.In;
                        personalId = int.Parse(param[1]);
                        useType = int.Parse(param[2]);
                        locationType = settingAsset.IndexToUsageType(voiceType, useType);
                        if (param.Length > 3)
                        {
                            personalUniqueId = int.Parse(param[3]);
                        }
                        break;
                    case "insp":
                        voiceType = VoiceResourceSettings.VoiceType.InSp;
                        personalId = int.Parse(param[1]);
                        useType = int.Parse(param[2]);
                        // 掛け合いの前後を扱う必要がなければスルー
                        //insp_number = int.Parse(param[3]); 
                        locationType = settingAsset.IndexToUsageType(voiceType, useType);
                        if (param.Length > 4)
                        {
                            personalUniqueId = int.Parse(param[4]);
                        }
                        break;
                    case "sys":
                        voiceType = VoiceResourceSettings.VoiceType.System;
                        personalId = int.Parse(param[1]);
                        useType = int.Parse(param[2]);
                        locationType = settingAsset.IndexToUsageType(voiceType, useType);
                        if (param.Length > 3)
                        {
                            personalUniqueId = int.Parse(param[3]);
                        }
                        break;
                    case "part":
                        voiceType = VoiceResourceSettings.VoiceType.Part;
                        personalId = int.Parse(param[1]);
                        useType = int.Parse(param[2]);
                        locationType = settingAsset.IndexToUsageType(voiceType, useType);
                        if (param.Length > 3)
                        {
                            personalUniqueId = int.Parse(param[3]);
                        }
                        break;
                    case "origin":
                        voiceType = VoiceResourceSettings.VoiceType.Scenario;
                        personalId = int.Parse(param[2]);
                        useType = int.Parse(param[3]);
                        locationType = settingAsset.IndexToUsageType(voiceType, useType);
                        personalUniqueId = 0;
                        break;
                    case "skill":
                        voiceType = VoiceResourceSettings.VoiceType.Skill;
                        personalId = int.Parse(param[1]);
                        useType = int.Parse(param[2]);
                        locationType = VoiceResourceSettings.LocationType.Unknown;
                        if (param.Length > 3)
                        {
                            personalUniqueId = int.Parse(param[3]);
                        }
                        break;
                    default:
                        break;
                }

                // ボイス詳細
                var detail = new VoiceResourceSettings.VoiceDetail();
                detail.personalId = personalId;
                detail.personalUniqueId = personalUniqueId;
                detail.path = name;
                detail.useType = useType;
                detail.locationType = locationType;
                detail.voiceType = voiceType;

                CruFramework.Logger.Log($"{name}/{personalId}/{useType}/{insp_number}");

                if (voiceType != VoiceResourceSettings.VoiceType.Scenario)
                {
                    var chara = settingAsset.charaVoiceList.FirstOrDefault(chara => chara.personalId == personalId);
                    if (chara == null)
                    {
                        chara = new VoiceResourceSettings.CharaData(personalId);
                        settingAsset.charaVoiceList.Add(chara);
                    }
                    chara.AddVoiceDetail(detail);
                }
                else
                {
                    // チュートリアルシナリオ用のボイスは分ける
                    settingAsset.scenarioVoiceList.Add(detail);
                }
            }
        }
        
    }
}