using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.Addressables;
using CruFramework.H2MD;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Menu;
using Pjfb.Storage;
using UnityEngine;
using Logger = CruFramework.Logger;

namespace Pjfb.Gacha
{
    public class GachaEffectPage : Page
    {
        public enum GachaEffectType
        {
            Character,
            SpecialSupportCharacter,
        }
        
        private GachaResultPageData pageData = null;
        
        private GachaExecuteEffectBase effectInstance = null;

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            pageData = (GachaResultPageData)args;

            // エフェクトタイプ取得
            GachaEffectType effectType = GetEffectType(pageData);
            
            // リソース読み込み
            await LoadAssetAsync<GameObject>(GetEffectAddress(effectType), go =>
            {
                // エフェクト生成
                effectInstance = Instantiate(go, Manager.transform).GetComponent<GachaExecuteEffectBase>();
            });
            
            // ヘッダーフッターは非表示
            AppManager.Instance.UIManager.Header.Hide();
            AppManager.Instance.UIManager.Footer.Hide();

            await base.OnPreOpen(args, token);
        }
        

        protected override void OnOpened(object args)
        {
            // BGM止める
            BGMManager.PlayBGMAsync(BGM.None).Forget();
            
            effectInstance.Play(async () =>
            {
                string[] keys = await GetPreLoadAddress();
                if(keys.Length > 0)
                {
                    // 事前DL
                    bool success = await AddressablesUtility.DownloadAsset(keys);
                    // 失敗した場合はタイトルに戻るので処理しない
                    if(!success) return false;
                }
                // リザルト画面へ遷移
                GachaPage m = (GachaPage)Manager;
                await m.OpenPageAsync(GachaPageType.GachaResult, false, pageData);
                return true;
            });
        }
        
        private string GetEffectAddress(GachaEffectType effectType)
        {
            switch (effectType)
            {
                case GachaEffectType.Character:
                {
                    if((CharacterGachaExecuteEffect.Cut1)pageData.EffectDatas[0] == CharacterGachaExecuteEffect.Cut1.Barou)
                    {
                        // バロウの場合はGoldのみ
                        pageData.EffectDatas[1] = (long)CharacterGachaExecuteEffect.Cut2.Gold;
                        // バロウの場合は突き破るのみ
                        pageData.EffectDatas[3] = (long)CharacterGachaExecuteEffect.Cut4.Break;
                    } 
                    
                    return $"Prefabs/UI/Page/Gacha/Effect/CharacterGachaExecuteEffect_{pageData.EffectDatas[0]}_{pageData.EffectDatas[1]}_{pageData.EffectDatas[2]}_{pageData.EffectDatas[3]}.prefab";
                }
                case GachaEffectType.SpecialSupportCharacter:
                {
                    return $"Prefabs/UI/Page/Gacha/Effect/SpecialSupportCharacterGachaExecuteEffect_{pageData.EffectDatas[4]}_{pageData.EffectDatas[5]}.prefab";
                }
                
                default: throw new NotImplementedException();
            }
        }
        
        private async UniTask<string[]> GetPreLoadAddress()
        {
            List<string> keys = new List<string>();
            List<UniTask> tasks = new List<UniTask>();

            for(int i = 0; i < pageData.PrizeList.Length; i++)
            {
                // 報酬
                PrizeJsonViewData prize = pageData.PrizeList[i].ContentList[0];
                // キャラの場合
                if(prize.ItemIconType == ItemIconType.Character)
                {
                    // キャラマスタ取得
                    CharaMasterObject mChara = MasterManager.Instance.charaMaster.FindData(prize.Id);
                    // サポート器具は対象外
                    if(mChara.cardType == CardType.SupportEquipment) continue;
                    
                    // 獲得演出のキーを取得
                    string key = PageResourceLoadUtility.GetCharacterGetEffectPath(mChara.id);
                    tasks.Add(GetNeedDownloadAddress(key, _key =>
                    {
                        if(!string.IsNullOrEmpty(_key))
                        {
                            keys.Add(_key);
                        }
                    }));
                    
                    // キャラのガチャ立ち絵のキーを取得
                    string charaKey;
                    switch (mChara.cardType)
                    {
                        case CardType.Character:
                        case CardType.Adviser:
                            // 表示モードに応じてパスを取得
                            charaKey = GetGachaCharacterImagePath(mChara.id);
                            break;
                        case CardType.SpecialSupportCharacter:
                            charaKey = PageResourceLoadUtility.GetSpecialSupportCharacterCardGachaImagePath(mChara.id);
                            break;
                        default:
                            // 新しく追加される要素をPreloadに対応させるためのログ出力
                            charaKey = string.Empty;
                            Logger.LogError($"Unsupported Preload Gacha Asset {mChara.cardType} for character {mChara.id}");
                            break;
                    }

                    tasks.Add(GetNeedDownloadAddress(charaKey, _key =>
                    {
                        if(!string.IsNullOrEmpty(_key))
                        {
                            keys.Add(_key);
                        }
                    }));
                }
            }
            
            await UniTask.WhenAll(tasks);
            
            return keys.ToArray();
        }
        
        private async UniTask GetNeedDownloadAddress(string key, Action<string> callback)
        {
            long size = await AddressablesManager.GetDownloadSizeAsync(key);
            callback(size > 0 ? key : string.Empty);
        }
        
        private GachaEffectType GetEffectType(GachaResultPageData pageData)
        {
            try
            {
                // 報酬の1つ目でチェック
                PrizeJsonViewData prize = pageData.PrizeList[0].ContentList[0];
                
                if(prize.ItemIconType == ItemIconType.Character)
                {
                    // キャラマスタ取得
                    CharaMasterObject mChara = MasterManager.Instance.charaMaster.FindData(prize.Id);
                    // サポカの場合は演出が違う
                    if(mChara.cardType == CardType.SpecialSupportCharacter)
                    {
                        return GachaEffectType.SpecialSupportCharacter;
                    }
                }
                
                return GachaEffectType.Character;
            }
            catch
            {
                return GachaEffectType.Character;
            }
        }

        /// <summary> 軽量化オプションに応じたキャラ立ち絵のパスを取得 </summary>
        private string GetGachaCharacterImagePath(long mCharaId)
        {
            string imagePath = PageResourceLoadUtility.GetCharacterCardGachaImagePath(mCharaId);
            
            if (LocalSaveManager.saveData.appConfig.CharacterCardEffectType == (int)AppConfig.DisplayType.Light)
            {
                // 軽量モードの場合はそのまま立ち絵のパスを返す
                return imagePath;
            }
            
            // H2MDが存在すれば対応するパスを返す
            string effectPath = PageResourceLoadUtility.GetCharacterCardGachaEffectPath(mCharaId);
            
            return AddressablesManager.HasResources<H2MDAsset>(effectPath) ? effectPath : imagePath;
        }
    }
}
