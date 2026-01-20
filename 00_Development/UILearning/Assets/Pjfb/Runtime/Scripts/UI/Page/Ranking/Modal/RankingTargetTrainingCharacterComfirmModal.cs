using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.UserData;
using TMPro;

namespace Pjfb.Ranking
{
    public class RankingTargetTrainingCharacterComfirmModal : ModalWindow
    {
        
        public class Arguments
        {
            
            private long mRankingClientId = 0;
            /// <summary>マスタのId</summary>
            public long MRankingClientId{get{return mRankingClientId;}}
            
            public Arguments(long mRankingClientId)
            {
                this.mRankingClientId = mRankingClientId;
            }
        }
        
        [SerializeField]
        private ScrollGrid scrollGrid = null;
        
        [SerializeField]
        private TMP_Text descriptionText = null;
        
        protected override UniTask OnPreOpen(object arguments, CancellationToken token)
        {
            Arguments args = (Arguments)arguments;
            
            // ランキングマスタを取得
            RankingClientPreviewMasterObject mClientPreview = MasterManager.Instance.rankingClientPreviewMaster.FindData(args.MRankingClientId);
            
            // Descriptionにデータがある場合はそれを表示
            if( string.IsNullOrEmpty(mClientPreview.description) == false)
            {
                descriptionText.text = mClientPreview.description;
                descriptionText.gameObject.SetActive(true);
                scrollGrid.gameObject.SetActive(false);
            }
            else
            {
                
                descriptionText.gameObject.SetActive(false);
                scrollGrid.gameObject.SetActive(true);
            
                // 対象
                RankingClientPreviewMasterObject.TargetType targetType = RankingClientPreviewMasterObject.TargetType.None;
                // 対象のId
                long[] targetIds = null;
                
                // Tableタイプに応じて見るマスタを変更
                switch (mClientPreview.tableType)
                {
                    // ポイントランキング
                    // TODO 3.12.0では必要ないので未実装
                    case RankingClientPreviewMasterObject.TableType.PointRanking:
                    {
                        //PointRankingSettingRealTimeMasterObject mPointRanking = MasterManager.Instance.pointRankingSettingRealTimeMaster.FindData(mClientPreview.masterId);
                        targetIds = new long[0];
                        break;
                    }
                    
                    // ポイントランキング
                    case RankingClientPreviewMasterObject.TableType.UserRanking:
                    {
                        UserRankingSettingMasterObject mUserRanking = MasterManager.Instance.userRankingSettingMaster.FindData(mClientPreview.masterId);
                        targetType = mUserRanking.targetType;
                        targetIds = mUserRanking.targetIdList;
                        break;
                    }
                }
                
                // 表示するキャラ
                List<MCharacterScrollData> characterList = new List<MCharacterScrollData>();


                switch(targetType)
                {
                    
                    case RankingClientPreviewMasterObject.TargetType.MChar:
                    {
                        // MCharIdなのでそのまま入れる
                        foreach(long mCharId in targetIds)
                        {
                            characterList.Add( new MCharacterScrollData(mCharId, UserDataManager.Instance.chara.HaveCharacterWithMasterCharaId(mCharId)) );
                        }
                        break;
                    }
                    
                    case RankingClientPreviewMasterObject.TargetType.ParentMChar:
                    {
                        // MParentIdのキャラを探して追加
                        foreach(long mParentId in targetIds)
                        {
                            foreach(CharaMasterObject mChar in MasterManager.Instance.charaMaster.values)
                            {
                                // 親キャラが一致しているものを追加する(キャラクターのみに絞る)
                                if(mChar.parentMCharaId == mParentId && mChar.cardType == CardType.Character && CharacterUtility.IsCharacterAvailable(mChar.parentMCharaId, mChar.id))
                                {
                                    characterList.Add( new MCharacterScrollData(mChar.id, UserDataManager.Instance.chara.HaveCharacterWithMasterCharaId(mChar.id)) );
                                }
                            }
                        }
                        break;
                    }
                    
                    case RankingClientPreviewMasterObject.TargetType.MRarity:
                    {
                        // レアリティのキャラを探して追加
                        foreach(long mRarityId in targetIds)
                        {
                            foreach(CharaMasterObject mChar in MasterManager.Instance.charaMaster.values)
                            {
                                if(mChar.Rarity == mRarityId && mChar.cardType == CardType.Character && CharacterUtility.IsCharacterAvailable(mChar.parentMCharaId, mChar.id))
                                {
                                    characterList.Add( new MCharacterScrollData(mChar.id, UserDataManager.Instance.chara.HaveCharacterWithMasterCharaId(mChar.id)) );
                                }
                            }
                        }
                        break;
                    }
                    
                    default:
                    {
                        CruFramework.Logger.LogError("未実装の対象一覧 : " + mClientPreview.tableType + " , " + targetType);
                        break;
                    }
                }
                
                // Id順にソート
                IEnumerable<MCharacterScrollData> sortedTargetCharacters = characterList.OrderByDescending(v=>v.MCharaId);
                // スクロールにセット
                scrollGrid.SetItems( sortedTargetCharacters.ToArray() );
            }
            
            return base.OnPreOpen(args, token);
        }
    }
}