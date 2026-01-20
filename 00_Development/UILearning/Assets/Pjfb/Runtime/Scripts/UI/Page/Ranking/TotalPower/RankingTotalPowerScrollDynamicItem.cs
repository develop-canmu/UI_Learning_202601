using CruFramework.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Ranking
{
    /// <summary>総戦力個人ランキングにおけるスクロール内のアイテムの振る舞いを定義したクラス</summary>
    public class RankingTotalPowerScrollDynamicItem : ScrollDynamicItem
    {
        /// <summary>総戦力個人ランキングにおけるスクロール内のアイテムのパラメーター</summary>
        public class RankingUserData
        {
            /// <summary>ユーザーID</summary>
            public long UserId { get; }
            
            /// <summary>順位</summary>
            public long Rank { get; }
            
            /// <summary>ユーザーアイコン</summary>
            public long MIconId { get; }
            
            /// <summary>ユーザー名</summary>
            public string Name { get; }

            /// <summary>チーム総戦力値</summary>
            // チーム総戦力 Value
            public BigValue CombatPower { get; }
            
            /// <summary>総戦力のランクアイコン</summary>
            public long DeckRank { get; }
            
            /// <summary>デック情報</summary>
            public RankingTotalPowerDeckView.CharacterData[] DeckInfo { get; }

            /// <summary>ランクインしているか</summary>
            public bool IsFrameRankInShown { get; }

            /// <summary>デッキ表示しているかの状態保持</summary>
            public bool IsDeckVisible { get; set; } = false;

            public RankingUserData(long userId, long rank, long mIconId, string name, BigValue combatPower, long deckRank, RankingTotalPowerDeckView.CharacterData[] deckInfo, bool isFrameRankInShown)
            {
                UserId = userId;
                Rank = rank;
                MIconId = mIconId;
                Name = name;
                CombatPower = combatPower;
                DeckRank = deckRank;
                DeckInfo = deckInfo;
                IsFrameRankInShown = isFrameRankInShown;
            }
        }

        /// <summary>スクロール領域のアイテムのViewの参照</summary>
        [SerializeField]
        private RankingUserInfoView userInfoView = null;

        /// <summary>デッキ表示のステートを保持する</summary>
        private RankingUserData rankingUserData;
        
        /// <summary>ScrollDynamicからの値をキャストしてセットする</summary>
        protected override void OnSetView(object value)
        {
            rankingUserData = (RankingUserData)value;
            
            // ユーザーIDをセット
            userInfoView.SetUserId(rankingUserData.UserId);
            
            // 順位をセット
            userInfoView.SetRank(rankingUserData.Rank, rankingUserData.IsFrameRankInShown);
            
            // ユーザーアイコンをセット
            userInfoView.SetUserIcon(rankingUserData.MIconId);
            
            // ユーザー名をセット
            userInfoView.SetName(rankingUserData.Name);
            
            // チーム総戦力値、ランクアイコンをセット
            userInfoView.SetTotalPower(rankingUserData.CombatPower, rankingUserData.DeckRank);
            
            // プロフィールボタンのセット
            // 自身のアイテムの場合（ランクインしている場合）はプロフィールボタンを非アクティブにする
            userInfoView.SetButtonProfileActive(!rankingUserData.IsFrameRankInShown);
            
            // チームボタンクリック時のデッキ表示で高さを調整するためのコールバックを設定する
            // 重複を防ぐために一度削除してから追加する
            userInfoView.DeckView.OnClickDeckEvent -= OnItemHeightChange;
            userInfoView.DeckView.OnClickDeckEvent += OnItemHeightChange;

            // デッキ部分が表示されている場合はデッキ情報をセットする
            if (rankingUserData.IsDeckVisible)
            {
                // デッキ情報のセット
                userInfoView.DeckView.SetView(rankingUserData.DeckInfo);
            }
            
            // アイテムのプレハブに対して、デッキ表示の状態を反映させる
            userInfoView.DeckView.ShowDeck(rankingUserData.IsDeckVisible);
        }

        /// <summary>スクロール領域内のアイテムの高さを変更した時に呼ぶ</summary>
        private void OnItemHeightChange(bool isDeckVisible)
        {
            // デッキ部分を閉じる場合はDeckをセットしない
            if (isDeckVisible)
            {
                userInfoView.DeckView.SetView(rankingUserData.DeckInfo);
            }
            
            // Unityレイアウトの再計算
            LayoutRebuilder.ForceRebuildLayoutImmediate(UITransform);
            
            // Scrollの再構築
            RecalculateSize();
            
            // アイテムのデッキ表示状態をセットする
            rankingUserData.IsDeckVisible = isDeckVisible;
        }
    }
}