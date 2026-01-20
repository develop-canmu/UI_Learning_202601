using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Training;

namespace Pjfb
{
    //// <summary> 発動するカードコンボのハイライト表示を行う </summary>
    public class TrainingCardComboHighlightView : MonoBehaviour
    {
        public class CardComboData
        {
            // コンボId
            private long comboId = 0;
            public long ComboId => comboId;
            
            // コンボに必要なカードId
            private List<long> activateCardIdList;
            public List<long> ActivateCardIdList => activateCardIdList;

            public CardComboData(long comboId, List<long> activateCardIdList)
            {
                this.comboId = comboId;
                this.activateCardIdList = activateCardIdList;
            }
        }
        
        // 複数のコンボがある場合にハイライトを切り替える時間
        [SerializeField] private float switchHighlightTime = 0f;

        // 選択時の連結エフェクトを切り替える時間
        [SerializeField] private float switchSelectConnectTime = 0f;
        
        // メイン表示用のハイライト色
        [SerializeField] private Color mainHighlightColor;

        // メインのコンボと重複してないコンボのハイライト色
        [SerializeField] private Color[] subHighlightColorList;
        
        // コンボに必要なカードデータをまとめたList
        private List<CardComboData> cardComboDataList;

        // カードIdごとのコンボをリスト化したDictionary
        Dictionary<long, List<CardComboData>> cardComboDictionary = new Dictionary<long, List<CardComboData>>();
        
        // カード表示View
        private TrainingActionCardView[] cardViews;

        // 現在ハイライト表示しているカードView
        private List<TrainingActionCardView> highlightCardViewList = new List<TrainingActionCardView>();
        
        // 連結エフェクトを発生させているカードView
        private List<TrainingActionCardView> connectCardViewList = new List<TrainingActionCardView>();
        
        // 選択されたカード
        private TrainingActionCardView selectCardView;

        // 選択カードの接続基準点
        private RectTransform selectConnectRoot;
        
        // 前の選択オブジェクトの座標
        private Vector3 PreSelectConnectPos;
        
        // 現在表示しているコンボ
        private int currentIndex = 0;

        // キャンセルトークンソース
        private CancellationTokenSource cancellationTokenSource = null;

        private CancellationToken CancellationToken
        {
            get
            {
                // トークンがないなら作成する
                if (cancellationTokenSource == null)
                {
                    cancellationTokenSource = new CancellationTokenSource();
                }
                return cancellationTokenSource.Token;
            }
        }

        // 時間計測タイマー
        private float timer = 0;
        
        public void SetCardComboData(TrainingCard[] handList, TrainingCardReward[] cardRewardList, TrainingActionCardView[] cardViews)
        {
            this.cardViews = cardViews;
            selectCardView = null;
           
            long[] cardIdList = null;
            List<CardComboData> cardComboDataList = new List<CardComboData>();
            Dictionary<long, List<CardComboData>> cardComboDictionary = new Dictionary<long, List<CardComboData>>();
            // 並び替え用リスト
            IEnumerable<long> sortComboIdList;
            currentIndex = 0;
            timer = 0;
           
            // 現在の手札で発生するコンボId
            List<long> activeCardComboId = new List<long>();
            // カード１枚毎に発生するコンボからカード全体で発生するコンボを取得する
            foreach (TrainingCardReward reward in cardRewardList)
            {
                foreach (long comboId in reward.mTrainingCardComboIdList)
                {
                    // すでに追加されたカードコンボなら無視
                    if (activeCardComboId.Contains(comboId))
                    {
                        continue;
                    }
                    activeCardComboId.Add(comboId);
                }
            }

            // 並び替え
            sortComboIdList = activeCardComboId
                // Priorityで降順にソート 
                .OrderByDescending(x => MasterManager.Instance.trainingCardComboMaster.FindData(x).priority)
                // Id降順でソート
                .ThenByDescending(x => x);

            // 現在の手札で発生するカードコンボデータ
            List<CardComboData> activateCardComboDataList = new List<CardComboData>();

            foreach (long comboId in sortComboIdList)
            {
                // コンボに必要なカード
                cardIdList = MasterManager.Instance.trainingCardComboElementMaster.GetComboCardIdList(comboId);
                activateCardComboDataList.Add(new CardComboData(comboId, new List<long>(cardIdList)));
            }
            
            // カードごとに発動するコンボを分別
            for (int i = 0; i < handList.Length; i++)
            {
                cardComboDataList.Clear();
                // 並び替え
                sortComboIdList = cardRewardList[i].mTrainingCardComboIdList
                    // Priorityで降順にソート 
                    .OrderByDescending(x => MasterManager.Instance.trainingCardComboMaster.FindData(x).priority)
                    // Id降順でソート
                    .ThenByDescending(x => x);

                foreach (long comboId in sortComboIdList)
                {
                    // コンボに必要なカード
                    cardIdList = MasterManager.Instance.trainingCardComboElementMaster.GetComboCardIdList(comboId);
                    cardComboDataList.Add(new CardComboData(comboId, new List<long>(cardIdList)));
                }
                cardComboDictionary.Add(handList[i].mTrainingCardId, new List<CardComboData>(cardComboDataList));
            }
            
            // コンボデータを設定
            UpdateComboData(activateCardComboDataList, cardComboDictionary);
        }

        public void UpdateComboData(List<CardComboData> comboDataList, Dictionary<long, List<CardComboData>> comboDataDictionary)
        {
            cardComboDataList = comboDataList;
            cardComboDictionary = comboDataDictionary;
        }

        //// <summary> カードコンボハイライト表示 </summary>
        public async UniTask ShowHighlightAsync()
        {
            // 演出をすべて止めてから表示する
            await StopAllEffectAsync();
            ShowHighlight();
        }
        
        //// <summary> カードコンボが発動するかのハイライト表示 </summary>
        private void ShowHighlight()
        {
            // 表示するコンボがない、表示Indexが範囲外なら何も表示しない
            if (cardComboDataList.Count <= 0 || cardComboDataList.Count < currentIndex + 1)
            {
                return;
            }
            
            // 現在のメインのコンボデータ
            CardComboData currentComboData = cardComboDataList[currentIndex];
            // ハイライト表示に使用されていないカードView
            List<TrainingActionCardView> notUsedCardViewList = new List<TrainingActionCardView>();
            
            // メインのハイライト
            foreach (TrainingActionCardView cardView in cardViews)
            {
                // カードコンボの発生条件のカードならハイライト表示する
                if (currentComboData.ActivateCardIdList.Contains(cardView.Card.mTrainingCardId))
                {
                    cardView.PlayCardComboEffect(mainHighlightColor, true, false);
                    highlightCardViewList.Add(cardView);
                }
                else
                {
                    // ハイライト表示に利用されてないものは保持しておく
                    notUsedCardViewList.Add(cardView);
                }
            }

            // サブエフェクトとして利用しているカードコンボのリスト
            List<CardComboData> subCardComboDataList = new List<CardComboData>();
            
            // サブエフェクト色でハイライト表示ができなくなるまでループ
            while (true)
            {
                // ハイライト表示に利用されていないカードIdリスト
                IEnumerable<long> notUsedCardIdList = notUsedCardViewList.Select(x => x.Card.mTrainingCardId);
                CardComboData subCardComboData = null;
                
                for(int i = 0; i < cardComboDataList.Count; i++)
                {
                    // 現在表示しているカードコンボの順番から優先して見ていく
                    int index = currentIndex + i;
                    // 配列長を超えるならIndexに収まるようにする
                    if (index >= cardComboDataList.Count)
                    {
                        index -= cardComboDataList.Count;
                    }
                    
                    // 利用されていないカード内で発動されるカードコンボがある
                    if (cardComboDataList[index].ActivateCardIdList.All(x => notUsedCardIdList.Contains(x)))
                    {
                        subCardComboData = cardComboDataList[index];
                        subCardComboDataList.Add(subCardComboData);
                        break;
                    }
                }

                // 重複せずに表示出来るカードコンボがないならループから抜ける
                if (subCardComboData == null)
                {
                    break;
                }
             
                // 未使用のカードViewからハイライト表示に
                foreach (TrainingActionCardView cardView in notUsedCardViewList.ToList())
                {
                    // カードコンボの発生条件のカードならハイライト表示する
                    if (subCardComboData.ActivateCardIdList.Contains(cardView.Card.mTrainingCardId))
                    {
                        // 現状はサブエフェクトは１つしか出ないが今後カードの枚数が増えて同時コンボ数が増える場合があるのでサブエフェクトも複数色対応しておく
                        cardView.PlayCardComboEffect(subHighlightColorList[subCardComboDataList.Count - 1], true, false);
                        highlightCardViewList.Add(cardView);
                        // ハイライト未表示リストから削除
                        notUsedCardViewList.Remove(cardView);
                    }
                }
            }
            
            // 複数コンボがあり、メインエフェクトとサブエフェクトで表示できてないならタイマーをセットし時間で切り替える
            if (cardComboDataList.Count > 1 && cardComboDataList.Count - 1 > subCardComboDataList.Count)
            {
                timer = switchHighlightTime;
            }
        }

        //// <summary> 次のカードコンボを表示する </summary>
        private async UniTask NextCardComboHighLight(CancellationToken token)
        {
            // 次のコンボエフェクトに進める
            currentIndex++;

            // カード非選択時のエフェクト切り替え
            if (selectCardView == null)
            {
                // Listの要素数を超えないように調整
                if (currentIndex >= cardComboDataList.Count)
                {
                    currentIndex = 0;
                }
                // ハイライト表示しているカードエフェクトを止める
                await StopAllEffect(token, false, true);
                ShowHighlight();
            }
            // カード選択時
            else
            {
                // 選択したカードで発生するコンボListの要素数を超えないように調整
                if (currentIndex >= cardComboDictionary[selectCardView.Card.mTrainingCardId].Count)
                {
                    currentIndex = 0;
                }
               
                await StopAllEffect(token, false, false);
                ShowComboConnectLine();
            }
        }

        //// <summary> ハイライト表示を止める </summary>
        private async UniTask StopCardComboHighLight(CancellationToken token, bool immediate)
        {
            List<UniTask> taskList = new List<UniTask>();

            // ハイライト表示しているカードエフェクトを止める
            foreach (TrainingActionCardView cardView in highlightCardViewList)
            {
                // 連結用の球体オブジェクトのアクティブは切る
                cardView.SetActiveCardComboSphere(false);
                // タスクに積む
                taskList.Add(cardView.StopCardComboEffectAsync(token, immediate));
            }

            // 積まれたタスクがあるなら全部終わるまで待つ
            if (taskList.Count > 0)
            {
                await UniTask.WhenAll(taskList);
            }

            // 今ハイライト表示している物をクリア
            highlightCardViewList.Clear();
        }
        
        //// <summary> コンボの連結部分の表示 </summary>
        public async UniTask SelectCardComboConnectLineAsync(int selectCardIndex)
        {
            // 非同期処理を止める
            Cancel();
            // すべてのエフェクトを止める
            await StopAllEffect(CancellationToken, true, true);
            
            // 選択したカードにコンボが含まれていない場合はエフェクトは再生しない
            if (cardComboDictionary.ContainsKey(cardViews[selectCardIndex].Card.mTrainingCardId) == false)
            {
                return;
            }
            // Index番号を初期値に
            currentIndex = 0;
            // 選択されたカード
            selectCardView = cardViews[selectCardIndex];
            // コンボ連結エフェクトを表示
            ShowComboConnectLine();
        }
        
        //// <summary> コンボの連結部分の表示 </summary>
        private void ShowComboConnectLine()
        {
            List<CardComboData> comboDataList = cardComboDictionary[selectCardView.Card.mTrainingCardId];
            // 選択カードの接続基準点
            selectConnectRoot = selectCardView.GetCardComboConnectRoot();
            // 切り替えが行われないようにタイマーを初期化しておく
            timer = 0;
            // 保持しておいた接続位置をリセット
            PreSelectConnectPos = Vector3.zero;

            // 発動するコンボがないなら表示しない
            if (comboDataList.Count <= 0)
            {
                return;
            }
            
            foreach (TrainingActionCardView cardView in cardViews)
            {
                // コンボカードに含まれていないものは無視
                if (comboDataList[currentIndex].ActivateCardIdList.All(x => x != cardView.Card.mTrainingCardId))
                {
                    continue;
                }

                // 選択カードは連結エフェクトは出さず縁のエフェクトのみ
                if (cardView.Card.mTrainingCardId == selectCardView.Card.mTrainingCardId)
                {
                    highlightCardViewList.Add(cardView);
                    cardView.PlayCardComboEffect(mainHighlightColor, true, true);
                    cardView.SetActiveCardComboSphere(true);
                }
                // 選択カード以外のカードは選択カードに向かって連結エフェクトを出す
                else
                {
                    highlightCardViewList.Add(cardView);
                    connectCardViewList.Add(cardView);
                    cardView.SetActiveCardComboSphere(true);
                    // 選択カード以外のカードはハイライト表示しない
                    cardView.PlayCardComboEffect(mainHighlightColor, false, true);
                    cardView.PlayCardComboConnectLineEffect();
                }
            }
            
            // 複数コンボがあるならタイマーをセットし時間で切り替える
            if (cardComboDictionary[selectCardView.Card.mTrainingCardId].Count > 1)
            {
                timer = switchSelectConnectTime;
            }
        }

        //// <summary> 連結エフェクトを止める </summary>
        private async UniTask StopComboConnectLine(CancellationToken token, bool immediate)
        {
            List<UniTask> taskList = new List<UniTask>();

            // ハイライト表示しているカードエフェクトを止める
            foreach (TrainingActionCardView cardView in connectCardViewList)
            {
                // 連結用の球体オブジェクトのアクティブは切る
                cardView.SetActiveCardComboSphere(false);
                // タスクに積む
                taskList.Add(cardView.StopCardComboConnectLineEffectAsync(token, immediate));
            }

            // 積まれたタスクがあるなら全部終わるまで待つ
            if (taskList.Count > 0)
            {
                await UniTask.WhenAll(taskList);
            }
            
            connectCardViewList.Clear();
        }
        
        //// <summary> すべてのエフェクトを止める(引数のフラグで即時に終了させるかを指定する) </summary>
        private async UniTask StopAllEffect(CancellationToken token, bool isImmediateHighlight, bool isImmediateConnectLine)
        {
            List<UniTask> taskList = new List<UniTask>();
            // ハイライト表示を止める
            taskList.Add(StopCardComboHighLight(token, isImmediateHighlight));
            // 連結エフェクトを止める
            taskList.Add(StopComboConnectLine(token, isImmediateConnectLine));
            await UniTask.WhenAll(taskList);
        }

        //// <summary> エフェクトを外部から完全に止める </summary>
        public async UniTask StopAllEffectAsync()
        {
            Cancel();
            // 時間で切り替わらないようにタイマーも初期化
            timer = 0;
            await StopAllEffect(CancellationToken, true, true);
        }

        //// <summary> 連結オブジェクトを更新 </summary>
        private void UpdateConnectLine()
        {
            // 以前と座標が同じなら更新はしない
            if (PreSelectConnectPos == selectConnectRoot.position)
            {
                return;
            }
          
            PreSelectConnectPos = selectConnectRoot.transform.position;   
            
            foreach (TrainingActionCardView cardView in connectCardViewList)
            {
                RectTransform comboCardConnectRoot = cardView.GetCardComboConnectRoot();
                // 選択カードへのベクトル
                Vector3 selectCardVec = selectConnectRoot.transform.position - comboCardConnectRoot.transform.position;
                float distance = Vector2.Distance(selectConnectRoot.transform.position, comboCardConnectRoot.transform.position);
                // Y軸基準で選択カードの向きに
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, selectCardVec);
                // 連結オブジェクト調整
                cardView.AdjustCardComboConnectLineEffect(distance, rotation);
            }
        }
        
        private void Update()
        {
            // カード選択時は連結オブジェクトを更新する
            if (selectCardView != null)
            {
                UpdateConnectLine();
            }

            // タイマーがセットされてないなら何もしない
            if (timer <= 0)
            {
                return;
            }

            timer -= Time.deltaTime;

            // セットされた時間が経過した
            if (timer <= 0)
            {
                // ハイライト表示切り替え
                NextCardComboHighLight(CancellationToken).Forget();
            }
        }

        //// <summary> オブジェクトが破棄される際は非同期処理を止める </summary>
        private void OnDestroy()
        {
            Cancel();
        }
        

        private void Cancel()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
        }
    }
}