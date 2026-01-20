using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;

namespace Pjfb.Character
{
    public class CharacterLiberationConfirmModal : CharacterLevelUpConfirmModalBase<CharacterLiberationConfirmModal.LiberationData>
    {
        private static readonly string LvStringValueKey = "character.status.lv_value";
        public class LiberationData : Data
        {
            public readonly long MCharaId;
            public readonly long CurrentMaxGrowthLv;
            public readonly long AfterMaxGrowthLv;
            public readonly long PossessionValue;
            public readonly long RequiredValue;

            public LiberationData(long userCharacterId, long mCharaId, long currentLv, long afterLv, long currentMaxGrowthLv,
                long afterMaxGrowthLv, long possessionValue,
                long requiredValue)
                : base(userCharacterId, currentLv, afterLv)

            {
                MCharaId = mCharaId;
                CurrentMaxGrowthLv = currentMaxGrowthLv;
                AfterMaxGrowthLv = afterMaxGrowthLv;
                PossessionValue = possessionValue;
                RequiredValue = requiredValue;
            }
        }

        
        [SerializeField] private GameObject maxGrowthLvRoot;
        [SerializeField] private TMPro.TMP_Text currentMaxGrowthLvText;
        [SerializeField] private TMPro.TMP_Text afterMaxGrowthLvText;
        [SerializeField] protected CharacterPieceIcon characterPieceIcon;


        
  

        protected override void InitializeUi()
        {
            // モーダルタイトルのセット
            CardType cardType = MasterManager.Instance.charaMaster.FindData(modalData.MCharaId).cardType;
            modalTitle.text = StringValueAssetLoader.Instance[$"character.card_type_{(int)cardType}.liberation.confirm.title"];
            
            var sb = new StringBuilder();
            currentLvText.text = sb.AppendFormat(StringValueAssetLoader.Instance[LvStringValueKey], modalData.CurrentLv).ToString();
            sb.Clear();
            afterLvText.text = sb.AppendFormat(StringValueAssetLoader.Instance[LvStringValueKey], modalData.AfterLv).ToString();
            maxGrowthLvRoot.SetActive(modalData.CurrentMaxGrowthLv != modalData.AfterMaxGrowthLv);
            sb.Clear();
            currentMaxGrowthLvText.text = sb.AppendFormat(StringValueAssetLoader.Instance[LvStringValueKey], modalData.CurrentMaxGrowthLv).ToString();
            sb.Clear();
            afterMaxGrowthLvText.text = sb.AppendFormat(StringValueAssetLoader.Instance[LvStringValueKey], modalData.AfterMaxGrowthLv).ToString();
            SetRequiredItem();
        }

        private void SetRequiredItem()
        {
            characterPieceIcon.SetIconId(modalData.MCharaId);
            characterPieceIcon.SetCountDigitString(modalData.PossessionValue, modalData.RequiredValue);
        }
        
        
        protected override async UniTask CallApi()
        {
            CharaLiberationAPIRequest request = new CharaLiberationAPIRequest();
            CharaLiberationAPIPost post = new CharaLiberationAPIPost();
            post.uCharaId = modalData.UserCharacterId;
            post.level = modalData.AfterLv;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            // 閉じた時に受け取るデータセット
            SetCloseParameter(request.GetResponseData());
            Close();
        }
    }
}