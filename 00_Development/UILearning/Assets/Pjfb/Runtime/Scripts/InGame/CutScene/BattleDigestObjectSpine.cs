using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Addressables;
using CruFramework.Audio;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Spine;
using UnityEngine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;

namespace Pjfb.InGame
{
    public class BattleDigestObjectSpine : MonoBehaviour
    {
        [SerializeField]
        private SkeletonAnimation skeletonAnimation = null;
        [SerializeField] 
        private string customSkinIdentifier;
        [SerializeField] [SpineSlot] 
        private string[] customSkinSlot;

        private Texture _tempMainTex;
        private Material _tempOverrideMat;

        /// <summary>
        /// SkeletonData変更しないフラグ
        /// </summary>
        [SerializeField] private bool isNotSetSkeletonData = false;
        private CancellationTokenSource source = null;

        private float defaultSpineSpeed = 0.0f;
        private Texture2D _repackedTex;
        private MeshRenderer _attachedMeshRenderer;

        private void Awake()
        {
            defaultSpineSpeed = skeletonAnimation?.timeScale ?? 0.0f;
        }

        public void SetSpineTimeScale(float speedCoef)
        {
            if (skeletonAnimation == null)
            {
                return;
            }

            skeletonAnimation.timeScale = speedCoef * defaultSpineSpeed;
        }

        public async UniTask SetSkeletonDataAssetAsync(ResourcesLoader resourcesLoader, BattleConst.DigestType type, long mCharId, bool isOffence, bool isPlayer)
        {
            if (isNotSetSkeletonData)
            {
                return;
            }
            
            CharaMasterObject mChar = MasterManager.Instance.charaMaster.FindData(mCharId);
            // キャンセル
            Cancel();
            // トークン生成
            source = new CancellationTokenSource();

            var bAllowSwap = false;
            // スケルトンデータ読み込み
            string key = GetSpineAddress(type,　mChar.parentMCharaId, isOffence, isPlayer, out var rootPath, out var path);
            if (!string.IsNullOrEmpty(key))
            {
                await resourcesLoader.LoadAssetAsync<SkeletonDataAsset>(key, skeleton =>
                    {
                        skeletonAnimation.skeletonDataAsset = skeleton;
                        if(skeleton == null)return;
                        skeletonAnimation.ClearState();
                    },
                    source.Token
                );
                
                //Check swap file
                var swapPath = $"{rootPath}/{path}{(isPlayer?"":"_red")}.png";

                bAllowSwap = AddressablesManager.HasResources<Texture2D>(swapPath);
                if (bAllowSwap)
                {
                    var tex2D = default(Texture2D);
                    await resourcesLoader.LoadAssetAsync<Texture2D>(swapPath,
                        tex =>
                        {
                            if (tex == null) return;
                            tex2D = tex;
                            CruFramework.Logger.Log($"[SPINE] swap atlas loaded {swapPath}");
                        },
                        source.Token
                    );
                
                    //Create dummy texture
                    var originalMaterial = skeletonAnimation.SkeletonDataAsset.atlasAssets[0].PrimaryMaterial;
                    if (originalMaterial.mainTexture == null)
                    {
                        originalMaterial.mainTexture = new Texture2D(0,0);
                        originalMaterial.mainTexture.name = $"{path}";
                        _tempMainTex = originalMaterial.mainTexture;
                    }
                    //Create material override
                    var newmat = new Material(originalMaterial);
                    newmat.mainTexture = tex2D;
                    skeletonAnimation.CustomMaterialOverride.Add(originalMaterial, newmat);
                    _tempOverrideMat = newmat;
                }

                skeletonAnimation.Initialize(true);
            }

            if (!isPlayer && !bAllowSwap)
            {
                // プレイヤーでない場合はスキンを変更
                await ChangeSkin(resourcesLoader, type, mChar.SizeType, mChar.GetPersonalId() ,isOffence);
            }
        }

        private string GetSpineAddress(BattleConst.DigestType type, long id, bool isOffence, bool isPlayer, out string rootPath, out string path)
        {
            path = "";
            string folder = "";
            rootPath = default;

            switch (type)
            {
                case BattleConst.DigestType.MatchUp:
                    path = "pjfb100";
                    folder = "ShowdownlMatchup";
                    break;
                case BattleConst.DigestType.TechnicMatchUpLoseL:
                case BattleConst.DigestType.TechnicMatchUpLoseR:
                case BattleConst.DigestType.TechnicMatchUpWinL:
                case BattleConst.DigestType.TechnicMatchUpWinR:
                    path = "pjfb103";
                    folder = "TechniqueMatchup";
                    break;
                case BattleConst.DigestType.PhysicalMatchUpLoseL:
                case BattleConst.DigestType.PhysicalMatchUpLoseR:
                case BattleConst.DigestType.PhysicalMatchUpWinL:
                case BattleConst.DigestType.PhysicalMatchUpWinR:
                case BattleConst.DigestType.SpeedMatchUpLoseL:
                case BattleConst.DigestType.SpeedMatchUpLoseR:
                case BattleConst.DigestType.SpeedMatchUpWinL:
                case BattleConst.DigestType.SpeedMatchUpWinR:
                    path = "pjfb102";
                    folder = "PhysicalMatchup";
                    break;
                default:
                    return "";
            }
            
            // 上2桁目〜5桁目を抽出
            path += id.ToString().Substring(1, 4);

            // ポジション
            path += isOffence ? "a" : "b";
            
            // Spines/InGame/ShowdownlMatchup/pjfb1000001a/pjfb1000001a_SkeletonData.asset
            // Spines/InGame/TechniqueMatchup/pjfb1030001a/pjfb1030001a_SkeletonData.asset
            // Spines/InGame/PhysicalMatchup/pjfb1020009a/pjfb1020009a_SkeletonData.asset
            // Spines/InGame/000101_SkillAnim/pjfb10s0001a_SkeletonData.asset
            rootPath = $"Spines/InGame/{folder}/{path}";
            var address = $"{rootPath}/{path}_SkeletonData.asset";
            
            CruFramework.Logger.Log(address);
            return address;
        }
        
        private string GetSkinFolderAddress(BattleConst.DigestType type, SizeType sizeType, long personalId, bool isOffence)
        {
            string folder = "";
            string size = "";
            string side = "";

            switch (type)
            {
                case BattleConst.DigestType.DribbleL:
                case BattleConst.DigestType.DribbleR:
                    folder = "Dribble";
                    break;
                case BattleConst.DigestType.MatchUp:
                    folder = "ShowdownlMatchup";
                    side = isOffence ? "a" : "b";
                    size = CharaMasterObject.SizeToString(sizeType);
                    break;
                case BattleConst.DigestType.TechnicMatchUpLoseL:
                case BattleConst.DigestType.TechnicMatchUpLoseR:
                case BattleConst.DigestType.TechnicMatchUpWinL:
                case BattleConst.DigestType.TechnicMatchUpWinR:
                    folder = "TechniqueMatchup";
                    side = isOffence ? "a" : "b";
                    size = CharaMasterObject.SizeToString(sizeType);
                    break;
                case BattleConst.DigestType.PhysicalMatchUpLoseL:
                case BattleConst.DigestType.PhysicalMatchUpLoseR:
                case BattleConst.DigestType.PhysicalMatchUpWinL:
                case BattleConst.DigestType.PhysicalMatchUpWinR:
                case BattleConst.DigestType.SpeedMatchUpWinL:
                case BattleConst.DigestType.SpeedMatchUpWinR:
                case BattleConst.DigestType.SpeedMatchUpLoseL:
                case BattleConst.DigestType.SpeedMatchUpLoseR:
                    folder = "PhysicalMatchup";
                    side = isOffence ? "a" : "b";
                    size = CharaMasterObject.SizeToString(sizeType);
                    break;
                case BattleConst.DigestType.Cross:
                    folder = "Cross";
                    break;
                case BattleConst.DigestType.PassFailed:
                case BattleConst.DigestType.PassSuccess:
                    folder = "Short-pass";
                    break;
                case BattleConst.DigestType.PassCutBlock:
                case BattleConst.DigestType.PassCutCatch:
                    folder = "Pass-cut";
                    break;
                case BattleConst.DigestType.Shoot:
                    folder = "Shoot";
                    break;
                case BattleConst.DigestType.ShootBlockL:
                case BattleConst.DigestType.ShootBlockR:
                case BattleConst.DigestType.ShootBlockTouchL:
                case BattleConst.DigestType.ShootBlockTouchR:
                case BattleConst.DigestType.ShootBlockNotReachL:
                case BattleConst.DigestType.ShootBlockNotReachR:
                    folder = "Shoot-block";
                    break;
                case BattleConst.DigestType.Special:
                    folder = $"{personalId:D4}01_SkillAnim";
                    break;
                default: 
                    CruFramework.Logger.LogError("不正なタイプが指定されています");
                    break;
            }

            // Spines/InGame/Dribble/Bibs/01/A_Bibs_Body.png
            // Spines/InGame/ShowdownlMatchup/Bibs/01/b/M/M_Bibs_Body.png
            // Spines/InGame/TechniqueMatchup/Bibs/01/b/M/M_Bibs_Body.png
            // Spines/InGame/ShowdownlMatchup/Bibs/01/b/M/M_Bibs_Body.png",
            // Spines/InGame/Cross/Bibs/01/B_Bibs_Body.png
            // Spines/InGame/Short-pass/Bibs/01/A_Bibs_Body.png
            // Spines/InGame/Pass-cut/Bibs/01/Bibs_Body.png
            // Spines/InGame/Shoot/Bibs/01/Bibs_Body.png
            // Spines/InGame/Shoot-block/Bibs/01/Bibs_Body.png
            // Spines/InGame/000601_SkillAnim/Bibs/01/a/Bibs_Body.png

            var address = $"Spines/InGame/{folder}/Bibs/01/";
            if (!string.IsNullOrEmpty(side))
            {
                address += $"{side}/";
            }
            if (!string.IsNullOrEmpty(size))
            {
                address += $"{size}/";
            }
            if (!string.IsNullOrEmpty(customSkinIdentifier))
            {
                address += $"{customSkinIdentifier}/";
            }
            
            return address;
        }

        private List<string> GetSkinList(BattleConst.DigestType type, SizeType sizeType, bool isOffence)
        {
            List<string> skinList = new List<string>();
            
            string size = CharaMasterObject.SizeToString(sizeType);
            
            switch (type)
            {
                case BattleConst.DigestType.DribbleL:
                case BattleConst.DigestType.DribbleR:
                    skinList.Add("A_Bibs_Body");
                    skinList.Add("A_Bibs_Hips");
                    skinList.Add("A_Bibs_LeftLeg");
                    skinList.Add("A_Bibs_RightLeg");
                    skinList.Add("B_Bibs_Body");
                    skinList.Add("B_Bibs_Hips");
                    skinList.Add("B_Bibs_LeftLeg");
                    skinList.Add("B_Bibs_RightLeg");
                    skinList.Add("C_Bibs_Body");
                    skinList.Add("C_Bibs_BodyBack");
                    skinList.Add("C_Bibs_Hips");
                    skinList.Add("C_Bibs_LeftLeg");
                    skinList.Add("C_Bibs_RightLeg");
                    break;
                case BattleConst.DigestType.MatchUp:
                    if (isOffence)
                    {
                        skinList.Add($"{size}_Bibs_Body");
                        skinList.Add($"{size}_Bibs_LeftLeg");
                        skinList.Add($"{size}_Bibs_RightLeg");
                    }
                    else
                    {
                        skinList.Add($"{size}_Bibs_Body");
                    }
                    break;
                case BattleConst.DigestType.TechnicMatchUpLoseL:
                case BattleConst.DigestType.TechnicMatchUpLoseR:
                case BattleConst.DigestType.TechnicMatchUpWinL:
                case BattleConst.DigestType.TechnicMatchUpWinR:
                    if (isOffence)
                    {
                        skinList.Add($"{size}_Bibs_Body_BK");
                        skinList.Add($"{size}_Bibs_Body");
                        skinList.Add($"{size}_Bibs_Hips");
                        skinList.Add($"{size}_Bibs_LeftLeg");
                        skinList.Add($"{size}_Bibs_RightLeg");
                    }
                    else
                    {
                        skinList.Add($"{size}_Bibs_Body");
                        skinList.Add($"{size}_Bibs_LeftLeg_BK");
                        skinList.Add($"{size}_Bibs_LeftLeg");
                        skinList.Add($"{size}_Bibs_RightLeg");
                    }
                    break;
                case BattleConst.DigestType.PhysicalMatchUpLoseL:
                case BattleConst.DigestType.PhysicalMatchUpLoseR:
                case BattleConst.DigestType.PhysicalMatchUpWinL:
                case BattleConst.DigestType.PhysicalMatchUpWinR:
                case BattleConst.DigestType.SpeedMatchUpLoseL:
                case BattleConst.DigestType.SpeedMatchUpLoseR:
                case BattleConst.DigestType.SpeedMatchUpWinL:
                case BattleConst.DigestType.SpeedMatchUpWinR:
                    if (isOffence)
                    {
                        skinList.Add($"{size}_Bibs_Body_B1");
                        skinList.Add($"{size}_Bibs_Body_B2");
                        skinList.Add($"{size}_Bibs_Body");
                        skinList.Add($"{size}_Bibs_Hips");
                        skinList.Add($"{size}_Bibs_LeftLeg");
                        skinList.Add($"{size}_Bibs_RightLeg");
                    }
                    else
                    {
                        skinList.Add($"{size}_Bibs_Body_B1");
                        skinList.Add($"{size}_Bibs_Body_B2");
                        skinList.Add($"{size}_Bibs_Body");
                        skinList.Add($"{size}_Bibs_LeftLeg");
                        skinList.Add($"{size}_Bibs_RightLeg");
                    }
                    break;
                case BattleConst.DigestType.Cross:
                    skinList.Add("B_Bibs_Body");
                    skinList.Add("B_Bibs_RightLeg");
                    skinList.Add("Bibs_Body");
                    skinList.Add("Bibs_LeftLeg");
                    break;
                case BattleConst.DigestType.PassFailed:
                case BattleConst.DigestType.PassSuccess:
                    skinList.Add("A_Bibs_Body_bk");
                    skinList.Add("A_Bibs_Body");
                    skinList.Add("A_Bibs_LeftLeg");
                    skinList.Add("B_Bibs_Body");
                    skinList.Add("B_Bibs_RightLeg");
                    break;
                case BattleConst.DigestType.PassCutBlock:
                case BattleConst.DigestType.PassCutCatch:
                    skinList.Add("Bibs_Body_bk");
                    skinList.Add("Bibs_Body");
                    skinList.Add("Bibs_LeftLeg");
                    break;
                case BattleConst.DigestType.Shoot:
                    skinList.Add("Bibs_Body");
                    skinList.Add("Bibs_Hips_A");
                    skinList.Add("Bibs_Hips_B");
                    skinList.Add("Bibs_LeftLeg");
                    skinList.Add("Bibs_RightLeg_A");
                    skinList.Add("Bibs_RightLeg_B");
                    break;
                case BattleConst.DigestType.ShootBlockL:
                case BattleConst.DigestType.ShootBlockR:
                case BattleConst.DigestType.ShootBlockTouchL:
                case BattleConst.DigestType.ShootBlockTouchR:
                case BattleConst.DigestType.ShootBlockNotReachL:
                case BattleConst.DigestType.ShootBlockNotReachR:
                    skinList.Add("Bibs_BodyA");
                    skinList.Add("Bibs_BodyB");
                    skinList.Add("Bibs_BodyC");
                    skinList.Add("Bibs_BodyD");
                    skinList.Add("Bibs_RightLeg");
                    break;
                case BattleConst.DigestType.Special:
                    foreach (var slot in customSkinSlot)
                    {
                        skinList.Add(slot);
                    }
                    break;
                default: 
                    CruFramework.Logger.LogError("不正なタイプが指定されています");
                    return skinList;
            }

            return skinList;
        }

        private async UniTask ChangeSkin(ResourcesLoader resourcesLoader, BattleConst.DigestType type, SizeType sizeType, long personalId, bool isOffence)
        {
            string folderAddress = GetSkinFolderAddress(type,　sizeType, personalId, isOffence);
            
            Skeleton skeleton = skeletonAnimation.Skeleton;
            // スキン
            Skin customSkin = new Skin("CustomSkin");

            // 元のスキン取得
            Skin tempSkin = skeleton.Data.DefaultSkin;
            customSkin.AddSkin(skeleton.Data.DefaultSkin);
     
            // 元のマテリアル取得
            Material sourceMaterial = skeletonAnimation.SkeletonDataAsset.atlasAssets[0].PrimaryMaterial;
            
            // スキンリスト取得
            List<string> skinList = GetSkinList(type, sizeType, isOffence);

            // 設定されているアタッチメントを処理
            foreach(string skin in skinList)
            {
                // スロット
                var slot = skeleton.Data.FindSlot(skin);
                // 取得できない
                if(slot == null)continue;
                
                // Attachment
                Attachment tempAttachment = tempSkin.GetAttachment(slot.Index, skin);
                // 取得できない
                if(tempAttachment == null)continue;

                var key = $"{folderAddress}{skin}.png";
                CruFramework.Logger.Log(key);
                await resourcesLoader.LoadAssetAsync<Sprite>(key,
                    sprite =>
                    {
                        if(sprite == null)return;
                        // アタッチメントの生成　
                        Attachment newAttachment = tempAttachment.GetRemappedClone(sprite, sourceMaterial, premultiplyAlpha: true);
                        // スキンにセット
                        customSkin.SetAttachment(slot.Index, skin, newAttachment);
                    },
                    source.Token
                );

            }

            // Runtime atlas repacking
            var atlasName = $"repacked_{this.name}";
            var repackedSkin = new Skin(atlasName);
            repackedSkin.AddSkin(customSkin);
            repackedSkin = repackedSkin.GetRepackedSkin(atlasName, skeletonAnimation.SkeletonDataAsset.atlasAssets[0].PrimaryMaterial, 
                out var repackedMat, out _repackedTex);
            _repackedTex.Compress(true);
            AtlasUtilities.ClearOriginRegionTexturesCache();
            // スケルトンに適用
            skeleton.SetSkin(repackedSkin);

            // アニメーション初期化
            skeleton.SetSlotsToSetupPose();
            skeletonAnimation.AnimationState.Apply(skeletonAnimation.Skeleton);
            
        }

        /// <summary>キャンセル</summary>
        protected virtual void Cancel()
        {
            // キャンセル
            if(source != null)
            {
                source.Cancel();
                source.Dispose();
                source = null;
            }
        }

        protected virtual void OnDestroy()
        {
            if (_repackedTex != null)
            {
                Destroy(_repackedTex);
                _repackedTex = null;
            }
            Destroy(_tempMainTex);
            Destroy(_tempOverrideMat);
            _tempMainTex = null;
            _tempOverrideMat = null;
            AtlasUtilities.ClearCache();
            // ゲームオブジェクト削除時にキャンセル
            Cancel();
        }
    }
}