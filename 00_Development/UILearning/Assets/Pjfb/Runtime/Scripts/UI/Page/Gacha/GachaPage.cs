using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.Gacha
{
    public enum GachaPageType
    {
        GachaTop,
        GachaEffect,
        GachaResult,
    }
    
    public class GachaPage : PageManager<GachaPageType>
    {
        /// <summary>ページアドレス取得</summary>
        protected override string GetAddress(GachaPageType page)
        {
            return $"Prefabs/UI/Page/Gacha/{page}Page.prefab";
        }

        /// <summary>事前準備</summary>
        protected async override UniTask OnPreOpen(object args, CancellationToken token)
        {
            // ページスタック削除
            ClearPageStack();
            
            // ページ引数
            GachaPageArgs pageArgs = args as GachaPageArgs;
            // ページ開く
            await GetGachaListAndOpenPageAsync(pageArgs, token);
        }

        /// <summary>ガチャ一覧を取得してページを開く</summary>
        public async UniTask GetGachaListAndOpenPageAsync(GachaPageArgs args ){
            await GetGachaListAndOpenPageAsync(args, this.GetCancellationTokenOnDestroy());
        }
        
        public async UniTask GetGachaListAndOpenPageAsync(GachaPageArgs args, CancellationToken token )
        {
            // リクエスト
            GachaGetListAPIRequest request = new GachaGetListAPIRequest();
            // 接続
            await APIManager.Instance.Connect(request);
            // レスポンス取得
            GachaGetListAPIResponse response = request.GetResponseData();

            // 仮想ポイント情報更新
            GachaUtility.UpdatePointAlternativeDictionary();

            // 保留情報が存在する
            if(GachaUtility.HasPendingInfo(response))
            {
                // ガチャリザルトページを表示
                // 既に引ききっている保留ガチャはレスポンスのSettingListに含まれないため、gachaCategoryDataは作成せずnullを渡す
                GachaResultPageData resultPageData = new GachaResultPageData(response.pendingInfo, null, null, null);
                resultPageData.IsComebackPage = true;

                await OpenPageAsync(GachaPageType.GachaResult, false, resultPageData, token);
            }
            else
            {
                // 最初にフォーカスするm_gacha_setting.id
                long focusGachaSettingId = 0;
                var focusTicketGacha = false;
                // 引数がある場合は設定
                if(args != null)
                {
                    focusGachaSettingId = args.FocusGachaSettingId;
                    focusTicketGacha = args.FocusTicketGacha;
                }
                
                // ガチャトップページを表示
                GachaTopPageData topPageData = new GachaTopPageData(focusGachaSettingId, focusTicketGacha, response.settingList);
                await OpenPageAsync(GachaPageType.GachaTop, false, topPageData, token);
            }
        }

        /// <summary>ページ閉じた時の後処理</summary>
        protected override void OnClosed()
        {
            base.OnClosed();
            GachaUtility.Release();
        }

        /// <summary>ページ削除された時の後処理</summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            GachaUtility.Release();
        }
    }
}
