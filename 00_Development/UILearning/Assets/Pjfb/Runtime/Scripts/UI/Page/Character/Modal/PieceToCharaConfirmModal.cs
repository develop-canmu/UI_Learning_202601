using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;

namespace Pjfb
{
    public class PieceToCharaConfirmModal : ModalWindow
    {
        public class Data
        {
            public readonly long MCharaId;
            public readonly long BeforePossessionValue;
            public readonly long AfterPossessionValue;
            public readonly Action<CharaPieceToCharaAPIResponse> OnSetCloseParameter;

            public Data(long mCharaId, long beforePossessionValue, long afterPossessionValue, Action<CharaPieceToCharaAPIResponse> onSetCloseParameter)
            {
                MCharaId = mCharaId;
                BeforePossessionValue = beforePossessionValue;
                AfterPossessionValue = afterPossessionValue;
                OnSetCloseParameter = onSetCloseParameter;
            }
        }
        
        [SerializeField] private CharacterPieceIcon characterPieceIcon;
        [SerializeField] private TMPro.TMP_Text description = null;
        [SerializeField] private TMPro.TMP_Text beforePossessionText;
        [SerializeField] private TMPro.TMP_Text afterPossessionText;

        private Data modalData;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            modalData = (Data)args;
            InitializeUi();
            return base.OnPreOpen(args, token);
        }

        private void InitializeUi()
        {
            characterPieceIcon.SetIconId(modalData.MCharaId);
            CharaMasterObject master = MasterManager.Instance.charaMaster.FindData(modalData.MCharaId);
            // 解放するカードタイプに応じて文言変更
            description.text = StringValueAssetLoader.Instance[$"character.piece_to_chara.content.card_type_{(int) master.cardType}"];
            beforePossessionText.text = modalData.BeforePossessionValue.ToString();
            afterPossessionText.text = modalData.AfterPossessionValue.ToString();
        }

        public void OnClickPieceToCharaButton()
        {
            PieceToCharaAPI().Forget();
        }

        public void OnClickCancelButton()
        {
            // 閉じる場合は更新しないので−１を送る
            SetCloseParameter(-1);
            Close();
        }
        
        private async UniTask PieceToCharaAPI()
        {
            CharaPieceToCharaAPIRequest request = new CharaPieceToCharaAPIRequest();
            CharaPieceToCharaAPIPost post = new CharaPieceToCharaAPIPost();
            post.mCharaId = modalData.MCharaId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            CharaPieceToCharaAPIResponse response = request.GetResponseData();
            modalData.OnSetCloseParameter?.Invoke(response);
            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => true);
            Close();
        }
    }
}