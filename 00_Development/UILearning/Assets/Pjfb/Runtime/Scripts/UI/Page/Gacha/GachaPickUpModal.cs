using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using UnityEngine;
using TMPro;
using Pjfb.Storage;

namespace Pjfb.Gacha
{
    //ピックアップ選択可能なキャラクターデータ
    public class GachaPickUpSelectableCharacterData{
        public long id{get;private set;} = 0;
        public PrizeJsonWrap prize{get;private set;} = null;
        public GachaPickUpSelectableCharacterData(MasterDetailContent content ){
            this.id = content.id;
            foreach( var prize in content.prizeList ){
                var prizeData = PrizeJsonUtility.GetPrizeContainerData(prize);
                var prizeType = prizeData.itemIconType;
                this.prize = prize;
                break;
            }
        }
    }

    //各項目事のピックアップデータ
    public class GachaPickUpElementData{
        public long id{get;private set;} = 0; // mGachaChoiceElementId
		public string name{get;private set;} = ""; // 表示名
		public long choiceCount{get;private set;} = 0; // 選択可能数
        public List<GachaPickUpSelectableCharacterData> selectableCharacters{get;private set;} = null; // 選択可能数
        public List<long> selectedPrizeContentIdList = new List<long>(); // ユーザーが選択したピックアップキャラのコンテントID

        public GachaPickUpElementData(MasterDetailElement element, ChoiceUserChoice selectedData ){
            this.id = element.id;
            this.name = element.name;
            this.choiceCount = element.choiceCount;
            selectableCharacters = new List<GachaPickUpSelectableCharacterData>();
            foreach( var content in element.contentList ){
                var data = new GachaPickUpSelectableCharacterData(content);
                selectableCharacters.Add(data);
            }

            if( selectedData != null ) {
                selectedPrizeContentIdList = new List<long>(selectedData.mCommonPrizeContentIdList);
            }
        }
    }

    //ピックアップデータルート
    public class GachaPickUpData{

        public Dictionary<long, GachaPickUpElementData> elements{get;private set;} = null;

        public GachaPickUpData(GachaGetChoiceDetailAPIResponse data ){
            elements = new Dictionary<long, GachaPickUpElementData>();

            foreach( var element in data.masterChoiceDetail.elementList ){
                ChoiceUserChoice selectedData = null;
                foreach( var userChoice in data.userChoiceList ){
                    if( userChoice.mGachaChoiceElementId == element.id ) {
                        selectedData = userChoice;
                        break;
                    }
                }   

                var elementData = new GachaPickUpElementData(element, selectedData);
                elements[element.id] = elementData;
            }
        }
    }


    public class GachaPickUpModal : ModalWindow {
        public class Param {
            public long gachaId = 0;
            public System.Action onUpdatePickup = null;
        }

        [SerializeField]
        GachaPickUpSelectedListItem _listItem = null;
        [SerializeField]
        Transform _scrollContent = null;
        [SerializeField]
        UIButton _updateButton = null;

        long _gachaId = 0;

        Dictionary<long, List<long>> _initSelectedIds = null;
        Dictionary<long, List<long>> _selectedIds = null;
        GachaPickUpData _pickUpData = null;
        List<GachaPickUpSelectedListItem> _listItems = new List<GachaPickUpSelectedListItem>();
        Dictionary<long, float> _scrollPosList = new Dictionary<long, float>();

        Param _param = null;

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            _param = (Param)args;
            _gachaId = _param.gachaId;

            //データがない場合のみ通信する
            if( _pickUpData == null ) {

                var request = new GachaGetChoiceDetailAPIRequest();
                var post = new GachaGetChoiceDetailAPIPost();
                post.mGachaChoiceId = _gachaId;
                request.SetPostData(post);
                await APIManager.Instance.Connect(request);
                var response = request.GetResponseData();

                //データ作成
                _pickUpData = new GachaPickUpData(response);
                _selectedIds = new Dictionary<long, List<long>>();
                _initSelectedIds = new Dictionary<long, List<long>>();
                
                foreach( var element in _pickUpData.elements.Values ){
                    // scrollPosの初期
                    _scrollPosList[element.id] = 0f;

                    var initList = new List<long>();
                    for( int i=0; i<element.choiceCount; ++i ){
                        if( element.selectedPrizeContentIdList.Count <= i ) {
                            initList.Add(0);
                        }else {
                            initList.Add(element.selectedPrizeContentIdList[i]);
                        }
                    }

                    _selectedIds[element.id] = new List<long>(initList);
                    _initSelectedIds[element.id] = new List<long>(initList);
                    
                    //ローカルセーブ処理
                    //存在しない場合はデータだけ作成しておく
                    var gachaSaveData = LocalSaveManager.saveData.gachaData.lastCheckDate.FirstOrDefault(itr=> itr.choiceId == _gachaId && itr.elementId == element.id );
                    if( gachaSaveData == null ) {
                        var addDate = new GachaPickUpSaveData();
                        addDate.choiceId = _gachaId;
                        addDate.elementId = element.id;
                        addDate.lastConfirmedDate = "";
                        addDate.confirmedPrizeIds = new List<long>();
                        LocalSaveManager.saveData.gachaData.lastCheckDate.Add(addDate);
                        LocalSaveManager.Instance.SaveData();
                    }
                }

                CreateList();
            }
            UpdateUpdateButtonState();
        }   



        public void OnClickUpdate(){
            if ( !IsChangePickUp() ) {
                Close();
                return;
            }

            UpdateSelectedCharacter().Forget();
        }

        public void OnClickCancel(){
            if ( IsChangePickUp() ) {
                ConfirmModalData data = new ConfirmModalData(
                    StringValueAssetLoader.Instance["common.confirm"],
                    StringValueAssetLoader.Instance["gacha.pickup_cancel_confirm"],
                    null,
                    new ConfirmModalButtonParams(StringValueAssetLoader.Instance["gacha.dispose_pickup"], (window) => {AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop( (itr)=> itr != window ); window.Close();}),
                    new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.cancel"], window => window.Close())
                );
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
                return;
            }
            Close();
        }

        public async UniTask UpdateSelectedCharacter(){
            var request = new GachaChoiceAPIRequest();
            var post = new GachaChoiceAPIPost();
            post.mGachaChoiceId = _gachaId;

            var choices = new List<ChoiceUserChoice>();
            foreach( var pair in _selectedIds ){
                var choice = new ChoiceUserChoice();
                choice.mGachaChoiceElementId = pair.Key;
                choice.mCommonPrizeContentIdList = pair.Value.ToArray();
                choices.Add(choice);
            }
            post.userChoiceList =  choices.ToArray();
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            
            _param.onUpdatePickup?.Invoke();
            Close();
        }

        /// <summary>
        /// 各項目の更新処理
        /// </summary>
        public void UpdateList(){
            foreach( var item in _listItems ){
                item.UpdateView();
            }
        }

        /// <summary>
        /// 各項目作成
        /// </summary>
        void CreateList(){
            
            foreach( var item in _listItems ){
                Destroy(item.gameObject);
            }
            _listItems.Clear();
            
            foreach( var element in _pickUpData.elements.Values ){
                var item = Instantiate<GachaPickUpSelectedListItem>(_listItem, _scrollContent);
                var selectedId = new List<long>();
                if( _selectedIds.ContainsKey(element.id) ) {
                    selectedId = _selectedIds[element.id];
                }
                
                item.Init(_gachaId, element, selectedId, UpdateList, OnSelected, _scrollPosList[element.id]);
                _listItems.Add(item);
            }
        }

        void OnSelected( long id, long selectedId, int index, float scrollPos ){
            _selectedIds[id][index] = selectedId;
            _scrollPosList[id] = scrollPos;
            CreateList();
            UpdateUpdateButtonState();
        }

        //ピックアップが変更されたか
        bool IsChangePickUp(){
            foreach( var key in _initSelectedIds.Keys ){
                if( _selectedIds[key].Count != _initSelectedIds[key].Count ) {
                    return true;
                }
                
                foreach( var id in _initSelectedIds[key] ){
                    if ( !_selectedIds[key].Any( itr => itr == id ) ){
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 更新ボタン状態更新
        /// </summary>
        void UpdateUpdateButtonState(){
            _updateButton.interactable = IsChangePickUp();
        }
    }
}
