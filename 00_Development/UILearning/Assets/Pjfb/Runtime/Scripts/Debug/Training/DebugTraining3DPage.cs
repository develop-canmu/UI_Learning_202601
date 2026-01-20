#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT && !PJFB_REL

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Community;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Training;
using Pjfb.Utility;
using TMPro;

namespace Pjfb.DebugPage
{
    public class DebugTraining3DPage : Page
    {
        [Serializable]
        private class Param
        {
            public long mCharaId = 0;
            public ObjectPoolUtility.TrainingTypeEnum practice = ObjectPoolUtility.TrainingTypeEnum.squat;
            public long[] joinCharaList = null;
        }
        
        [SerializeField]
        private TrainingEventResultTrainingScene trainingScene = null;
        [SerializeField]
        private Param initParam = new Param();
        
        [SerializeField]
        private GameObject uiRoot = null;
        [SerializeField]
        private GameObject editRoot = null;
        [SerializeField]
        private ScrollGrid scrollGrid = null;
        [SerializeField]
        private TMP_InputField playTimeMills = null;
        [SerializeField]
        private TextMeshProUGUI mainCharaName = null;
        [SerializeField]
        private TextMeshProUGUI joinCharaName = null;
        [SerializeField]
        private GameObject applyButtonRoot = null;
        
        private TrainingMainArguments mainArguments = null;
        private List<DebugChara3dScrollItem.Data> charaList = new List<DebugChara3dScrollItem.Data>();
        private const int DefaultPlayTimeMills = 5000;
        
        private CancellationTokenSource cts = new CancellationTokenSource();

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            // デバッグ用のTrainingMainArgumentを生成
            mainArguments = CreateDebugArguments();
            
            // マスタデータの読み込み
            LoadAllChara();
            
            // 関連アセット読み込み
            SetPracticeType(initParam.practice);
            SetMainCharaId(initParam.mCharaId);
            SetJoinCharaList(initParam.joinCharaList);
            playTimeMills.SetTextWithoutNotify(DefaultPlayTimeMills.ToString());
            CloseEdit();
            
            // 設定の読み込み
            await TrainingUtility.LoadConfig();
            
            await base.OnPreOpen(args, token);
        }
        
        /// <summary> 再生するキャラの選択ビュー </summary>
        private void OpenCharaEdit(bool isMultiSelect)
        {
            applyButtonRoot.SetActive(isMultiSelect);

            // 選択情報の初期化
            foreach (DebugChara3dScrollItem.Data data in charaList)
            {
                if (isMultiSelect && mainArguments.JoinSupportCharacters?.Length > 0)
                {
                    // マルチ選択で既存設定がある場合ハイライトしておく
                    data.SetSelected(mainArguments.JoinSupportCharacters.Contains(data.mCharaId));
                }
                else
                {
                    data.SetSelected(false);
                }
                
                data.SetSelectable(isMultiSelect);
            }
            
            scrollGrid.SetItems(charaList);

            // イベントの再登録
            scrollGrid.OnItemEvent -= SelectItemEvent;
            scrollGrid.OnItemEvent += SelectItemEvent;
            
            editRoot.SetActive(true);
        }
        
        private void SelectItemEvent(ScrollGridItem item, object o)
        {
            DebugChara3dScrollItem.Data data = (DebugChara3dScrollItem.Data)o;
            if (data.canMultipleSelect)
            {
                // マルチ選択
                data.SetSelected(!data.isSelected);
                scrollGrid.RefreshItemView();
            }
            else
            {
                // 選択して閉じる
                SetMainCharaId(data.mCharaId);
                CloseEdit();
            }
        }

        private void ApplyEdit()
        {
            // 選択されたキャラのリストを取得
            SetJoinCharaList(charaList.Where(data => data.isSelected).Select(data => data.mCharaId).ToArray());
            CloseEdit();
        }
        
        private void CloseEdit()
        {
            editRoot.SetActive(false);
        }
        
        private void SetMainCharaId(long mCharaId)
        {
            mainArguments.Pending.mCharaId = mCharaId;
            mainCharaName.text = MasterManager.Instance.charaMaster.FindData(mCharaId).name;
        }
        
        private void SetPracticeType(ObjectPoolUtility.TrainingTypeEnum practiceType)
        {
            // 練習に紐づく、最初にヒットした任意のカードを選択状態にする。
            foreach (TrainingCardMasterObject mCard in MasterManager.Instance.trainingCardMaster.values)
            {
                if (mCard.practiceType == (int)practiceType)
                {
                    mainArguments.TrainingCardId = mCard.id;
                    mainArguments.SelectedTrainingCardIndex = (int)practiceType;
                    break;
                }
            }
        }

        private void SetJoinCharaList(long[] array)
        {
            mainArguments.JoinSupportCharacters = array;
            joinCharaName.text = array == null ? "未設定" : string.Join("\n", array.Select(id => MasterManager.Instance.charaMaster.FindData(id).name));
        }

        private void LoadAllChara()
        {
            charaList.Clear();
            
            foreach (CharaMasterObject mChara in MasterManager.Instance.charaMaster.values)
            {
                // キャラじゃない場合、idが56,57、親キャラじゃない場合は除外
                if(mChara.cardType != CardType.Character || mChara.id == 56 || mChara.id == 57 || mChara.id != mChara.parentMCharaId) continue;
                charaList.Add(new DebugChara3dScrollItem.Data(mChara.id, false,false, mChara.name));
            }
        }
        
        /// <summary> トレーニングアニメーションを再生 </summary>
        private async UniTask PlayTrainingAnimationAsync(CancellationToken token = default)
        {
            // 開始処理
            // 3Dカメラの背景を表示
            uiRoot.SetActive(false);
            Camera3DUtility.Instance.ShowRenderTextureBackGround(true);
            trainingScene.PlayOpenAnimation(mainArguments, () => { });

            // ステート管理されないので指定時間だけ待機
            await UniTask.Delay(int.Parse(playTimeMills.text), cancellationToken: token);
            // スキップ用デバッグメニューの削除
            RemoveDebugMenu();
            
            // 終了処理
            CloseTrainingAnimation();
        }
        
        private void CloseTrainingAnimation()
        {
            TrainingCardMasterObject mCard = MasterManager.Instance.trainingCardMaster.FindData(mainArguments.TrainingCardId);
            
            trainingScene.PlayCloseAnimation(() => { });
            TagHelperUtility.SetEnable(TagHelperUtility.GroupEnum.Model3D, $"training_{mCard.practiceType}", false);
            Camera3DUtility.Instance.ShowRenderTextureBackGround(false);
            uiRoot.SetActive(true);
            
            // 溜まっていくのでモデルキャッシュの削除
            ObjectPoolUtility.ResetPool();
            Destroy(mainArguments.activeTimeline.gameObject);
        }
        
        /// <summary> デバッグ用のTrainingMainArgumentを生成 </summary>
        private TrainingMainArguments CreateDebugArguments()
        {
            TrainingProgressAPIResponse response = new TrainingProgressAPIResponse()
            {
                // 必要なデータだけ生成
                pending = new TrainingPending()
                {
                    // サポートの詳細情報
                    supportDetailList = new TrainingSupport[]{ },
                    // 練習の進行状況
                    nextGoalIndex = -1,
                },
                charaVariable = new TrainingCharaVariable(),
            };

            TrainingMainArguments arguments = new TrainingMainArguments(response, "デバッグアクション", new TrainingMainArgumentsKeeps());

            return arguments;
        }
        
        /// <summary> 選択したキャラでトレーニングを再生 </summary>
        private async UniTask PlayAsync()
        {
            cts = new CancellationTokenSource();
            
            await trainingScene.LoadResource(mainArguments, false);
            
            PlayTrainingAnimationAsync(cts.Token).Forget();
            
            // 停止用のデバッグメニュー
            CruFramework.DebugMenu.AddOption("Training3DPage", "Stop", () =>
            {
                // デバッグメニューの削除
                RemoveDebugMenu();
                
                CancelTask();
                CloseTrainingAnimation();
            });
        }

        private void CancelTask()
        {
            cts.Cancel();
            cts.Dispose();
            cts = null;
        }
        
        private void RemoveDebugMenu()
        {
            CruFramework.DebugMenu.RemoveOption("Training3DPage", "Stop");
        }
        
        
        /// <summary> UGUI </summary>
        public void OnChangePracticeType(int index)
        {
            SetPracticeType((ObjectPoolUtility.TrainingTypeEnum)index);
        }

        /// <summary> UGUI </summary>
        public void OnClickEditMainChara()
        {
            OpenCharaEdit(false);
        }
        
        /// <summary> UGUI </summary>
        public void OnClickEditJoinChara()
        {
            OpenCharaEdit(true);
        }
        
        /// <summary> UGUI </summary>
        public void OnClickClose()
        {
            CloseEdit();
        }

        /// <summary> UGUI </summary>
        public void OnClickApply()
        {
            ApplyEdit();
        }
        
        /// <summary> UGUI </summary>
        public void OnClickPlay()
        {
            PlayAsync().Forget();
        }

        /// <summary> UGUI </summary>
        public void OnClickBack()
        {
            AppManager.Instance.BackToTitle();
        }
    }
}
#endif