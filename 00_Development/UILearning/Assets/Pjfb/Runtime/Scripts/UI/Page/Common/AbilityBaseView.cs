using System;
using System.Collections.Generic;
using Pjfb.Master;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{
    /// <summary> アビリティ表示用ベースクラス </summary>
    public class AbilityBaseView : MonoBehaviour
    {
        // アビリティスキルの下地画像(レアリティに応じて切り替え)
        [SerializeField]   
        private CharacterSkillRarityBaseImage rarityImage = null;
        
        // スキルイメージ画像
        [SerializeField]
        protected CancellableImageWithId skillImage = null;
        
        // スキル名表示
        [SerializeField]
        protected TMP_Text skillName = null;

        // スキルレベル
        [SerializeField]
        protected TMP_Text skillLevel = null;
        
        // ロックオブジェクト
        [SerializeField]
        protected GameObject lockObject = null;

        // グレーアウト用のイメージ
        [SerializeField]
        protected Image lockMaskImage = null;
        
        // ロック時のテキスト
        [SerializeField]
        protected TMP_Text lockText = null;
        
        // 選択可能か(選択時の処理を実行できるか)
        [SerializeField]
        protected bool canSelect = true;
        
        // 通常時の文字色
        [SerializeField]
        [ColorValue] 
        private string normalColorId = String.Empty;

        // レベルアップ時の文字色
        [SerializeField]
        [ColorValue]
        private string levelUpColorId = string.Empty;

        // レアリティに応じて下地を変更するか
        [SerializeField]
        protected bool isChangeRarityBackImage = true;
        
        // ロック表示するか
        [SerializeField]
        protected bool isLockShow = true;
        
        public void SetView(CharaAbilityInfo abilityInfo, bool isLevelUp)
        {
            AbilityMasterObject master = abilityInfo.GetAbilityMaster();
            // スキルイメージの設定
            skillImage.SetTexture(master.iconId);
            // スキル名設定
            skillName.text = master.name;
            // スキルレベルの表示(最大レベルかに応じて表示切り替え)
            skillLevel.text =  string.Format(StringValueAssetLoader.Instance[(abilityInfo.SkillLevel == master.maxLevel) ? "character.max_level" : "character.status.lv_value"], abilityInfo.SkillLevel);
            // レベルアップ時の文字色の変更
            skillLevel.color = ColorValueAssetLoader.Instance[isLevelUp ? levelUpColorId : normalColorId];
            
            // スキルのレアリティ下地をセット
            SetAbilityBackImage(master.rarity);

            // ロックオブジェクトの表示が必要ならセット
            if (lockObject != null)
            {
                // アンロック表示するか(ロック表示フラグとデータから判定)
                bool isAbilityUnlock = isLockShow == false || abilityInfo.IsAbilityUnLock;
                lockObject.SetActive(isAbilityUnlock == false);
                
                // アンロック済みなら解放条件のテキストのセットは無視
                if (isAbilityUnlock)
                {
                    return;
                }
                
                string lockText = string.Empty;
                
                List<string> conditionTexts = new List<string>();

                // キャラのレベルでの解放条件があるか？
                if (abilityInfo.OpenCharaLevel > 0)
                {
                    lockText = string.Format(StringValueAssetLoader.Instance["character.base_chara_growth_liberation.growth_level"], abilityInfo.OpenCharaLevel);
                    conditionTexts.Add(lockText);
                }

                // 能力解放レベルでの解放条件があるか？
                if (abilityInfo.OpenLiberationLevel > 0)
                {
                    lockText = string.Format(StringValueAssetLoader.Instance["character.base_chara_growth_liberation.liberation_level"], abilityInfo.OpenLiberationLevel);
                    conditionTexts.Add(lockText);
                }

                // 複数条件の場合は条件ごとに区切る
                if (conditionTexts.Count > 1)
                {
                    lockText = string.Join(",", conditionTexts);
                }

                this.lockText.text = string.Format(StringValueAssetLoader.Instance["ability.release_condition"], lockText);
            }
        }

        /// <summary> レアリティ毎の下地をセット </summary>
        private void SetAbilityBackImage(long rarity)
        {
            // 下地画像が設定されていない,レアリティが設定されていないなら何もしない
            if (rarityImage == null || rarity <= 0)
            {
                return;
            }
            
            // レアリティに応じてイメージ変更するならセットイメージを取得
            if (isChangeRarityBackImage)
            {
                rarityImage.SetTexture(rarity);
            }
            
            // ロック用マスクイメージをベース画像に合わせる(レアリティに応じて画像の作りが違いサイズが一致しないので)
            lockMaskImage.sprite = rarityImage.GetSprite();
        }
        
        /// <summary> UGUI </summary>
        /// <summary> 選択時の処理 </summary>
        public void OnSelect()
        {
            // 選択不可の場合は無視する
            if (canSelect == false)
            {
                return;
            }
            
            SelectAbility();
        }

        /// <summary> アビリティ選択時の処理 </summary>
        protected virtual void SelectAbility()
        {
            
        }
    }
}