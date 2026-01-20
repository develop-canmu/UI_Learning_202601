using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Master;
using UnityEngine;
using TMPro;

namespace Pjfb.ClubRoyal
{
    public class ClubRoyalMatchHistoryClubInfoView : MonoBehaviour
    {
        /// <summary>
        /// サーバー側のレーン番号に依存
        /// 本拠点: 1、上拠点: 2、下拠点: 3
        /// </summary>
        public enum OccupiedSpotType
        {
            None = 0,
            // 本拠地
            CenterBase = 1,
            // 上拠点
            TopBase = 2,
            // 下拠点
            BottomBase = 3,
        }
        
        /// <summary>クラブ情報</summary>
        public class ClubInfo
        {
            /// <summary>クラブ名</summary>
            private string clubName;
            /// <summary>クラブ名のGetter</summary>
            public string ClubName => clubName;

            /// <summary>クラブエンブレム</summary>
            private long clubEmblemId;
            /// <summary>クラブエンブレムIDのGetter</summary>
            public long ClubEmblemId => clubEmblemId;

            /// <summary>占有ゴールのIDリスト</summary>
            private long[] occupiedSpotIdList;
            /// <summary>占有ゴールのIDリストのGetter</summary>
            public long[] OccupiedSpotIdList => occupiedSpotIdList;
            
            /// <summary>獲得ポイント</summary>
            private long earnedPoint;
            /// <summary>獲得ポイントのGetter</summary>
            public long EarnedPoint => earnedPoint;

            public ClubInfo(string clubName, long clubEmblemId, long[] occupiedSpotIdList, long earnedPoint)
            {
                this.clubName = clubName;
                this.clubEmblemId = clubEmblemId;
                this.occupiedSpotIdList = occupiedSpotIdList;
                this.earnedPoint = earnedPoint;
            }
        }
        
        /// <summary>クラブ名</summary>
        [SerializeField]
        private TextMeshProUGUI clubNameText = null;

        /// <summary>クラブエンブレム</summary>
        [SerializeField]
        private ClubEmblemImage clubEmblem = null;

        /// <summary>本拠地のゴール</summary>
        [SerializeField]
        private ClubRoyalMatchHistoryGoalView centerBaseGoalView = null;

        /// <summary>上拠点のゴール</summary>
        [SerializeField]
        private ClubRoyalMatchHistoryGoalView topBaseGoalView = null;

        /// <summary>下拠点のゴール</summary>
        [SerializeField]
        private ClubRoyalMatchHistoryGoalView bottomBaseGoalView = null;
        
        /// <summary>獲得ポイント</summary>
        [SerializeField]
        private TextMeshProUGUI earnedPointText = null;

        /// <summary>ゴールタイプとゴールViewのマップ</summary>
        private Dictionary<OccupiedSpotType, ClubRoyalMatchHistoryGoalView> goalViews = new Dictionary<OccupiedSpotType, ClubRoyalMatchHistoryGoalView>();
        private bool IsDictionaryInitialized => goalViews.Count > 0;
        
        private void InitializeDictionary()
        {
            // 辞書に何も登録されていなければデータ登録
            if (IsDictionaryInitialized == false)
            {
                // ゴールタイプとゴールViewの紐づけ
                goalViews.Add(OccupiedSpotType.CenterBase, centerBaseGoalView);
                goalViews.Add(OccupiedSpotType.TopBase, topBaseGoalView);
                goalViews.Add(OccupiedSpotType.BottomBase, bottomBaseGoalView);
            }
        }
        
        public void SetView(ClubInfo clubInfo)
        {
            // 初期化
            InitializeDictionary();
            
            // クラブ情報をセット
            SetClubName(clubInfo.ClubName);
            // クラブエンブレムをセット
            SetClubEmblem(clubInfo.ClubEmblemId).Forget();
            // 獲得ポイントをセット
            SetEarnedPoint(clubInfo.EarnedPoint);
            
            // ゴールを全てグレーアウトする
            foreach (KeyValuePair<OccupiedSpotType, ClubRoyalMatchHistoryGoalView> goalView in goalViews)
            {
                goalView.Value.SetFilter(true);
            }
            
            // 占有ゴールのIDリストからゴールの種類（本拠点、上拠点、下拠点）を判定し、GoalViewを更新する
            foreach (long id in clubInfo.OccupiedSpotIdList)
            {
                // 生き残っているゴールIDを取得する
                BattleConquestFieldSpotMasterObject conquestFieldSpotMaster = MasterManager.Instance.battleConquestFieldSpotMaster.FindData(id);
                
                // 生き残っているゴールの種類を取得する
                OccupiedSpotType occupiedSpotType = GetOccupiedSpotType(conquestFieldSpotMaster.positionY);
                
                // 生き残っているゴールをのグレーアウトを切る
                goalViews[occupiedSpotType].SetFilter(false);
            }
        }

        /// <summary>クライアント側で定義したTypeと紐づける</summary>
        private OccupiedSpotType GetOccupiedSpotType(long positionY)
        {
            return (OccupiedSpotType)positionY;
        }

        /// <summary>クラブ名のセット</summary>
        public void SetClubName(string clubName)
        {
            clubNameText.text = clubName;
        }

        /// <summary>クラブエンブレムのセット</summary>
        public async UniTask SetClubEmblem(long clubEmblemId)
        {
            await clubEmblem.SetTextureAsync(clubEmblemId);
        }

        /// <summary>獲得ポイントのセット</summary>
        public void SetEarnedPoint(long earnedPoint)
        {
            // カンマ区切りで表示
            earnedPointText.text = earnedPoint.GetStringNumberWithComma();
        }
    }
}