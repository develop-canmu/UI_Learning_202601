using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace Pjfb.Menu
{
    public class HelpModalWindow : ModalWindow
    {
        #region Params
        public static string CacheHelpDetail = "";
        
        [Serializable]
        public partial class MHelp
        {
            public int sort;    //順番 
            public string categoryName;    // カテゴリ名称
            public string title;    // タイトル
            public string body;    // 内容
            public string image;   //画像
            public string onClick;   //クリック処理
        }
        public class WindowParams
        {
            public List<string> categoryList;  //表示カテゴリ、nullなら全部表示
            public List<string> titleList; //カテゴリ内の表示タイトル、nullなら全部表示
            public Action onClosed;
        }
        
        [SerializeField] private ScrollDynamic scroller;
        private WindowParams _windowParams;
        private Dictionary<string, List<MHelp>> allItemList = new Dictionary<string, List<MHelp>>();
        private List<HelpCategoryScrollItem.Data> selectItemList = new List<HelpCategoryScrollItem.Data>();

        #endregion

        #region Life Cycle
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Help, data);
        }
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            await Init();
            await base.OnPreOpen(args, token);
            UpdateScrollItems();
        }

        protected override void OnOpened()
        {
            base.OnOpened();
        }

        private async UniTask Init()
        {
            string helpUrl = AppEnvironment.HelpJsonURL;
            //json textをダウロードして情報リスト作成
            await DownloadAndParseList(helpUrl);
        }
        #endregion

        #region EventListeners
        public void OnClickClose()
        {
            Close(onCompleted: _windowParams.onClosed);
        }
        #endregion

        #region Other

        private void UpdateScrollItems()
        {
            selectItemList.Clear();
            //表示カテゴリ設定
            if (_windowParams.categoryList != null && _windowParams.categoryList.Any())
            {
                _windowParams.categoryList.ForEach(category =>
                {
                    if (!allItemList.ContainsKey(category)) return;

                    var itemList = (_windowParams.titleList != null && _windowParams.titleList.Any())
                        ? allItemList[category].Where(help => _windowParams.titleList.Contains(help.title))
                        : allItemList[category];
                    
                    selectItemList.Add(new HelpCategoryScrollItem.Data
                    {
                        title = category,
                        ItemList = itemList.OrderBy(c => c.sort).ToList(),
                        defaultShow = true 
                    });
                });
            }
            
            foreach (var pair in allItemList)
            {
                if(_windowParams.categoryList != null && _windowParams.categoryList.Contains(pair.Key)) continue;
                selectItemList.Add(new HelpCategoryScrollItem.Data
                {
                    title = pair.Key,
                    ItemList = pair.Value.OrderBy(c=> c.sort).ToList()
                });
            }
            scroller.SetItems(selectItemList);
        }

        private async UniTask DownloadAndParseList(string targetUrl)
        {
            try
            {
                UnityWebRequest request = null;
                if (string.IsNullOrEmpty(CacheHelpDetail))
                {
                    request = UnityWebRequest.Get(targetUrl);
                    await request.SendWebRequest().WithCancellation(gameObject.GetCancellationTokenOnDestroy());
                    CacheHelpDetail = request.downloadHandler.text;
                }

                var jsons = JsonUtils.ParseList<Dictionary<string, object>>(CacheHelpDetail);
                if (jsons.Any())
                {
                    allItemList.Clear();
                    foreach (var option in jsons)
                    {
                        // TODO: 他のパラメータの追加(あれば)
                        var model = new MHelp();                              
                        model.sort = Convert.ToInt32(option["sort"]);
                        model.categoryName = option["categoryName"].ToString();
                        model.title = option["title"].ToString();
                        model.body = option["body"].ToString();
                        model.image = option.ContainsKey("image") ? option["image"].ToString() : "";
                        model.onClick = option.ContainsKey("onClick") ? option["onClick"].ToString() : "";

                        if (allItemList.ContainsKey(model.categoryName))
                        {
                            allItemList[model.categoryName].Add(model);
                        }
                        else
                        {
                            allItemList.Add(model.categoryName, new List<MHelp>() {model});
                        }
                    }
                }
                if(request != null) request.Dispose();
            }
            catch (Exception)
            {
                transform.localScale = Vector3.zero;
                string errorMessage = StringValueAssetLoader.Instance["help.network_error"];
                var errorParam = new ConfirmModalData(StringValueAssetLoader.Instance["common.confirm"], errorMessage,
                    string.Empty, new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"],
                        window =>
                        {
                            transform.localScale = Vector3.one;
                            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(w=> w.GetType() == typeof(HelpModalWindow));
                            window.Close();
                        })); 
                ConfirmModalWindow.Open(errorParam);
            }
        }
        #endregion
    }
}