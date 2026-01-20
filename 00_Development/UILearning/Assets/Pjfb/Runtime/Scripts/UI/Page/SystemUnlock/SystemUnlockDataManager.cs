using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Logger = CruFramework.Logger;

namespace Pjfb.SystemUnlock
{
    public class SystemUnlockDataManager : CruFramework.Utils.Singleton<SystemUnlockDataManager>
    {
        public enum SystemUnlockNumber
        {
            Club = 200001,
            Pvp = 310001,
            SpecialSupportCard1 = 421001,
            SpecialSupportCard2 = 421002,
            SpecialSupportCard3 = 421003,
            AnyTraining = 431000,
            SupportEquipment = 441001,
            ExSupport = 451001,
            CollectionSkill = 510001,
            TrainingSkill = 510002,
            MatchSkill = 510003,
            AutoTraining = 610001,
            TrainingDeckEnhance = 710001,
            TrainingAdviser = 471101,
            ClubRoyalAdviser = 473101,
        }
        
        private bool isUnlockPrepared = false;
        public bool IsUnlockPrepared => isUnlockPrepared;
        
        private const long DefaultNumber = 0;
        private const int DefaultScenarioValue = 0;
        private long tempUnlockSystemNumber;
        private int tempReadScenarioValue;

        private List<SystemUnlockReference> prevReferenceList;
        private SystemUnlockNumber unlockingReference = 0;

        public async UniTask Unlock(SystemUnlockView.Param param)
        {
            unlockingReference = (SystemUnlockNumber)param.systemNumber;
            prevReferenceList = new List<SystemUnlockReference>();
            
            var unlockView = AppManager.Instance.UIManager.EffectManager.SystemUnlockView;
            UpdateTempUnlockSystemNumber(param.systemNumber);

            var unlockViewToken = unlockView.GetCancellationTokenOnDestroy();

            // シナリオに紐づく解放番号があれば取得
            var trainingNumbers =
                MasterManager.Instance.trainingScenarioMaster.values.Where(x =>
                    x.mSystemLockSystemNumber == param.systemNumber);
            if (trainingNumbers.Any())
            {
                // トレーニングの解放である場合、汎用のものに上書き
                unlockingReference = SystemUnlockNumber.AnyTraining;
            }

            // ビューの初期化
            unlockView.Open(param);

            // 汎用解放演出の再生
            await unlockView.StartEffect();
            
            if (unlockView.NoGuideSystemUnlock.Contains(unlockingReference))
            {
                await UniTask.WaitUntil(() => isUnlockPrepared, cancellationToken: unlockViewToken);
                // ボタン誘導のない場合はそのまま既読させる
                await RequestReadUnlockEffectAsync();
            }
            else
            {
                while (SearchNext(unlockingReference, out var reference))
                {
                    unlockView.SetActiveTouchGuard(true);

                    // タップエフェクトを生成
                    await unlockView.WaitInputTapEffectAsync(reference);

                    // ページのロードを待機
                    await WaitLoadPageAsync();
                }
            }

            try
            {
                // 既読APIを投げ終わるまで待機する。
                await UniTask.WaitUntil(() => tempUnlockSystemNumber == DefaultNumber,
                    cancellationToken: unlockViewToken);
            }
            catch (Exception)
            {
                // API送信までに何らかのエラーが発生した時
                ResetAllTempData();
                return;
            }
            
            // ビューを破棄
            unlockView.EndEffectAndDestroy();
            isUnlockPrepared = false;
        }

        private bool SearchNext(SystemUnlockNumber systemUnlockNumber, out SystemUnlockReference referenceObject)
        {
            referenceObject = null;
            // 探す
            var references = AppManager.Instance.UIManager.RootCanvas
                .GetComponentsInChildren<SystemUnlockReference>();

            if (references.Length == 0)
            {
                return false;
            }
            
            foreach (var reference in references)
            {
                // すでにポインティングしたものは除外
                if (prevReferenceList.Contains(reference))
                {
                    continue;
                }
                
                // 該当の番号が割り当てられたオブジェクトがあれば返す
                if (reference.SystemUnlockNumber.Contains(systemUnlockNumber))
                {
                    referenceObject = reference;
                    prevReferenceList.Add(reference);
                    return true;
                }
            }

            return false;
        }

        public void EffectPrepare()
        {
            isUnlockPrepared = true;
        }
        
        public void UpdateTempUnlockSystemNumber(long systemNumber)
        {
            tempUnlockSystemNumber = systemNumber;
        }
        
        public void ResetTempUnlockSystemNumber()
        {
            //初期化
            UpdateTempUnlockSystemNumber(DefaultNumber);
        }

        public bool IsUnlockingSystem(long systemNumber)
        {
            return tempUnlockSystemNumber == systemNumber;
        }
        
        public void UpdateTempReadScenarioValue(int scenarioValue)
        {
            tempReadScenarioValue = scenarioValue;
        }
        
        public bool IsReadScenario(int scenarioValue)
        {
            return tempReadScenarioValue == scenarioValue;
        }

        public void ResetTempReadScenarioValue()
        {
            // 初期化
            UpdateTempReadScenarioValue(DefaultScenarioValue);
        }
        
        /// <summary>
        /// データ周りの初期化
        /// </summary>
        public void ResetAllTempData()
        {
            // ADVの再生フラグを初期化
            ResetTempReadScenarioValue();
            ResetTempUnlockSystemNumber();
        }
        
        public async UniTask RequestReadUnlockEffectAsync()
        {
            if (tempUnlockSystemNumber == DefaultNumber)
            {
                Logger.LogError("機能の解放演出中ではありません");
                return;
            }
            
            UserUpdateViewedSystemEffectAPIRequest request = new UserUpdateViewedSystemEffectAPIRequest();
            UserUpdateViewedSystemEffectAPIPost post = new UserUpdateViewedSystemEffectAPIPost()
            {
                systemNumber = tempUnlockSystemNumber,
            };
    
            request.SetPostData(post);

            await APIManager.Instance.Connect(request);

            UserUpdateViewedSystemEffectAPIResponse res = request.GetResponseData();
            
            ResetAllTempData();
        }
        
        private async UniTask WaitLoadPageAsync()
        {
            await UniTask.WaitUntil(() =>
                AppManager.Instance.UIManager.PageManager.LoadingPageType ==
                AppManager.Instance.UIManager.PageManager.CurrentPageType);
        }
    }
}