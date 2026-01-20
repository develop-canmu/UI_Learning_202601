using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.InGame;
using System;

namespace Pjfb
{
    public class ClubRoyalBattleLogMessageScrollGridItem : BattleLogMessageScrollGridItem
    {
        // 継承時に機能が変更したものがいくつかある
        // Text：スキルエフェクトのテキストを表示
        // TextWithIcon：
        protected const string OpenAnimationTrigger = "Open";

        protected override void OnSetView(object value)
        {
            BattleLog data = (BattleLog)value;
            FrameImages[0].sprite = FrameTextures[0];

            gameObject.SetActive(true);
            Animator.SetTrigger(OpenAnimationTrigger);

            if (data.IconMCharaId > 0)
            {
                CharacterIcon.gameObject.SetActive(true);
                CharacterIcon.SetTextureAsync(data.IconMCharaId).Forget();
            }
            else
            {
                CharacterIcon.gameObject.SetActive(false);
            }

            string[] messages = data.MessageLog.Split(',');
            string skillNameString = messages[0];
            string useMessageString = messages[1];

            Text.text = skillNameString;
            TextWithIcon.text = useMessageString;
        }
    }
}