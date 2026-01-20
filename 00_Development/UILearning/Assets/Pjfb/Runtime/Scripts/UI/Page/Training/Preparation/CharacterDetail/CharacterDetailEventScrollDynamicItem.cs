using System.Collections.Generic;
using CruFramework.UI;
using Pjfb.Master;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{ 
    public class CharacterDetailEventScrollDynamicItem : ScrollDynamicItem
    {
        public class Param : CharacterDetailEventScrollDynamicItemSelector.IEventScrollDynamicItem
        {
            // キャラクターId
            private long mCharacterId;
            public long MCharacterId => mCharacterId;
            // キャラクターLv
            private long charaLevel;
            public long CharaLevel => charaLevel;
            // 表示するイベントスキルデータ
            private EventSkillData eventSkill;
            public EventSkillData EventSkill => eventSkill;
            // 発生するコンボタイプ
            private TrainingUnitComboType comboType;
            public TrainingUnitComboType ComboType => comboType;
            // 初期化完了済みか
            private bool isInitComplete = false;
            public bool IsInitComplete
            {
                get => isInitComplete;
                set => isInitComplete = value;
            }

            // プルダウンがアクティブか
            private bool isActivePullDown;
            public bool IsActivePullDown
            {
                get => isActivePullDown;
                set => isActivePullDown = value;
            }

            // 子要素のプルダウンのアクティブ状況
            private List<bool> elementActivePullDownList;
            public List<bool> ElementActivePullDownList => elementActivePullDownList;

            public Param(long mCharacterId, long charaLevel, EventSkillData eventSkill, TrainingUnitComboType comboType)
            {
                this.mCharacterId = mCharacterId;
                this.charaLevel = charaLevel;
                this.eventSkill = eventSkill;
                this.comboType = comboType;
                isInitComplete = false;
                isActivePullDown = false;
                elementActivePullDownList = new List<bool>();
            }
        }
        
        [SerializeField] private CharacterDetailEventView detailEventView;
        private Param param;
        
        protected override void OnSetView(object value)
        {
            param = (Param)value;
            detailEventView.SetEvent(param.MCharacterId, param.CharaLevel, param.EventSkill, param.ComboType, param.IsInitComplete, param.IsActivePullDown, param.ElementActivePullDownList, OnSetPullDownActive, OnChangeItemSize);
            // 初回セット完了したなら完了フラグを立てる
            if (param.IsInitComplete == false)
            {
                param.IsInitComplete = true;
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(UITransform);
        }

        //// <summary> アイテムのサイズが変更された時の処理 </summary>
        private void OnChangeItemSize()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(UITransform);
            // 再計算をする
            RecalculateSize();
        }

        //// <summary> プルダウンのアクティブをセット </summary>
        private void OnSetPullDownActive(bool isActive)
        {
            // プルダウンのアクティブ状況を更新
            param.IsActivePullDown = isActive;
        }
    }
}