using System.Collections;
using System.Collections.Generic;

namespace Pjfb.InGame
{
    public enum BattleState
    {
        None,
        StartBattle,            // バトル開始時に一回だけ通るステート
        SelectRoundMember,      // 今回のラウンドの対象になるキャラクターの抽選とポジションの決定.
        KickOff,                // 初期ボール保有者の決定. スキル効果の適用とかやりそう.
        SetMarkTarget,          // 各キャラクターがマークする対象の決定.
        JustRunAction,          // マッチアップ発生までの走っている状態.
        SelectMatchUpAction,    // ボール保有者のマーク対象とのアクション選択.
        JudgeMatchUpPreResult,  // マッチアップ結果の判定.
        JudgeMatchUpFinalResult,// マッチアップ結果の判定.
        Finish,                 // 終了待機
    }
}