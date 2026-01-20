using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using UnityEngine.UI;

namespace Pjfb
{
    public class PracticeSkillViewLabel : MonoBehaviour
    {
        [SerializeField] private PracticeSkillViewLotteryStateLabel stateLabel;
        [SerializeField] private PracticeSkillViewEffectLabel effectLabel;
        [SerializeField] private PracticeSkillViewLotteryMethodLabel lotteryMethodLabel;
        [SerializeField] private bool isAlwaysActive;
        
        public void Init(PracticeSkillLotteryInfo lotteryInfo, bool isShow)
        {
            // いったん全てのラベルをオフに
            HideAllLabel();
            
            if (isShow)
            {
                ShowLabel(lotteryInfo);
                
                // SupportEquipmentRedrawingModalではスキルの表示間隔を一定にするためにLabelは常に表示する
                if (isAlwaysActive)
                {
                    this.gameObject.SetActive(true);
                }
                else
                {
                    // 一つでもラベルがアクティブならLabel表示
                    this.gameObject.SetActive(IsAnyActiveLabel());
                }
            }
        }

        //全てのラベルをオフ
        private void HideAllLabel()
        {
           stateLabel.HideLabel();
           effectLabel.HideLabel();
           lotteryMethodLabel.HideLabel();
        }

        // 一つでもラベルがアクティブか
        private bool IsAnyActiveLabel()
        {
            return stateLabel.IsAnyActiveLabel() || effectLabel.IsAnyActiveLabel() || lotteryMethodLabel.IsAnyActiveLabel();
        }
        
        // ラベル表示
        private void ShowLabel(PracticeSkillLotteryInfo lotteryInfo)
        {
            SetLotteryStateLabel(lotteryInfo);
            SetLotteryEffectLabel(lotteryInfo);
            SetLotteryMethodLabel(lotteryInfo);
        }

        private void SetLotteryStateLabel(PracticeSkillLotteryInfo lotteryInfo)
        {
            // 上書きできるなら
            if (lotteryInfo.CanReloadOverwrite())
            {
                // リロードマスターがnullなら表示しない
                if (lotteryInfo.ReloadMaster == null)
                {
                    return;
                }
                
                // テーブル指定以外
                if (lotteryInfo.ReloadMaster.reloadType != PracticeSkillLotteryReloadType.SelectTable)
                {
                    if (lotteryInfo.IsResult)
                    {
                      SetLotteryResultStateLabel(lotteryInfo);
                    }
                    
                    //上書き不可以外はすべて抽選対象
                    else
                    {
                        stateLabel.SetStateLabel(SupportEquipmentLotterySkillStateType.RedrawingSubject);
                    }
                }
            }
            // 上書き不可
            else
            {
                stateLabel.SetStateLabel(SupportEquipmentLotterySkillStateType.OverwriteProhibited);
            }

        }

        private void SetLotteryResultStateLabel(PracticeSkillLotteryInfo lotteryInfo)
        {
            // リザルト画面の効果値抽選の場合は表示しない
            if (lotteryInfo.ReloadMaster.reloadType != PracticeSkillLotteryReloadType.SelectValue)
            {
                // (リザルト)再抽選された番号と一致しているならラベルを表示
                if (lotteryInfo.UpdateNumberList.Contains(lotteryInfo.SlotNumber))
                {
                    stateLabel.SetStateLabel(SupportEquipmentLotterySkillStateType.Redrawing);
                }
            }
        }

        // 効果値ラベルの表示(効果値ラベルはリザルト画面でのみ表示)
        private void SetLotteryEffectLabel(PracticeSkillLotteryInfo lotteryInfo)
        {
            // リロードマスターがnullなら表示しない
            if (lotteryInfo.ReloadMaster == null)
            {
                return;
            }
            // 抽選タイプが効果値抽選でリザルト画面のときのみ表示
            if (lotteryInfo.ReloadMaster.reloadType == PracticeSkillLotteryReloadType.SelectValue && lotteryInfo.IsResult)
            {
                // 効果値が上昇したなら
                if (lotteryInfo.SkillInfo.Value > lotteryInfo.PreviewSkillInfo.Value)
                {
                    effectLabel.SetEffectLabel(SupportEquipmentLotterySkillEffectType.EffectUp);
                }
                // 効果値が減少したら
                else if (lotteryInfo.SkillInfo.Value < lotteryInfo.PreviewSkillInfo.Value)
                {
                    effectLabel.SetEffectLabel(SupportEquipmentLotterySkillEffectType.EffectDown);
                }
                // 効果値が変わらないなら
                else
                {
                    effectLabel.SetEffectLabel(SupportEquipmentLotterySkillEffectType.EffectKeep);
                }
                effectLabel.SetEffectLabel(lotteryInfo.GetLotteryEffectType());
            }
        }

        // テーブル抽選時の表示
        private void SetLotteryMethodLabel(PracticeSkillLotteryInfo lotteryInfo)
        {
            // 上書きできないなら
            if (!lotteryInfo.CanReloadOverwrite())
            {
                return;
            }
            
            // リロードマスターがnullなら表示しない
            if (lotteryInfo.ReloadMaster == null)
            {
                return;
            }
            // 抽選タイプがテーブル抽選の場合にラベルを設定
            if (lotteryInfo.ReloadMaster.reloadType == PracticeSkillLotteryReloadType.SelectTable)
            {
                // マスタ検索
                var mCharaTrainerLotteryReloadDetailList = MasterManager.Instance.charaTrainerLotteryReloadDetailMaster
                    .FindDetailGroupId(lotteryInfo.ReloadMaster.mCharaTrainerLotteryDetailGroupId)
                    .OrderBy(x => x.number).ToList();

                // マスタが見つからない
                if (mCharaTrainerLotteryReloadDetailList.Count == 0)
                {
                    CruFramework.Logger.LogError(
                        $"Not found charaTrainerLotteryReloadDetailMaster Type {lotteryInfo.ReloadMaster.mCharaTrainerLotteryDetailGroupId}");
                }
                else
                {
                    foreach (var reloadDetail in mCharaTrainerLotteryReloadDetailList)
                    {
                        // スキルの枠番号が一致しているならラベルを表示する
                        if (reloadDetail.number == lotteryInfo.SlotNumber)
                        {
                            switch (reloadDetail.lotteryType)
                            {
                                case PracticeSkillLotteryReloadDetailType.None:
                                    lotteryMethodLabel.SetLotteryMethodLabel(SupportEquipmentLotterySkillMethodType.None, reloadDetail.text);
                                    break;
                                //  練習能力の再抽選なら確定ラベル表示
                                case PracticeSkillLotteryReloadDetailType.SelectSkill:
                                    lotteryMethodLabel.SetLotteryMethodLabel(SupportEquipmentLotterySkillMethodType.Decision, reloadDetail.text);
                                    break;
                                //　効果量の再抽選なら抽選確率Upラベル表示
                                case PracticeSkillLotteryReloadDetailType.SelectValue:
                                    lotteryMethodLabel.SetLotteryMethodLabel(SupportEquipmentLotterySkillMethodType.LotteryProbability, reloadDetail.text);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                    }
                }
            }
        }
    }
}