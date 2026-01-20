using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.UI;
using Pjfb.UserData;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Menu
{
    public class ItemIconParams
    {
        public ItemIconType iconType;
        public long iconId;
        public long count;
        public bool showCountText = false;
    }

    public class ItemListModalWindow : ModalWindow
    {
        #region Params
        public class WindowParams
        {
            public Action onClosed;
        }

        [SerializeField] private ListContainer scroller;
        [SerializeField] private GameObject itemListRoot;
        [SerializeField] private GameObject emptyText;

        private WindowParams _windowParams;
        private List<ItemIconParams> itemInfoList = new List<ItemIconParams>();

        #endregion

        #region Life Cycle
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ItemList, data);
        }

        protected override UniTask OnOpen(CancellationToken token)
        {
            Init();
            UpdateScrollItems();
            return base.OnOpen(token);
        }

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            return base.OnPreOpen(args, token);
        }

        private void Init()
        {
            GetUserItemList();
            bool hasItem = itemInfoList.Any();
            itemListRoot.SetActive(hasItem);
            emptyText.SetActive(!hasItem);
        }
        #endregion

        #region EventListeners
        public void OnClickClose()
        {
            Close(onCompleted: _windowParams.onClosed);
        }
        #endregion
        #region Other

        private void GetUserItemList()
        {
            itemInfoList.Clear();
            
            //ポイントアイテムリスト
            foreach (PointMasterObject point in MasterManager.Instance.pointMaster.values)
            {
                if (point.visibleType == (long)PointMasterContainer.PointVisibleType.Hidden) continue;
                if (point.visibleType == (long)PointMasterContainer.PointVisibleType.Display || UserDataManager.Instance.point.Contains(point.id))
                {
                    long pointValue;

                    if(point.pointType == (long)PointMasterObject.PointType.ExternalPoint)
                    {
                        // 代替えアイテムは期限内のアイテム数を表示する
                        pointValue = UserDataManager.Instance.GetExpiryPointValue(point.id);
                    }
                    else
                    {
                        pointValue = UserDataManager.Instance.point.Find(point.id)?.value ?? 0;
                    }
                    itemInfoList.Add(new ItemIconParams { iconType = ItemIconType.Item, iconId = point.id, count = pointValue, showCountText = true });
                }
            }
            
            //キャラクターピースリスト
            var pieces = UserDataManager.Instance.charaPiece.data.Values.Where(piece => piece.value > 0).OrderBy(piece => piece.charaId).ToList();
            foreach (var piece in pieces)
            {
                itemInfoList.Add(new ItemIconParams
                {
                    iconType = ItemIconType.CharacterPiece,
                    iconId = piece.charaId,
                    count = piece.value,
                    showCountText = true
                });
            }
        }

        private async void UpdateScrollItems()
        {
            if (!itemInfoList.Any()) return;
            
            //ポイントアイテム
            List<ItemListScrollItem.ItemParams> categoryContentList = new List<ItemListScrollItem.ItemParams>();
            foreach (var category in MasterManager.Instance.pointCategoryMaster.values)
            {
                if (category.visibleFlg)
                {
                    var items = itemInfoList.Where(info => info.iconType == ItemIconType.Item && MasterManager.Instance.pointMaster.FindData(info.iconId)?.mPointCategoryId == category.id)?.ToList();
                    if (items.Any())
                    {
                        categoryContentList.Add(new ItemListScrollItem.ItemParams
                        {
                            Category = category.name,
                            CategoryItemList = items.ToList()
                        });
                    }
                }
            }

            // カードタイプごとにキャラのピース
            Dictionary<CardType, List<ItemIconParams>> charaPieceDictionary = new Dictionary<CardType, List<ItemIconParams>>();
            // 分類する
            foreach (ItemIconParams iconParam in itemInfoList.Where(x => x.iconType == ItemIconType.CharacterPiece))
            {
                CharaMasterObject chara = MasterManager.Instance.charaMaster.FindData(iconParam.iconId);
                // マスターがないなら無視する
                if (chara == null)
                {
                    continue;
                }

                // まだリストにないなら追加する
                if (charaPieceDictionary.TryGetValue(chara.cardType, out List<ItemIconParams> pieceList) == false)
                {
                    pieceList = new List<ItemIconParams>();
                    charaPieceDictionary.Add(chara.cardType, pieceList);
                }
                
                pieceList.Add(iconParam);
            }

            foreach (KeyValuePair<CardType, List<ItemIconParams>> pair in charaPieceDictionary.OrderBy(x => x.Key))
            {
                // リストの要素が0なら無視
                if (pair.Value.Count <= 0)
                {
                    continue;
                }
                
                // リストに追加
                categoryContentList.Add(new ItemListScrollItem.ItemParams
                {
                    // カードタイプごとに表示文言切り替え
                    Category = StringValueAssetLoader.Instance[$"menu.item.chara_piece.card_type_{(int)pair.Key}"],
                    CategoryItemList = pair.Value,
                });
            }
            
            scroller.SetDataList(categoryContentList);
            //Scroll Itemの自動Layout更新のため1 Frame待ち、そのあとScroll ContentのRectを更新
            await UniTask.DelayFrame(1);
            LayoutRebuilder.ForceRebuildLayoutImmediate(scroller.GetComponent<ScrollRect>().content);
        }

        #endregion
    }
}