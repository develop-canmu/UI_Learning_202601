using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using TMPro;

namespace Pjfb
{
    public class MerchandiseListItemUi : MonoBehaviour
    {
        [SerializeField] private PrizeJsonView prizeJsonView;
        [SerializeField] private TMP_Text nameText;

        public void InitializeUi(PrizeJsonWrap wrap, bool isPaid)
        {
            prizeJsonView.SetView(wrap);
            var id = wrap.type switch
            {
                "point" => wrap.args.mPointId,
                "chara" => wrap.args.mCharaId,
                "charaPiece" => wrap.args.pieceMCharaId,
                "charaVariable" => wrap.args.variableMCharaId,
                "charaVariableTrainer" => wrap.args.variableTrainerMCharaId,
                "icon" => wrap.args.mIconId,
                "title" => wrap.args.mTitleId,
                "chatStamp" => wrap.args.mChatStampId,
                _ => 0
            };
            SetNameText(wrap.type, id, isPaid, wrap.args.lockId);
        }
        
        public void InitializeUi(Networking.App.Request.PrizeJsonWrap wrap)
        {
            prizeJsonView.SetView(wrap);
            var id = wrap.type switch
            {
                "point" => wrap.args.mPointId,
                "chara" => wrap.args.mCharaId,
                "charaPiece" => wrap.args.pieceMCharaId,
                "charaVariable" => wrap.args.variableMCharaId,
                "charaVariableTrainer" => wrap.args.variableTrainerMCharaId,
                "icon" => wrap.args.mIconId,
                "title" => wrap.args.mTitleId,
                "chatStamp" => wrap.args.mChatStampId,
                _ => 0
            };
            SetNameText(wrap.type, id, false, wrap.args.lockId);
        }

        private void SetNameText(string type, long id, bool isPaid, long lockId)
        {
            switch (type)
            {
                case "point":
                {
                    var mPoint = MasterManager.Instance.pointMaster.FindData(id);
                    if(mPoint == null) break;
                    var sb = new StringBuilder();
                    nameText.text = isPaid
                        ? sb.AppendFormat(StringValueAssetLoader.Instance["shop.value.paid"], mPoint.name).ToString()
                        : mPoint.name;
                    break;
                }
                case "chara":
                {
                    var mChara = MasterManager.Instance.charaMaster.FindData(id);
                    if(mChara == null) break;
                    nameText.text = mChara.name;
                    break;
                }
                case "charaPiece":
                {
                    var mChara = MasterManager.Instance.charaMaster.FindData(id);
                    if(mChara == null) break;
                    nameText.text = mChara.name;
                    break;
                }
                case "charaVariable":
                {
                    var mChara = MasterManager.Instance.charaMaster.FindData(id);
                    if(mChara == null) break;
                    nameText.text = mChara.name;
                    break;
                }
                case "charaVariableTrainer":
                {
                    var mChara = MasterManager.Instance.charaMaster.FindData(id);
                    if(mChara == null) break;
                    nameText.text = mChara.name;
                    break;
                }
                case "icon":
                {
                    var mIcon = MasterManager.Instance.iconMaster.FindData(id);
                    if (mIcon == null) break;
                    nameText.text = mIcon.name;
                    break;
                }
                case "title":
                {
                    var mTitle = MasterManager.Instance.titleMaster.FindData(id);
                    if (mTitle == null) break;
                    nameText.text = mTitle.name;
                    break;
                }
                case "chatStamp":
                {
                    // TODO:チャットスタンプ実装時に再度対応
                    var mChatStamp = MasterManager.Instance.chatStampMaster.FindData(id);
                    if (mChatStamp == null) break;
                    nameText.text = mChatStamp.name.ToString();
                    break;
                }
                    
            }

            if (!string.IsNullOrEmpty(nameText.text) && lockId > 0) nameText.text = $"{PrizeJsonUtility.LockedText}{nameText.text}";
        }
    }
}