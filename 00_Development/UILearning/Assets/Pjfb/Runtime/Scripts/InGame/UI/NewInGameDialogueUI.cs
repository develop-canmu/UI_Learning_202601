using System;
using System.Collections.Generic;
using Pjfb.Master;
using Pjfb.Voice;
using UnityEngine;
using UnityEngine.Playables;

namespace Pjfb.InGame
{
    public class NewInGameDialogueUI : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        public Animator Animator
        {
            get { return animator; }
        }
        
        [SerializeField] private List<NewInGameDialogueParts> partsList;

        private List<BattleDialogueData> dialogData;
        private int nextPlayVoiceIndex;
        private int endedPlayVoiceIndex;
        private int requestedPlayCount;
        private NewInGameDialogueParts playingDialogPart;
        private bool isPlaySameTeamDialog;

        public bool IsEndAllVoice => (this != null && !gameObject.activeSelf) || endedPlayVoiceIndex >= dialogData?.Count;
        public Action OnEndCallback;
        private bool isAllyCaptain;

        private const string OpenBlue1Key = "OpenBlue1";
        private const string OpenBlue2Key = "OpenBlue2";
        private const string OpenRed1Key = "OpenRed1";
        private const string OpenRed2Key = "OpenRed2";
        private const string OpenBlue1Red1Key = "OpenBlue1Red1";
        private const string OpenRed1Blue1Key = "OpenRed1Blue1";
        private readonly int CloseHash = Animator.StringToHash("Base Layer.Close");
        
        public void SetDialogueData(List<BattleDialogueData> dialogueDataList, bool _isAllyCaptain)
        {
            OnEndCallback = null;
            dialogData = dialogueDataList;
            nextPlayVoiceIndex = 0;
            endedPlayVoiceIndex = 0;
            requestedPlayCount = 0;
            isAllyCaptain = _isAllyCaptain;
        }

        public void PlayDialog()
        {
            PlayVoice(nextPlayVoiceIndex);
        }

        public void CloseDialog()
        {
            if (OnEndCallback != null)
            {
                OnEndCallback.Invoke();
                OnEndCallback = null;
            }

            // そもそもモノがない場合はクローズアニメーションもなし.
            if (dialogData == null || dialogData.Count == 0)
            {
                return;
            }

            // まだ再生するやつがある場合はアニメーションが喧嘩するのでやむなく再生完了状態に強制
            var normalizedTime = endedPlayVoiceIndex >= dialogData.Count ? 0.0f : 1.0f;
            Close(normalizedTime);
        }
        
        public void ClosedDialog()
        {
            if (OnEndCallback != null)
            {
                OnEndCallback.Invoke();
                OnEndCallback = null;
            }

            Close(1.0f);
        }

        public void Close(float normalizedTime)
        {
            var anim = animator.GetCurrentAnimatorStateInfo(0);
            // close状態からのcloseでUIがちらついてしまうため制限
            if (anim.fullPathHash == CloseHash)
            {
                return;
            }
            animator.Play(CloseHash, 0, normalizedTime);
        }

        public void CancelPlayVoice()
        {
            dialogData?.Clear();
            playingDialogPart?.CancelPlayVoice();
            ClosedDialog();
        }

        private string GetNextTrigger(int index)
        {
            if (index > dialogData.Count)
            {
                return string.Empty;
            }

            var first = dialogData[index];
            var second = index + 1 < dialogData.Count ? dialogData[index + 1] : null;
            
            // 一個しか出ないパターン
            if (second?.master == null)
            {
                return first.isPlayer ? OpenBlue1Key : OpenRed1Key;
            }

            isPlaySameTeamDialog = first.isPlayer == second.isPlayer;

            if (first.isPlayer)
            {
                return second.isPlayer ? OpenBlue2Key : OpenBlue1Red1Key;
            }
            else
            {
                return second.isPlayer ? OpenRed1Blue1Key : OpenRed2Key;
            }
        }

        public void PlayVoice(int index)
        {
            if (dialogData == null || index >= dialogData.Count)
            {
                CloseDialog();
                return;
            }
            
            // 再生の必要なし
            if (index < 0 || index >= dialogData.Count || dialogData[index].master == null)
            {
                // partsList[index % 2].PlayDialog(null);
                nextPlayVoiceIndex++;
                OnEndPlayVoice();
                return;
            }
            
            // 奇数個目の再生時のみアニメーション指定
            if (index % 2 == 0)
            {
                var trigger = GetNextTrigger(index);
                if (!string.IsNullOrEmpty(trigger))
                {
                    animator.SetTrigger(trigger);
                }
            }

            // 前のが再生中
            if (index > endedPlayVoiceIndex)
            {
                requestedPlayCount++;
                return;
            }

            playingDialogPart = GetDialogPart(index, dialogData[index]);
            playingDialogPart.PlayDialog(dialogData[index]);
            // 倍速のときはボイス再生なし (の代わりにアニメーションのイベントでOnEndPlayVoiceをフック). スキップ等のときはそもそもダイジェストが実行されないので大丈夫.
            if (isAllyCaptain && !BattleDataMediator.Instance.IsDoubleSpeed)
            {
                playingDialogPart.PlayVoice(OnEndPlayVoice);
            }

            animator.speed = isAllyCaptain ? BattleDataMediator.Instance.PlaySpeed : BattleDataMediator.Instance.PlaySpeed * BattleConst.AdjustableValueNonDigestPlaySpeed;
            
            nextPlayVoiceIndex++;
        }

        private NewInGameDialogueParts GetDialogPart(int index, BattleDialogueData data)
        {
            if (index % 2 == 0 || !isPlaySameTeamDialog)
            {
                return data.isPlayer ? partsList[0] : partsList[2];
            }
            else
            {
                return data.isPlayer ? partsList[1] : partsList[3];
            }
        }

        private void OnEndPlayVoice()
        {

            // ボイスだけ再生されてダイアログ破棄されてるならreturn
            if (this == null)
            {
                return;
            }
            
            animator.speed = isAllyCaptain ? BattleDataMediator.Instance.PlaySpeed : BattleDataMediator.Instance.PlaySpeed * BattleConst.AdjustableValueNonDigestPlaySpeed;
            endedPlayVoiceIndex++;
            
            // 二個一組のボイス(セリフ)を再生しきったらクローズ
            if (endedPlayVoiceIndex % 2 == 0)
            {
                CloseDialog();
            }

            // ボイス(というかセリフ)は2個で1組になっているため, 1つ目を再生しきった状態だったら次のを自動で再生する.
            if (endedPlayVoiceIndex % 2 == 1)
            {
                PlayVoice(endedPlayVoiceIndex);
                return;
            }
            
            // リクエストされていた状態だったら次を再生
            if (requestedPlayCount > 0)
            {
                requestedPlayCount--;
                PlayVoice(endedPlayVoiceIndex);
            }
        }

        /// <summary>
        /// Called by animation event.
        /// </summary>
        public void OnEndOpenDialog()
        {
            if (!playingDialogPart.IsPlayedVoice)
            {
                OnEndPlayVoice();
            }
        }

        /// <summary>
        /// Called by animation event.
        /// </summary>
        public void OnPauseTillEndVoice(int index)
        {
            if (playingDialogPart.IsPlayedVoice && endedPlayVoiceIndex % 2 == 0 && index == 0)
            {
                animator.speed = 0.0f;
            }
        }
    }
}