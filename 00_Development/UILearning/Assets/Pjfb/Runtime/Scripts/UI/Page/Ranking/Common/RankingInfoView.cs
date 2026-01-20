using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Extensions;
using TMPro;
using UnityEngine.UI;

namespace Pjfb
{
    public abstract class RankingInfoView : MonoBehaviour
    {
        [SerializeField, Header("順位を表示する画像")]
        protected Image rankImage = null;
        [SerializeField, Header("順位を表示するテキスト")]
        protected TextMeshProUGUI rankText = null;
        [SerializeField, Header("順位表示に使うスプライト")]
        protected Sprite[] rankSprite = null;
        [SerializeField, Header("ランクインした時に表示するフレーム")]
        protected Image rankInFrame = null;
        [SerializeField, Header("名前を表示するテキスト")]
        protected TextMeshProUGUI nameText = null;
        [SerializeField, Header("ランキングで比較される値を表示するテキスト")]
        protected TextMeshProUGUI pointText = null;
        [SerializeField, Header("ランキングで表示される値を省略表示するための設定")]
        protected OmissionTextSetter omissionTextSetter = null;
        
        /// <summary>
        /// 順位をセットする
        /// </summary>
        /// <param name="rank">順位</param>
        /// <param name="isCurrent">所属しているクラブかどうか</param>
        public virtual void SetRank(long rank, bool isCurrent)
        {
            if (rankImage != null)
            {
                if (rank == 0)
                {
                    rankText.gameObject.SetActive(true);
                    rankImage.gameObject.SetActive(false);
                    rankText.text = StringValueAssetLoader.Instance["ranking.rank_blank"];
                    return;
                }

                if (rank <= rankSprite.Length)
                {
                    rankImage.gameObject.SetActive(true);
                    rankImage.sprite = rankSprite[rank - 1];
                    rankText.gameObject.SetActive(false);
                }
                else
                {
                    rankText.gameObject.SetActive(true);
                    rankImage.gameObject.SetActive(false);
                }
                rankInFrame.gameObject.SetActive(isCurrent);
            }
            rankText.text = rank.GetStringNumberWithComma();
        }
        
        /// <summary>
        /// 名前をセットする
        /// </summary>
        /// <param name="userName">ユーザー名</param>
        public void SetName(string name)
        {
            nameText.text = name;
        }
        
        /// <summary>
        /// ポイントをセットする
        /// </summary>
        /// <param name="point">ポイント</param>
        /// <param name="isOmission"></param>>
        public void SetPoint(BigValue point, bool isOmission)
        {
            if (isOmission)
            {
                pointText.text = point.ToDisplayString(omissionTextSetter.GetOmissionData());
            }
            else
            {
                pointText.text = point.ToDisplayCommaString();
            }
        }
    }
}