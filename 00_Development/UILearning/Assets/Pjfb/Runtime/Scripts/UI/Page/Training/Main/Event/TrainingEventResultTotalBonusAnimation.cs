using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

namespace Pjfb
{
    public class TrainingEventResultTotalBonusAnimation : MonoBehaviour
    {
        private static string OpenAnimationKey = "Open";
        private static string IdleAnimationKey = "Idle";

        [SerializeField] private Animator animator;
        // ベースボーナス値テキスト
        [SerializeField] private TMP_Text baseBonusValueText;
        // エクストラボーナス値テキスト
        [SerializeField] private TMP_Text gradeUpBonusValueText;
        // ブーストボーナス倍率テキスト
        [SerializeField] private TMP_Text boostBonusRateText;

        // エクストラボーナスのルートオブジェクト(エクストラボーナスがない場合はルートを切る)
        [SerializeField] private GameObject gradeUpBonusRoot;

        // グレードアップのラベルテキスト
        [SerializeField] private TMP_Text gradeUpLabelText;
        
        // TotalBonusのそれぞれの桁の画像Image(格納順は１桁から昇順に)
        [SerializeField] private Image[] valueImages;

        // 残像演出用のそれぞれの桁の画像Image
        [SerializeField] private Image[] valueAfterImage;

        // 対象の桁に対するマスク画像
        [SerializeField] private Image[] maskValueImages;
        
        // 表示に使う数値画像
        [SerializeField] private Sprite[] numberSprites;
        
        // マスク数値画像
        [SerializeField] private Sprite[] numberMaskSprites;
        
        //// <summary> アニメーションの再生 </summary>
        public async UniTask PlayAnimationAsync(long baseBonusValue, long maxBonusValue, long boostBonusRate, long totalBonusValue, TrainingEventResultBonusAnimation.GradeUpType gradeUpType, Action onComplete)
        {
            gameObject.SetActive(true);
            
            // 表示するボーナス値の設定
            baseBonusValueText.text = string.Format(StringValueAssetLoader.Instance["common.percent_value"], baseBonusValue);
            gradeUpBonusValueText.text = string.Format(StringValueAssetLoader.Instance["common.percent_value"],maxBonusValue);
            boostBonusRateText.text = string.Format(StringValueAssetLoader.Instance["training.event_result.boost_bonus_rate"], boostBonusRate);
            
            // Grade上昇がない場合は非表示
            gradeUpBonusRoot.SetActive(gradeUpType != TrainingEventResultBonusAnimation.GradeUpType.None);
            
            // グレードアップタイプによって表示ラベルのテキストを変える 
            if (gradeUpType == TrainingEventResultBonusAnimation.GradeUpType.ExtraBonus)
            {
                gradeUpLabelText.text = StringValueAssetLoader.Instance["training.total.bonus.label.extraBonus"];
            }
            else if (gradeUpType == TrainingEventResultBonusAnimation.GradeUpType.LimitBreak)
            {
                gradeUpLabelText.text = StringValueAssetLoader.Instance["training.total.bonus.label.limitBreak"];
            }
            
            // 桁に表示する数値をセット
            SetTotalBonusValue(totalBonusValue);
            
            // アニメーションを再生する
            animator.SetTrigger(OpenAnimationKey);
            // アイドル状態にStateがなるまで待つ
            await AnimatorUtility.WaitStateChangeAsync(animator, IdleAnimationKey);

            onComplete();
        }

        //// <summary> それぞれの桁に数値を設定する </summary>
        private void SetTotalBonusValue(long value)
        {
            // 桁の表示を隠すか
            bool isValueHide = false;
            
            for (int i = 0; i < valueImages.Length; i++)
            {
                // 桁が非表示ならオブジェクトを隠す
                if (isValueHide)
                {
                    valueImages[i].gameObject.SetActive(false);
                    valueAfterImage[i].gameObject.SetActive(false);
                    continue;
                }
                
                // それぞれの桁の数字を取得してそれに適する数値画像を設定する
                Sprite numberSprite = numberSprites[value % 10];
                Sprite numberMaskSprite = numberMaskSprites[value % 10];
                valueImages[i].sprite = numberSprite;
                valueImages[i].gameObject.SetActive(true);
                valueAfterImage[i].sprite = numberSprite;
                valueAfterImage[i].gameObject.SetActive(true);
                maskValueImages[i].sprite = numberMaskSprite;
                maskValueImages[i].gameObject.SetActive(true);
                
                // 次の桁をみる 
                value /= 10;

                // 表示する数値の全ての桁を見たなら後の桁はすべて0になるので表示をオフにする
                if (value == 0) isValueHide = true;
            }
        }
    }
}