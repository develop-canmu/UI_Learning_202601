#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT

using System;
using System.Collections.Generic;
using System.Linq;
using SRDebugger.UI.Controls;
using SRF;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.SRDebugger
{
    public class SRDebugDropDownEnumControl : DataBoundControl
    {
        [RequiredField] public Text Title;
        
        [SerializeField] private TMP_Dropdown dropdown;
        
        // ドロップダウンリストの格納番号毎のEnum値
        private List<Enum> dropdownIndexValue = new List<Enum>();

        // セットされているデータ
        private SRDebugDropDownEnumData data;
        
        protected override void OnBind(string propertyName, Type t)
        {
            base.OnBind(propertyName, t);

            Title.text = propertyName;
            dropdown.onValueChanged.RemoveListener(OnChangeDropDown);
            dropdown.onValueChanged.AddListener(OnChangeDropDown);

            List<TMP_Dropdown.OptionData> optionDataList = new List<TMP_Dropdown.OptionData>();
            data = (SRDebugDropDownEnumData) Property.GetValue();
            dropdownIndexValue.Clear();
            
            // 初期状態Index番号
            int defaultIndex = 0;
            
            // ドロップダウンリストを構築
            foreach (Enum value in Enum.GetValues(data.Value.GetType()))
            {
                if(data.HideTypes != null && data.HideTypes.Contains(Convert.ToInt32(value)))
                {
                    continue;
                }

                // 初期状態のドロップダウンの番号を保持する
                if (data.Value.Equals(value))
                {
                    defaultIndex = optionDataList.Count;
                }
                TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(value.ToString());
                optionDataList.Add(optionData);
                dropdownIndexValue.Add(value);
            }
            dropdown.options = optionDataList;
            
            // 初期状態をセット
            dropdown.SetValueWithoutNotify(defaultIndex);
        }

        //// <summary> ドロップダウン側で更新するので何もしない </summary>
        protected override void OnValueUpdated(object newValue)
        {
        }

        public override bool CanBind(Type type, bool isReadOnly)
        {
            return type == typeof(SRDebugDropDownEnumData) && isReadOnly == false;
        }

        //// <summary> ドロップダウン変更時のイベント </summary>
        public void OnChangeDropDown(int index)
        {
            // データを更新
            data = new SRDebugDropDownEnumData(dropdownIndexValue[index]);
            UpdateValue(data);
            Refresh();
        }

        public override void Refresh()
        {
            // 現在の値の状態に応じてドロップダウンを更新する
            for (int i = 0; i < dropdownIndexValue.Count; i++)
            {
                // 変更されたデータに応じてドロップダウンの値をセット
                if (dropdownIndexValue[i].Equals(data.Value))
                {
                    dropdown.SetValueWithoutNotify(i); 
                    return;
                }
            }
        }
    }
}
#endif