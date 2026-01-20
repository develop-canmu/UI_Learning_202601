using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.UI;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Master;
using Pjfb.UserData;
using Logger = CruFramework.Logger;

namespace Pjfb.Event
{
    public class EventBoostWindow : ModalWindow
    {
        #region Params

        public class WindowParams
        {
            public List<FestivalSpecificCharaMasterObject> boostCharaList;
            public Action onClosed;
        }

        #endregion
        private WindowParams _windowParams;
        
        [SerializeField]
        private List<BoostData> boostDataList = new List<BoostData>();
        
        /// <summary> ブーストキャラのスクロールデータクラス </summary>
        [Serializable]
        private class BoostData
        {
            [SerializeField]
            private CardType cardType = CardType.Character;
            /// <summary> カードタイプ </summary>
            public CardType CardType => cardType;
            
            [SerializeField]
            private Transform root = null;
            /// <summary> ルートオブジェクト </summary>
            public Transform Root => root;
            
            [SerializeField]
            private BoostCharacterIcon iconPrefab = null;
            /// <summary> アイコンPrefab </summary>
            public BoostCharacterIcon IconPrefab => iconPrefab;
            
            private List<CharacterDetailData> detailDataList = new List<CharacterDetailData>();
            /// <summary> スクロール内のキャラクターデータ </summary>
            public List<CharacterDetailData> DetailDataList => detailDataList;
            
            private List<BoostCharaScrollData> boostScrollData = new List<BoostCharaScrollData>();
            public List<BoostCharaScrollData> BoostScrollData => boostScrollData;
            
            // 生成済みキャラアイコンリスト
            private List<BoostCharacterIcon> characterIconList = new List<BoostCharacterIcon>();
            public List<BoostCharacterIcon> CharacterIconList => characterIconList;
        }
        
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.EventBoost, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            Init();
            return base.OnPreOpen(args, token);
        }



        #region PrivateMethods
        private void Init()
        {
            foreach (var mTrainingFestivalSpecificChara in _windowParams.boostCharaList
                         .OrderByDescending(x => RarityUtility.GetBaseRarity(x.mCharaId))
                         .ThenBy(x => x.mCharaId)) 
            {
                long mCharaId = mTrainingFestivalSpecificChara.mCharaId;
                CharaMasterObject mChara = MasterManager.Instance.charaMaster.FindData(mCharaId);
                if(mChara is null)  continue;
                bool hasChara = EventPage.MCharaPossessionHashSet.Contains(mCharaId);
                var charaDetailData = new CharacterDetailData(mCharaId, 1, 0);

                // カードタイプ毎のスクロールデータを取得
                BoostData data = GetBoostCharaScrollData(mChara.cardType);
                // データがない場合は無視
                if (data == null)
                {
                    continue;
                }
                
                // スクロールIndex番号
                int scrollIndex = data.BoostScrollData.Count;
                // 詳細データ追加
                data.DetailDataList.Add(charaDetailData);
                // スワイプ用データ作成
                SwipeableParams<CharacterDetailData> swipeParam = new SwipeableParams<CharacterDetailData>(data.DetailDataList, scrollIndex);
                // ブーストキャラスクロールデータ追加
                data.BoostScrollData.Add(new BoostCharaScrollData(mCharaId, hasChara, mTrainingFestivalSpecificChara.rate, swipeParam));
            }


            foreach (BoostData boostData in boostDataList)
            {
                // データがあるか
                bool hasData = boostData.BoostScrollData.Count > 0;
                // データがないなら非表示
                boostData.Root.gameObject.SetActive(hasData);
                if (hasData == false)
                {
                    continue;
                }
                
                // スクロール数分のアイコンを生成
                for (int i = boostData.CharacterIconList.Count; i < boostData.BoostScrollData.Count; i++)
                {
                    // アイコン生成
                    BoostCharacterIcon icon = Instantiate(boostData.IconPrefab, boostData.Root);
                    // キャッシュリストに追加
                    boostData.CharacterIconList.Add(icon);
                }

                for (int i = 0; i < boostData.CharacterIconList.Count; i++)
                {
                    BoostCharacterIcon icon = boostData.CharacterIconList[i];
                    // スクロール以上のキャッシュデータは非表示
                    if (i >= boostData.BoostScrollData.Count)
                    {
                        icon.gameObject.SetActive(false);
                        continue;
                    }
                    icon.gameObject.SetActive(true);
                    // アイコン初期化
                    icon.InitializeUI(boostData.BoostScrollData[i]);
                }

            }
        }

        /// <summary> 指定したカードタイプのブーストスクロールデータを取得 </summary>
        private BoostData GetBoostCharaScrollData(CardType cardType)
        {
            foreach (BoostData data in boostDataList)
            {
                // カードタイプが一致
                if (data.CardType == cardType)
                {
                    return data;
                }
            }

            Logger.LogError($"Not Define BoostChara CardType :{cardType}");
            return null;
        }
        
        #endregion

        #region EventListeners

        public void OnClickClose()
        {
            Close(onCompleted: _windowParams.onClosed);
        }
        #endregion
       
        
        
    }
}
