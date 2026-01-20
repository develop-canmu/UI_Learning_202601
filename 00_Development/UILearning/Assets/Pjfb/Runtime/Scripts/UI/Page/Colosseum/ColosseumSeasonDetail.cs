using System.Collections;
using System.Collections.Generic;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Colosseum
{

    public class ColosseumSeasonDetail : MonoBehaviour
    {
        // ランク変動状態
        enum RankState
        {
            Up,
            Stay,
            Down
        }

        enum GradeType
        {
            Top,
            Bottom,
            Other,
        }
        
        [SerializeField] private TMP_Text rankText;
        [SerializeField] private TMP_Text winCountText;
        [SerializeField] private ColosseumRankImage rankImage;
        [SerializeField] private ColosseumRoomImage roomImage;
        [SerializeField] private Image borderBadge;
        [SerializeField] private Sprite[] borderBadgeSprite;
        
        public void SetOnSeasonValue(ColosseumSeasonData colosseumSeasonData, ColosseumState colosseumState)
        {
            var seasonStatus = colosseumSeasonData.UserSeasonStatus;

            // 集計後 or 進行中シーズンのgradeを取得
            var gradeNumber  = seasonStatus.gradeAfter != 0 ? seasonStatus.gradeAfter : seasonStatus.gradeNumber;
            var gradeMaster = MasterManager.Instance.colosseumGradeMaster.GetJoinedColosseumGradeMasterObject(seasonStatus.mColosseumEventId, gradeNumber);

            var gradeType = gradeMaster.promotionRankBottom == -1 ? GradeType.Top :
                gradeMaster.demotionRankTop == -1 ? GradeType.Bottom : GradeType.Other;
            var isTopGrade = gradeType == GradeType.Top;
            var rank = isTopGrade ? (gradeMaster.demotionRankTop-1) : gradeMaster.promotionRankBottom;
            var textKey = isTopGrade ? "pvp.rank.stay" : "pvp.rank.up";
            
            // 昇格、残留条件
            rankText.text = string.Format(StringValueAssetLoader.Instance["pvp.rank.value"],seasonStatus.ranking);
            winCountText.text = string.Format(StringValueAssetLoader.Instance[textKey],rank);
            
            // ランク変動バッジ
            SetBorderBadge(gradeType,colosseumState,seasonStatus.ranking,gradeMaster);
            
            // 号棟アイコン設定
            roomImage.SetTexture(gradeNumber);
            
            // ランクアイコン設定
            var rankNumber = MasterManager.Instance.colosseumGradeRankLabelMaster.GetRankNumber(colosseumSeasonData.MColosseumEvent.mColosseumGradeGroupId, gradeNumber, seasonStatus.ranking);
            rankImage.SetTexture(rankNumber);

        }

        private void SetBorderBadge(GradeType gradeType, ColosseumState colosseumState, long ranking, ColosseumGradeMasterObject gradeMaster)
        {
            if (borderBadge == null)
            {
                return;
            }
            
            // ランク状態バッジ
            var isOnSeason = colosseumState == ColosseumState.OnSeason;
            if (borderBadge) borderBadge.gameObject.SetActive(isOnSeason);
            if (isOnSeason)
            {
                var rankState = RankState.Stay;
                if (gradeType == GradeType.Top)
                {
                    rankState = ranking >= gradeMaster.demotionRankTop ? RankState.Down : RankState.Stay;
                }
                else if (gradeType == GradeType.Bottom)
                {
                    rankState = ranking <= gradeMaster.promotionRankBottom ? RankState.Up : RankState.Stay;
                }
                else
                {
                    if (ranking <= gradeMaster.promotionRankBottom) rankState = RankState.Up;
                    else if (ranking >= gradeMaster.demotionRankTop) rankState = RankState.Down;
                    else rankState = RankState.Stay;
                }
                borderBadge.sprite = borderBadgeSprite[(int)rankState];
            }

        }

    }
}
