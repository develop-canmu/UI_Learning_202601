using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using UnityEngine;

using TMPro;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Training;
using Pjfb.UserData;

namespace Pjfb
{
    public class TrainingMemberView : MonoBehaviour
    {
        
        [System.Serializable]
        private class CharacterIconData
        {
            [SerializeField]
            public CharacterIcon icon = null;
            [SerializeField]
            public GameObject specialAttack = null;
        }
        
        [SerializeField]
        private TMP_Text characterNameText = null;
        [SerializeField]
        private TMP_Text characterNickNameText = null;
        
        [SerializeField]
        private CharacterIconData characterIcon = null;
        
        [SerializeField]
        private CharacterIconData[] supportCharacterIcons = null;
        
        [SerializeField]
        private ScrollGrid specialSupportCharacterScrollGrid = null;
        
        [SerializeField]
        private ScrollGrid extraSupportCharacterScrollGrid = null;
        
        [SerializeField]
        private SupportEquipmentIconItemParent supportEquipmentItem = null;

        [SerializeField]
        private ScrollGrid adviserScrollGrid = null;
        
        [SerializeField]
        private GameObject noSpecialSupportCardText = null;
        
        [SerializeField]
        private GameObject noExtraSupportCardText = null;

        [SerializeField]
        private GameObject noSupportEquipmentText = null;

        [SerializeField]
        private GameObject noAdviserText = null;
        
        [SerializeField]
        private CharacterIconData friendCharacterIcon = null;
        
        public Func<long> GetTrainingScenarioId;

        
        public void SetTrainingCharacter(long mCharId, long lv, long liberationLv, long trainingScenarioId, TrainingSupport[] supportCharacters)
        {
            List<CharacterDetailData> characterDetailOrderList = new();
            List<CharacterDetailData> specialSupportCardDetailOrderList = new();
            List<SupportEquipmentDetailData> equipmentDetailOrderList = new();
            List<CharacterDetailData> adviserDetailOrderList = new List<CharacterDetailData>();
            
            List<SupportCharacterIconScrollData> specialSupportCharacterScrollDataList = new();
            List<SupportCharacterIconScrollData> extraSupportCharacterScrollDataList = new();
            List<SupportEquipmentIconItemParent.SupportEquipmentIconItemData> equipmentDataList = new();
            List<SupportCharacterIconScrollData> adviserScrollDataList = new List<SupportCharacterIconScrollData>();
            
            // キャラマスタ
            CharaMasterObject mChar = MasterManager.Instance.charaMaster.FindData(mCharId);
            // 表示を更新
            characterNameText.text = mChar.name;
            characterNickNameText.text = mChar.nickname;
            
            // アイコン
            characterIcon.icon.SetIcon(mCharId, lv, liberationLv);
            characterIcon.icon.BaseCharaType = BaseCharacterType.TrainingCharacter;
            // Lv
            //characterIcon.SetLv(uChar.level);
            // 特攻
            characterIcon.specialAttack.SetActive( lv >= TrainingUtility.TrainingCharacterSupportJoinLv && CharacterUtility.IsTrainingScenarioSpAttackCharacter(mChar.id, lv, trainingScenarioId) );
            
            int supportIndex = 0;
            int specialSupportIndex = 0;
            int extraSupportIndex = 0;
            int equipmentIndex = 0;
            int detailIndex = 0;
            
            // サポカが枠数追加の際に並びが変になるのでクライアントでソート
            foreach(TrainingSupport s in supportCharacters.OrderBy(chara => MasterManager.Instance.charaMaster.FindData(chara.mCharaId).isExtraSupport == false))
            {
                // mChar
                CharaMasterObject mSupportChar = MasterManager.Instance.charaMaster.FindData(s.mCharaId);
                
                switch( (TrainingUtility.SupportCharacterType)s.supportType)
                {
                    case TrainingUtility.SupportCharacterType.Normal:
                    {
                        // アイコン
                        supportCharacterIcons[supportIndex].icon.SetIcon(s.mCharaId, s.level, s.newLiberationLevel);
                        supportCharacterIcons[supportIndex].icon.SwipeableParams = new SwipeableParams<CharacterDetailData>(characterDetailOrderList, supportIndex);
                        supportCharacterIcons[supportIndex].icon.GetTrainingScenarioId = () => GetTrainingScenarioId?.Invoke() ?? -1;
                        supportCharacterIcons[supportIndex].icon.BaseCharaType = BaseCharacterType.SupportCharacter;
                        // 特攻
                        supportCharacterIcons[supportIndex].specialAttack.SetActive( CharacterUtility.IsTrainingScenarioSpAttackCharacter(s.mCharaId, s.level, trainingScenarioId) );
                        supportIndex++;
                        characterDetailOrderList.Add(new CharacterDetailData(s.mCharaId, s.level, s.newLiberationLevel));
                        break;
                    }
                    
                    case TrainingUtility.SupportCharacterType.Special:
                    {
                        switch ((CardType) s.cardType)
                        {
                            // サポカ
                            case CardType.SpecialSupportCharacter:
                            {
                                SupportCharacterIconScrollData iconData = new SupportCharacterIconScrollData(s.mCharaId, s.level, s.newLiberationLevel,
                                    CharacterUtility.IsTrainingScenarioSpAttackCharacter(s.mCharaId, s.level, trainingScenarioId),
                                    new SwipeableParams<CharacterDetailData>(specialSupportCardDetailOrderList, detailIndex++));
                                if (mSupportChar.isExtraSupport)
                                {
                                    extraSupportCharacterScrollDataList.Add(iconData);
                                    extraSupportIndex++;
                                }
                                else
                                {
                                    specialSupportCharacterScrollDataList.Add(iconData);
                                    specialSupportIndex++;
                                }

                                specialSupportCardDetailOrderList.Add(new CharacterDetailData(s.mCharaId, s.level, s.newLiberationLevel));
                                break;
                            }
                            // アドバイザー
                            case CardType.Adviser:
                            {
                                // 特攻対象キャラクターか
                                bool isSpAttackCharacter = CharacterUtility.IsTrainingScenarioSpAttackCharacter(s.mCharaId, s.level, trainingScenarioId);
                                SwipeableParams<CharacterDetailData> swipeParams = new SwipeableParams<CharacterDetailData>(adviserDetailOrderList, adviserDetailOrderList.Count);
                                SupportCharacterIconScrollData adviserIconData = new SupportCharacterIconScrollData(s.mCharaId, s.level, s.newLiberationLevel, isSpAttackCharacter, swipeParams);
                                adviserDetailOrderList.Add(new CharacterDetailData(s.mCharaId, s.level, s.newLiberationLevel));
                                adviserScrollDataList.Add(adviserIconData);
                                break;
                            }
                        }
                        break;
                    }
                    
                    case TrainingUtility.SupportCharacterType.Friend:
                    {
                        // アイコン
                        friendCharacterIcon.icon.SetIcon(s.mCharaId, s.level, s.newLiberationLevel);
                        friendCharacterIcon.icon.SwipeableParams = new SwipeableParams<CharacterDetailData>(characterDetailOrderList, supportIndex);
                        friendCharacterIcon.icon.GetTrainingScenarioId = () => GetTrainingScenarioId?.Invoke() ?? -1;
                        friendCharacterIcon.icon.BaseCharaType = BaseCharacterType.SupportCharacter;
                        // 特攻
                        friendCharacterIcon.specialAttack.SetActive( CharacterUtility.IsTrainingScenarioSpAttackCharacter(s.mCharaId, s.level, trainingScenarioId) );

                        characterDetailOrderList.Add(new CharacterDetailData(s.mCharaId, s.level, s.newLiberationLevel));
                        break;
                    }
                    
                    case TrainingUtility.SupportCharacterType.Equipment:
                    {
                        SupportEquipmentDetailData detailData = new SupportEquipmentDetailData(s.mCharaId, s.level, s.statusIdList);
                        equipmentDetailOrderList.Add(detailData);
                        SwipeableParams<SupportEquipmentDetailData> swipeableParams = new SwipeableParams<SupportEquipmentDetailData>(equipmentDetailOrderList, equipmentIndex);
                        SupportEquipmentIconItemParent.SupportEquipmentIconItemData scrollData = new SupportEquipmentIconItemParent.SupportEquipmentIconItemData(detailData, swipeableParams);
                        equipmentDataList.Add(scrollData);
                        equipmentIndex++;
                        break;
                    }
                }
            }
            
            // スペシャルサポカ
            specialSupportCharacterScrollGrid.SetItems(specialSupportCharacterScrollDataList);
            // エクストラサポカ
            extraSupportCharacterScrollGrid.SetItems(extraSupportCharacterScrollDataList);
            // サポート器具
            supportEquipmentItem.CreateItem(equipmentDataList);
            // アドバイザー
            adviserScrollGrid.SetItems(adviserScrollDataList);
            
            // メンバーがいない場合のテキスト
            noExtraSupportCardText.SetActive(extraSupportIndex == 0);
            noSpecialSupportCardText.SetActive(specialSupportIndex == 0);
            noSupportEquipmentText.SetActive(equipmentIndex == 0);
            noAdviserText.SetActive(adviserScrollDataList.Count == 0);
         }
    }
}