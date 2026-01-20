using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Training
{
    /// <summary>スペシャルトレーニングのView</summary>
    public class TrainingSpecialTrainingMembersView : MonoBehaviour
    {
        // スペシャルレクチャー参加者アイコン格納用配列
        [SerializeField]
        private CharacterIcon[] memberIcons = new CharacterIcon[3];
        // スペシャルレクチャー参加表示テキスト
        [SerializeField]
        private TextMeshProUGUI titleText = null;
        // スペシャルレクチャー参加表示テキストの下線
        [SerializeField]
        private Image titleLineImage = null;
        // 選出された育成選手の練習メニューカード時のスペシャルレクチャー参加表示テキストの色
        [SerializeField]
        [ColorValue] 
        private string selectedSpecialLectureTextColorKey = string.Empty;
        // カードユニオン対象の練習メニューカード時のスペシャルレクチャー参加表示テキストの色
        [SerializeField]
        [ColorValue] 
        private string unionSpecialLectureTextColorKey = string.Empty;
        
        /// <summary>スペシャルトレーニング参加者の表示</summary>
        public void SetSpecialTrainingMembers(List<long> memberCharaIds)
        {
            // 3人分のアイコンを設定
            for (int i = 0; i < memberIcons.Length; i++)
            {
                // データがある場合は表示
                if (i < memberCharaIds.Count)
                {
                    memberIcons[i].gameObject.SetActive(true);
                    memberIcons[i].SetIconTextureWithEffectAsync(memberCharaIds[i]).Forget();
                }
                // データがない場合は非表示
                else
                {
                    memberIcons[i].gameObject.SetActive(false);
                }
            }
        }
        
        /// <summary>スペシャルレクチャー参加表示テキストと下線(ライン)の色をセット</summary>
        public void SetSpecialLectureTitleColor(bool isSelected)
        {
            // スペシャルレクチャー参加表示テキストと下線(ライン)の色を変更
            if (isSelected)
            {
                titleText.color = ColorValueAssetLoader.Instance[selectedSpecialLectureTextColorKey];
                titleLineImage.color = ColorValueAssetLoader.Instance[selectedSpecialLectureTextColorKey];
            }
            else
            {
                titleText.color = ColorValueAssetLoader.Instance[unionSpecialLectureTextColorKey];
                titleLineImage.color = ColorValueAssetLoader.Instance[unionSpecialLectureTextColorKey];
            }
        }
    }
}

