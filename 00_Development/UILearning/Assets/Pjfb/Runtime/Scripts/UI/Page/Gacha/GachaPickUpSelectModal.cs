using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using UnityEngine;
using TMPro;
using Pjfb.Storage;

namespace Pjfb.Gacha
{
    
    public class GachaPickUpSelectModal : ModalWindow {

        public class Param {
            public long choiceId = 0;
            public int index = 0;
            public float scrollPos = 0f;
            public GachaPickUpElementData pickUpData = null;
            public Action onUpdateSaveData = null;
            public Action<long,long,int,float> onClickSave = null;
            public List<long> selectedId = null;
            
        }

        [SerializeField]
        TextMeshProUGUI _title = null;
        [SerializeField]
        ScrollGrid _scrollGrid = null;
        [SerializeField]
        UIButton _updateButton = null;

        Param _param = null;

        List<long> _selectedId = null;
        GachaPickUpIcon.Param _selectedIconParam =  null;

        private List<CharacterDetailData> detailOrderList = new();
        
        protected override UniTask OnPreOpen(object args, CancellationToken token) {
            _param = (Param)args;
            _title.text = _param.pickUpData.name;

            _selectedId = new List<long>(_param.selectedId);
            _selectedIconParam = null;

            detailOrderList.Clear();
            int detailOrderIndex = 0;
            var index = 0;
            var data = new List<GachaPickUpIcon.Param>();
            Dictionary<int, GachaPickUpIcon.Param> dic = new();
            foreach( var characterData in _param.pickUpData.selectableCharacters ){
                GachaPickUpIcon.Param param = new GachaPickUpIcon.Param();
                param.characterData = characterData;
                param.onClickIcon = OnClickIcon;
                param.isSelected = _param.selectedId.Any(itr=>itr == characterData.id);
                param.index = index++;
                param.isNew = IsNew(characterData);
                param.initTime = Time.time;
                
                if (characterData.prize.args.mCharaId > 0)
                {
                    dic.Add(detailOrderIndex, param);
                    detailOrderList.Add(new CharacterDetailData(characterData.prize.args.mCharaId, 1, 0));
                    param.swipeableParams = new SwipeableParams<CharacterDetailData>(detailOrderList,
                        detailOrderIndex++,
                        (index) =>OnClickIcon(dic[index]));

                } 
                
                data.Add(param);
            }

            //ローカル保存
            SaveLocalGachaData();

            _scrollGrid.SetItems(data);
            UpdateButtonState();

            return default;
        }   

        public void OnClickCancel(){
            Close();
        }

        public void OnClickSave(){
            if( _selectedIconParam == null ) {
                return;
            }
            
            _param.onClickSave?.Invoke( _param.pickUpData.id, _selectedIconParam.characterData.id, _param.index, _param.scrollPos);
            Close();
        }


        public void OnClickIcon( GachaPickUpIcon.Param iconParam ){
            var isSelecting = _selectedIconParam != null && _selectedIconParam == iconParam;
            //一旦全部非選択
            foreach( var item in _scrollGrid.content.GetComponentsInChildren<Pjfb.Gacha.GachaPickUpIcon>(true) ){
                item.SetSelectFrameActive(false);
            }
            if( isSelecting ) {
                _selectedIconParam = null;
            } else {
                //　クリック時点でのparamを保存　*スクロールするとIconが再利用されParamが書き換えられるため
                _selectedIconParam = iconParam;
                // アイコン取得
                GachaPickUpIcon itemIcon = _scrollGrid.GetItem(_selectedIconParam) as GachaPickUpIcon;
                // アイコンがあれば選択状態にする
                if (itemIcon != null)
                {
                    itemIcon.SetSelectFrameActive(true);
                }
                else
                {
                    _selectedIconParam.isChoiced = true;
                }
            }
            UpdateButtonState();
        }

        void UpdateButtonState(){
            _updateButton.interactable = _selectedIconParam != null && !_selectedId.Any(itr=>itr == _selectedIconParam.characterData.id);
        }

        /// <summary>
        /// new表示するか
        /// </summary>
        bool IsNew(GachaPickUpSelectableCharacterData data){
            return !GachaUtility.IsConfirmedPickUpSelectableData(_param.choiceId, _param.pickUpData.id, data );
        }

        /// <summary>
        /// ローカルにガチャデータを保存
        /// </summary>
        void SaveLocalGachaData(){
            var gachaSaveData = LocalSaveManager.saveData.gachaData.lastCheckDate.FirstOrDefault(itr=> itr.choiceId == _param.choiceId && itr.elementId == _param.pickUpData.id );
            if( gachaSaveData == null ) {
                gachaSaveData = new GachaPickUpSaveData();
                gachaSaveData.choiceId = _param.choiceId;
                gachaSaveData.elementId = _param.pickUpData.id;
            }
            gachaSaveData.lastConfirmedDate = AppTime.Now.ToString();
            foreach( var characterData in _param.pickUpData.selectableCharacters ){
                //セーブデータに確認済みデータとして保存
                SetConfirmedPrizeIds(characterData, ref gachaSaveData);
            }

            LocalSaveManager.Instance.SaveData();

            _param.onUpdateSaveData?.Invoke();
        }

        /// <summary>
        /// 確認済みデータのIdセット
        /// </summary>
        void SetConfirmedPrizeIds( GachaPickUpSelectableCharacterData data, ref GachaPickUpSaveData saveData ){
            var prizeData = PrizeJsonUtility.GetPrizeContainerData(data.prize);
            var type = prizeData.itemIconType;
            long id = 0;
            if( type == ItemIconType.Character ) {
                id = data.prize.args.mCharaId;
            } else if ( type == ItemIconType.Item ){
                id = data.prize.args.mPointId;
            } else if( type == ItemIconType.SupportEquipment ){
                id = data.prize.args.variableTrainerMCharaId;
            }else {
                return;
            }

            saveData.confirmedPrizeIds.Add(id);
        }
    }
}
