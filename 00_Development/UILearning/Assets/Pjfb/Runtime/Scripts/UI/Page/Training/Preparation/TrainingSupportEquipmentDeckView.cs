using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Page;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

using CruFramework;
using CruFramework.UI;
using Pjfb.Networking.App.Request;

using System;
using System.Linq;
using Pjfb.Character;
using Pjfb.Combination;
using Pjfb.Master;
using Pjfb.UserData;
using TMPro;

namespace Pjfb.Training
{
    public class TrainingSupportEquipmentDeckView : TrainingSupportDeckView
    {
        [SerializeField]
        private ScrollGrid equipmentSkillScroll = null;
        
        [SerializeField]
        private RectTransform equipmentIconParentRectTransform = null;
        
        [SerializeField]
        private DeckSupportEquipmentScrollItem iconPrefab = null;
        
        private List<DeckSupportEquipmentScrollItem> iconList = new List<DeckSupportEquipmentScrollItem>();

        private List<DeckSupportEquipmentScrollData> equipmentScrollDataList = new ();
        
        private bool isInitilized = false;
        
        private void OnSelectedEquipment(int order)
        {
            CallOnSelect(order, TrainingDeckMemberType.Equipment, false);
        }

        /// <summary>タイプ数表示更新</summary>
        protected override void OnUpdateTypeCount(Dictionary<CharacterType, int> typeCount)
        {
            // サポート器具
            foreach (DeckSupportEquipmentScrollData deckSupportEquipmentData in equipmentScrollDataList)
            {
                typeCount[deckSupportEquipmentData.CharType]++;
            }
        }

        protected override void OnUpdateView()
        {
            if (isInitilized == false)
            {
                int order = 0;
                foreach (DeckFormatSlotMasterObject slotMasterObject in DeckUtility.GetSlotMaster(DeckType.SupportEquipment).OrderBy(data => data.index))
                {
                    DeckSupportEquipmentScrollData scrollData = new DeckSupportEquipmentScrollData(order, slotMasterObject.mDeckFormatConditionId > 0, OnSelectedEquipment);
                    equipmentScrollDataList.Add(scrollData);
                    // アイコン用オブジェクトの生成
                    iconList.Add(GameObject.Instantiate(iconPrefab, equipmentIconParentRectTransform));
                    iconList[order].gameObject.SetActive(true);
                    order++;
                }
                isInitilized = true;
            }
            
            Dictionary<long, long> characterTupleDic = new Dictionary<long, long>();
            List<TrainingCharacterData> characterDataList = new List<TrainingCharacterData>();
            
            List<SupportEquipmentDetailData> equipmentDetailOrderList = new List<SupportEquipmentDetailData>();
            
            UserDataChara trainingChar = UserDataManager.Instance.chara.Find(arguments.TrainingUCharId);
            
            // サポート器具
            long[] equipmentIds = DeckData.GetMemberIds(DeckSlotCardType.SupportEquipment);
            long[] index = DeckUtility.GetCharacterIndex(DeckType.SupportEquipment, DeckSlotCardType.SupportEquipment);
            for(int i=0;i<equipmentScrollDataList.Count;i++)
            {
                
                long id = equipmentIds[i];
                
                // コンテンツ自体が未開放の場合
                if(SupportEquipmentManager.IsUnLockSupportEquipment() == false)
                {
                    equipmentScrollDataList[i].SupportEquipmentLockId = SupportEquipmentManager.SupportEquipmentLockId;
                    continue;
                }
                
                // レベル制限
                long unlocklLevel = DeckData.GetUnlockLevel(index[i]);
                // トレーニングキャラのレベルと比較してロックを設定
                if(unlocklLevel <= trainingChar.level)
                {
                    // Id
                    equipmentScrollDataList[i].Id = id;
                    equipmentScrollDataList[i].LockLevel = 0;
                }
                else
                {
                    equipmentScrollDataList[i].Id = DeckUtility.EmptyDeckSlotId;
                    equipmentScrollDataList[i].LockLevel = unlocklLevel;
                }
                
                if(id != DeckUtility.EmptyDeckSlotId)
                {
                    UserDataSupportEquipment uEuqipment = UserDataManager.Instance.supportEquipment.Find(id);
                    // キャラデータ
                    characterDataList.Add( new TrainingCharacterData( uEuqipment.MChara.id, uEuqipment.level, 0, uEuqipment.id) );
                    // 重複しないようにリストを作成
                    characterTupleDic[uEuqipment.MChara.id] = uEuqipment.level;
                    
                    equipmentScrollDataList[i].DetailOrderList = new SwipeableParams<SupportEquipmentDetailData>(equipmentDetailOrderList, equipmentDetailOrderList.Count, null);
                    
                    if(unlocklLevel <= trainingChar.level)
                    {
                        equipmentDetailOrderList.Add( new SupportEquipmentDetailData(uEuqipment) );
                    }
                }
            }
            
            // アイコンの初期化
            for(int i = 0; i < equipmentScrollDataList.Count; i++)
            {
                iconList[i].SetData(equipmentScrollDataList[i]);
            }
            
            // サポート器具スキル
            UpdateEquipmentSkillView(destroyCancellationToken).Forget();
            
            characterTupleDictionary = characterTupleDic;
            // キャラ配列
            characterDatas = characterDataList.ToArray();
        }
        
        private async UniTask UpdateEquipmentSkillView(CancellationToken token)
        {
            
            
            UserDataChara trainingUChar = UserDataManager.Instance.chara.Find(arguments.TrainingUCharId);
            
            List<PracticeSkillInfo> skillList = new List<PracticeSkillInfo>();
            // サポート器具
            long[] equipmentIds = DeckData.GetMemberIds(DeckSlotCardType.SupportEquipment);
            for(int i=0;i<equipmentScrollDataList.Count;i++)
            {
                long id = equipmentIds[i];
                if(id == DeckUtility.EmptyDeckSlotId)continue;
                // 開放チェック
                if(DeckData.GetUnlockLevel(i) > trainingUChar.level)continue;
                // uEquip
                UserDataSupportEquipment uEquipment = UserDataManager.Instance.supportEquipment.Find(id);

                // スキル
                skillList.AddRange(PracticeSkillUtility.GetCharacterPracticeSkill(uEquipment.charaId));
                skillList.AddRange(PracticeSkillUtility.GetCharaTrainerLotteryStatusPracticeSkill(uEquipment.lotteryProcessJson.statusList));
            }
            
            // レイアウト更新を待つ
            await UniTask.Yield(token);
            equipmentSkillScroll.SetItems(skillList);
        }

        private void OnDestroy()
        {
            if (iconList != null)
            {
                foreach (DeckSupportEquipmentScrollItem item in iconList)
                {
                    Destroy(item.gameObject);
                }
                iconList = null;
            }
        }
    }
}