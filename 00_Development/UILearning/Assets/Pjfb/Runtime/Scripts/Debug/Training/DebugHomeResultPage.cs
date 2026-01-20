#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT && !PJFB_REL
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Colosseum;
using Pjfb.Community;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine;

namespace Pjfb
{
    public class DebugHomeResultPage : Page
    {
        // 再生するリザルトのデータ
        private ColosseumUserSeasonStatus userSeasonStatus; 

        [SerializeField]
        private DropDownUI colosseumTypeDropDown = null;
        
        // 対戦種別の設定項目
        [SerializeField]
        private GameObject shiftMatchTypeConfig = null;
        
        // 入れ替え戦結果の設定項目
        [SerializeField]
        private GameObject shiftMatchResultConfig = null;
        
        // 継続設定項目
        [SerializeField]
        private GameObject keepEntryConfig = null;
        
        [SerializeField]
        private GameObject gradeStateConfig = null;
        
        [SerializeField]
        private DropDownUI[] dropDownUIs = null;
        
        [SerializeField]
        private TMP_InputField rankingInputField = null;
        
        private ColosseumClientHandlingType currentType = ColosseumClientHandlingType.PvP;
        
        // 通常
        private const int ShiftMatchTypeRegularMatch = 0;
        // 昇格戦
        private const int ShiftMatchTypePromotionMatch = 1;
        // 降格戦
        private const int ShiftMatchTypeRelegationMatch = 2;
        // グレード維持
        private const int ResultStateKeep = 0;
        // 昇格
        private const int ResultStatePromotion = 1;
        // 降格
        private const int ResultStateRelegation = 2;
        
        private const int UpperGrade = 2;
        private const int LowerGrade = 1;
        
        private const string defaultRanking = "1";
        
        // ColosseumClientHandlingTypeとindexを紐づけるためのリスト
        private List<ColosseumClientHandlingType> typeList = new List<ColosseumClientHandlingType>();
        
        // 入れ替え戦が存在するかつ入れ替え戦無しで昇格することがある種別のリスト
        private List<ColosseumClientHandlingType> skippablePromotionMatchTypes = new List<ColosseumClientHandlingType>()
        {
            ColosseumClientHandlingType.ClubRoyal
        };

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            // 各種enumを回してDropDownUIで選択できるようにする
            foreach (ColosseumClientHandlingType type in System.Enum.GetValues(typeof(ColosseumClientHandlingType)))
            {
                colosseumTypeDropDown.options.Add(new TMP_Dropdown.OptionData(type.ToString()));
                typeList.Add(type);
            }
            
            InitData();
            
            return base.OnPreOpen(args, token);
        }


        private void InitData()
        {
            ColosseumUserSeasonStatus status = new ColosseumUserSeasonStatus();
            status.groupSeasonStatus = new();
            // 入れ替え戦情報
            status.groupSeasonStatus.shiftMatchInfo = new();
            // エントリー引き継ぎ情報(大会等)
            status.groupSeasonStatus.entryChainInfo = new();
            
            // 既読しないように
            status.sColosseumGroupStatusId = -1;
            status.sColosseumEventId = -1;
            status.groupSeasonStatus.sColosseumEventId = -1;

            // 対戦相手の名前、エンブレムは固定
            status.groupSeasonStatus.name = "対戦相手";
            status.groupSeasonStatus.shiftMatchInfo.name = "対戦相手";
            status.groupSeasonStatus.shiftMatchInfo.mGuildEmblemId = 1;
            
            // static変数に上書き
            userSeasonStatus = status;
            OnChangedColosseumType(0);
        }


        private void ConfigReset()
        {
            foreach (DropDownUI dropDownUI in dropDownUIs)
            {
                if (dropDownUI.value == 0)
                {
                    dropDownUI.onValueChanged.Invoke(0);
                    continue;
                }
                dropDownUI.value = 0;
            }
            
            // 初期化時ランキングは1位に設定
            rankingInputField.onEndEdit.Invoke(defaultRanking);
            rankingInputField.text = defaultRanking;
            
        }
        
        public void OnClickPlayTestData()
        {
            // 演出の再生
            GvGResultDebugPlay();
        }

        private void GvGResultDebugPlay()
        {
            
            // 変更前のグレードは合わせるように
            userSeasonStatus.groupSeasonStatus.gradeBefore = userSeasonStatus.gradeNumber;
            userSeasonStatus.groupSeasonStatus.gradeNumber = userSeasonStatus.gradeNumber;
            
            var masterData = MasterManager.Instance.colosseumEventMaster.FindData(userSeasonStatus.mColosseumEventId);
            // 配列化しておく
            ColosseumUserSeasonStatus[] userSeasonStatusArray = new ColosseumUserSeasonStatus[1];
            userSeasonStatusArray[0] = userSeasonStatus;
            
            // リザルト画面へ遷移
            ColosseumManager.OpenColosseumResultModal(userSeasonStatusArray, currentType, null);
        }

        // コロシアムタイプ変更時の処理
        public void OnChangedColosseumType(int index)
        {
            // 再生するGvGのmColosseumEventIdを指定
            ColosseumEventMasterObject data = MasterManager.Instance.colosseumEventMaster.values.FirstOrDefault(v => v.clientHandlingType == typeList[index]);
            // ランクマッチはマスタが違うため存在しないIDを入れておく
            long mColosseumEventId = data?.id ?? -1;
            userSeasonStatus.mColosseumEventId = mColosseumEventId;
            userSeasonStatus.groupSeasonStatus.mColosseumEventId = mColosseumEventId;
            
            // 自分のグレードは一旦下げておく
            userSeasonStatus.gradeNumber = LowerGrade;
            
            // 一旦すべてオフ
            shiftMatchTypeConfig.SetActive(false);
            shiftMatchResultConfig.SetActive(false);
            keepEntryConfig.SetActive(false);
            gradeStateConfig.SetActive(false);
            
            currentType = typeList[index];
            
            // 選択された種別によって表示/非表示を切り替え
            switch (currentType)
            {
                case ColosseumClientHandlingType.LeagueMatch:
                case ColosseumClientHandlingType.ClubRoyal:
                    shiftMatchTypeConfig.SetActive(true);
                    break;
                case ColosseumClientHandlingType.InstantTournament:
                    keepEntryConfig.SetActive(true);
                    break;
                case ColosseumClientHandlingType.PvP:
                case ColosseumClientHandlingType.ClubMatch:
                    gradeStateConfig.SetActive(true);
                    break;
                default:
                    break;
            }
            
            // 設定項目リセット
            ConfigReset();
        }
        
        // 入れ替え戦種別変更時の処理
        public void OnChangedMatchType(int index)
        {
            // 入れ替え戦参加可否判定
            // -1:入れ替え戦不参加 1:入れ替え戦進出
            userSeasonStatus.groupSeasonStatus.shiftMatchInfo.sColosseumGroupStatusId = index > ShiftMatchTypeRegularMatch ? 1 : -1;
            
            // 入れ替え戦対戦相手のランク(昇格戦:自分のランク+1 降格戦:自分のランク-1になるように)
            userSeasonStatus.groupSeasonStatus.shiftMatchInfo.gradeNumber = index == ShiftMatchTypePromotionMatch ? UpperGrade : LowerGrade;
            // 自分のランク,降格戦の場合はあらかじめ1つ上げておく
            userSeasonStatus.gradeNumber = index == ShiftMatchTypeRelegationMatch ? UpperGrade : LowerGrade;
            
            // 入れ替え戦結果の設定項目の表示/非表示を切り替え
            shiftMatchResultConfig.SetActive(index != ShiftMatchTypeRegularMatch || skippablePromotionMatchTypes.Contains(currentType));
        }

        // 入れ替え戦結果変更時の処理
        public void OnChangedShiftMatchResult(int index)
        {
            // 入れ替え戦結果かどうか
            // 0:入れ替え戦前 1:入れ替え戦勝利 2:入れ替え戦敗北
            userSeasonStatus.groupSeasonStatus.shiftMatchInfo.result = index;
            // 対戦相手のGrade
            long opponentGrade = userSeasonStatus.groupSeasonStatus.shiftMatchInfo.gradeNumber;
            // 自分のGrade
            long myGrade = userSeasonStatus.gradeNumber;
            // 試合後のグレードを初期化
            userSeasonStatus.groupSeasonStatus.gradeAfter = LowerGrade;
            
            // シーズン終了時のグレード
            if (userSeasonStatus.groupSeasonStatus.shiftMatchInfo.result == ColosseumManager.ResultWin)
            {
                if (currentType == ColosseumClientHandlingType.ClubRoyal)
                {
                    // クラロワは入れ替え戦無しのパターンが存在するため、強制的にグレードを上げる
                    userSeasonStatus.groupSeasonStatus.gradeAfter = UpperGrade;
                }
                else
                {
                    // 相手のグレードのほうが高い場合、シーズン終了後の自分のグレードを上げる
                    userSeasonStatus.groupSeasonStatus.gradeAfter = opponentGrade > myGrade ? opponentGrade : myGrade;    
                }
            }
            else if (userSeasonStatus.groupSeasonStatus.shiftMatchInfo.result == ColosseumManager.ResultLose)
            {
                // 相手のグレードのほうが低い場合、シーズン終了後の自分のグレードを下げる
                userSeasonStatus.groupSeasonStatus.gradeAfter = opponentGrade < myGrade ? opponentGrade : myGrade;
            }
            userSeasonStatus.gradeAfter = userSeasonStatus.groupSeasonStatus.gradeAfter;
        }

        
        // 次の大会にエントリーを引き継ぐかどうか
        public void OnChangedKeepEntry(int isOn)
        {
            // 次の大会にエントリーを引き継ぐかどうか(0==継続しない 1==継続する)
            userSeasonStatus.groupSeasonStatus.entryChainInfo.isChained = isOn == 1;
        }

        // 入れ替え戦が存在しない昇格/降格/維持の設定
        public void OnChangedGradeState(int index)
        {
            // 0:維持 1:昇格 2:降格
            switch (index)
            {
                case ResultStateKeep:
                    userSeasonStatus.gradeNumber = LowerGrade;
                    userSeasonStatus.groupSeasonStatus.gradeBefore = LowerGrade;
                    userSeasonStatus.gradeAfter = userSeasonStatus.gradeNumber;
                    userSeasonStatus.groupSeasonStatus.gradeAfter = userSeasonStatus.gradeNumber;
                    break;
                case ResultStatePromotion:
                    userSeasonStatus.gradeNumber = LowerGrade;
                    userSeasonStatus.groupSeasonStatus.gradeBefore = LowerGrade;
                    userSeasonStatus.gradeAfter = UpperGrade;
                    userSeasonStatus.groupSeasonStatus.gradeAfter = userSeasonStatus.gradeAfter;
                    break;
                case ResultStateRelegation:
                    userSeasonStatus.gradeNumber = UpperGrade;
                    userSeasonStatus.groupSeasonStatus.gradeBefore = UpperGrade;
                    userSeasonStatus.gradeAfter = LowerGrade;
                    userSeasonStatus.groupSeasonStatus.gradeAfter = userSeasonStatus.gradeAfter;
                    break;
            }
        }
        
        // 順位の設定
        public void OnEndEditRanking(string ranking)
        {
            if (string.IsNullOrEmpty(ranking))
            {
                ranking = defaultRanking;
                rankingInputField.text = ranking;
            }

            long result = long.Parse(ranking);
            userSeasonStatus.ranking = result;
            userSeasonStatus.groupSeasonStatus.ranking = result;
            userSeasonStatus.groupSeasonStatus.scoreRanking = result;
        }
    }
}
#endif