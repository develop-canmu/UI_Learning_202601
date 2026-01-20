using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Character;
using Pjfb.Community;
using Pjfb.Master;
using TMPro;

namespace Pjfb
{
    // 編成効果のView
    public class TrainingEnhanceDeckEffectView : TrainingEnhanceDeckBaseView
    {
        [SerializeField] private ScrollDynamic buffScroller;

        // 強化対象の表示を切り替えるドロップダウン
        [SerializeField] private DropDownUI enhanceTargetDropDown;
        
        // 文言表示用テキスト(バフがないときやLv最大時に使う)
        [SerializeField] private TMP_Text annotationText;

        private List<BuffTargetData> buffList;
        private List<TrainingDeckSlotData> slotTargetList;
        
        // 初期化
        public void Initialize()
        {
            SetEnhanceDropDown();
        }
        
        public void UpdateView(long currentLv, long afterLv, TrainingDeckEnhanceListData enhanceListData)
        {
            buffList = new List<BuffTargetData>();
            
            // 表示順を並べるためにデータを格納
            TrainingEnhanceDeckBaseView.Param param = new TrainingEnhanceDeckBaseView.Param()
            { 
                growthTargetList = TrainingDeckEnhanceUtility.GetBuffTargetTotalEnhanceData(DeckType.GrowthTarget, currentLv, afterLv, enhanceListData.GrowthTargetEnhanceList),
                trainingList = TrainingDeckEnhanceUtility.GetBuffTargetTotalEnhanceData(DeckType.Training, currentLv, afterLv, enhanceListData.TrainingTargetEnhanceList),
                equipmentList = TrainingDeckEnhanceUtility.GetBuffTargetTotalEnhanceData(DeckType.SupportEquipment, currentLv, afterLv, enhanceListData.SupportEquipmentEnhanceList),
                friendList = TrainingDeckEnhanceUtility.GetBuffTargetTotalEnhanceData(DeckType.SupportFriend, currentLv, afterLv, enhanceListData.FriendEnhanceList),
            };
            
            // 表示順に並べる
            SortBuffTargetType(param, buffList);
            
            // バフの表示
            SetBuffListView();
        }

        //// <summary> バフの表示を行う </summary>
        private void SetBuffListView()
        {
            List<BuffTargetData> filterModifyBuffList = new List<BuffTargetData>();

            int dropDownIndex = enhanceTargetDropDown.value;
            
            // 初期値は編成全体なのでフィルタをかけない
            if (dropDownIndex == 0)
            {
                filterModifyBuffList = buffList;
            }
            else
            {
                TrainingDeckSlotData slotData = slotTargetList[dropDownIndex - 1];
                filterModifyBuffList = buffList.Where(x => x.DeckSlotIndex == slotData.Index && x.DeckType == slotData.SlotType).ToList();
            }
            
            bool isEmpty = filterModifyBuffList.Count == 0;
            // なにもバフが発生しないなら表示
            annotationText.gameObject.SetActive(isEmpty);
            
            buffScroller.SetItems(filterModifyBuffList);
        }

        //// <summary> ドロップダウンリストに表示するデータの設定 </summary>
        private void SetEnhanceDropDown()
        {
            // 初期値に設定
            enhanceTargetDropDown.SetValueWithoutNotify(0);
            // ドロップダウン変更時の処理を登録
            enhanceTargetDropDown.onValueChanged.RemoveListener(OnChangeEnhanceTargetDropDown);
            enhanceTargetDropDown.onValueChanged.AddListener(OnChangeEnhanceTargetDropDown);
            
            slotTargetList = TrainingDeckEnhanceUtility.GetEnhanceTargetList();
            List<TMP_Dropdown.OptionData> optionDataList = new List<TMP_Dropdown.OptionData>();

            // 編成全体の文言を追加
            optionDataList.Add(new TMP_Dropdown.OptionData(StringValueAssetLoader.Instance["training.deckEnhance.enhanceTarget.All"]));
            
            foreach (TrainingDeckSlotData slotData in slotTargetList)
            {
                string text;
                
                // 育成対象選手は枠数表示がないので固定
                if (slotData.SlotType == TrainingDeckSlotType.GrowthTarget)
                {
                    text = StringValueAssetLoader.Instance["training.deckEnhance.enhanceTarget.GrowthTarget"];
                }
                // 枠番号によってテキストを設定
                else
                {
                    text = string.Format(StringValueAssetLoader.Instance[$"training.deckEnhance.enhanceTarget.{slotData.SlotType.ToString()}"], slotData.Index);
                } 
                
                TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(text);
                optionDataList.Add(optionData);   
            }
            enhanceTargetDropDown.options = optionDataList;
        }

        //// <summary> ドロップダウンを変更したときの処理 </summary>
        private void OnChangeEnhanceTargetDropDown(int index)
        {
            SetBuffListView();
        }
    }
}