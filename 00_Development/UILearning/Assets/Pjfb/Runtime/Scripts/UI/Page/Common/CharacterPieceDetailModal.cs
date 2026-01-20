using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.UserData;
using TMPro;
using UnityEngine;

namespace Pjfb
{
    public class CharacterPieceDetailModal : ModalWindow
    {
        #region Params
        public class WindowParams
        {
            public long MCharaId;
        }

        [SerializeField] private CharacterPieceIcon characterPieceIcon;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] [StringValue] private string characterPieceTitle; 
        [SerializeField] [StringValue] private string specialSupportCardPieceTitle; 
        [SerializeField] [StringValue] private string adviserPieceTitle; 
        private WindowParams _windowParams;
        
        #endregion

        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.CharacterPieceDetail, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            Init();
            return base.OnPreOpen(args, token);
        }
        private void Init()
        {
            long mCharaId = _windowParams.MCharaId;
            UserDataCharaPiece charaPiece = UserDataManager.Instance.charaPiece.Find(mCharaId);
            CharaMasterObject mChara = MasterManager.Instance.charaMaster.FindData(mCharaId);
            characterPieceIcon.SetIconId(mCharaId);
            titleText.text = mChara.cardType switch
            {
                CardType.Character => StringValueAssetLoader.Instance[characterPieceTitle],
                CardType.SpecialSupportCharacter => StringValueAssetLoader.Instance[specialSupportCardPieceTitle],
                CardType.Adviser => StringValueAssetLoader.Instance[adviserPieceTitle],
                _ => string.Empty
            };
            nameText.text = $"{mChara.nickname}  {mChara.name}";
            countText.text =  (charaPiece?.value ?? 0).ToString();
        }
    }
}