using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Combination;
using UnityEngine;

namespace Pjfb
{
    /// <summary>
    /// マッチスキルのソートフィルターモーダル
    /// </summary>
    public class CombinationMatchSortFilterModal : BaseCombinationSkillSortFilterModal<CombinationManager.CombinationMatch>
    {
        
        [SerializeField] private GameObject annotationTextObject;

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            await base.OnPreOpen(args, token);

            Data data = (Data)args;
            
            // 自チーム/敵チームのソートフィルタータイプかどうか
            bool isTeamCombinationSortFilterType = data.sortFilterType == SortFilterUtility.SortFilterType.PlayerCombinationMatch ||
                                        data.sortFilterType == SortFilterUtility.SortFilterType.EnemyCombinationMatch;
            
            // マッチスキル用の注釈テキスト表示制御 自チーム/敵チームのソートフィルタータイプの場合に表示
            annotationTextObject.SetActive(isTeamCombinationSortFilterType);
        }
        
        /// <summary>
        /// スクロールの表示を更新
        /// </summary>
        protected override void UpdateScrollDisplay(CombinationSkillCharacterScrollData scrollData, CardType cardType)
        {
            // 派生スクリプトにキャスト
            CombinationMatchCharacterScroll matchScroll = (CombinationMatchCharacterScroll)scrollData.Scroll;
            matchScroll.ChangeCharaIconSortFilterType(ModalData.sortFilterType);
            
            // ベースの処理を実行
            base.UpdateScrollDisplay(scrollData, cardType);
        }
    }
}

