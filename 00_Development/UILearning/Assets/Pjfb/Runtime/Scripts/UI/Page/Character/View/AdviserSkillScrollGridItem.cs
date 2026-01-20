using CruFramework.UI;
using UnityEngine;

namespace Pjfb.Character
{
    public class AdviserSkillScrollGridItem : ScrollGridItem
    {
        public class Param
        {
            private CharaAbilityInfo info;
            public CharaAbilityInfo Info => info;

            // キャラのレベル
            private long charaLevel = 0;
            public long CharaLevel => charaLevel;

            // 解放レベル
            private long liberationLevel = 0;
            public long LiberationLevel => liberationLevel;
            
            private bool isLevelUp = false;
            public bool IsLevelUp => isLevelUp;

            public Param(CharaAbilityInfo info, long charaLevel, long liberationLevel, bool isLevelUp)
            {
                this.info = info;
                this.charaLevel = charaLevel;
                this.liberationLevel = liberationLevel;
                this.isLevelUp = isLevelUp;
            }
        }

        [SerializeField]
        private AdviserSkillView skillView;

        // 選択時のフレームオブジェクト
        [SerializeField]
        private GameObject selectFrame = null;

        // 選択フレームを表示するか
        [SerializeField]
        private bool isShowSelectFrame = false;
        
        protected override void OnSetView(object value)
        {
           Param param = (Param)value;
           skillView.SetSkillView(param.Info, param.CharaLevel, param.LiberationLevel, param.IsLevelUp);
        }

        /// <summary> アイテム選択時の処理 </summary>
        protected override void OnSelectedItem()
        {
            // 選択フレームの表示をしないならアクティブにはしない
            if (isShowSelectFrame == false)
            {
                return;
            }
            selectFrame.SetActive(true);
        }

        /// <summary> アイテムが非選択になった時の処理 </summary>
        protected override void OnDeselectItem()
        {
            selectFrame.SetActive(false);
        }
    }
}