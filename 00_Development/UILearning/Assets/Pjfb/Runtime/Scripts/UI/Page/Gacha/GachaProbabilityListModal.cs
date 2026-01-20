using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using PrizeJsonWrap = Pjfb.Networking.App.Request.PrizeJsonWrap;

namespace Pjfb.Gacha
{
    public class GachaProbabilityData {
		public string name = ""; // 名称
		public long count = 0; // 排出個数
        public List<GachaProbabilityGroup> groups = null;
        public GachaProbabilityData( ProbabilityTable table ){
            name = table.name;
            count = table.count;
            groups = new List<GachaProbabilityGroup>();
            foreach( var group in table.frameGroupList ) {
                groups.Add( new GachaProbabilityGroup(group) );
            }   
        }
    }

    public class GachaProbabilityGroup {
        public string name = "";
		public long count = 0; 
		public double percentage = 0; // 各排出品の排出確率を合計したものの百分率表現
        public List<GachaProbabilityFrame> frames = null;
        public GachaProbabilityGroup( ProbabilityFrameGroup group ){
            name = group.name;
            count = group.count;
            percentage = group.percentage;
            frames = new List<GachaProbabilityFrame>();
            foreach( var frame in group.frameList ){
                frames.Add( new GachaProbabilityFrame(frame) );
            }
        }
    }

    public class GachaProbabilityFrame {
        public long id = 0; // m_common_prize_frame.id
		public string name = ""; // 枠名称
		public double percentage = 0.0; // この枠に内包されるピックアップ確率
		public double choicePercentage = 0.0; // この枠が締める合計排出率
        public List<GachaProbabilityContent> contents = null;
        public GachaProbabilityFrame( ProbabilityFrame frame ){
            id = frame.id;
            name = frame.name;
            percentage = frame.percentage;
            choicePercentage = frame.choicePercentage;

            contents = new List<GachaProbabilityContent>();
            foreach( var content in frame.contentList ){
                contents.Add( new GachaProbabilityContent(content) );
            }
        }
    }

    public class GachaProbabilityContent {
        public long id = 0; // m_common_prize_content.id
		public double percentage = 0; // 排出確率の百分率表現
		public long picked = 0; // ピックアップされたかどうか
        public string name = "";
        public GachaProbabilityContent( ProbabilityContent content ){
            id = content.id;
            percentage = content.percentage;
            picked = content.picked;
            
            foreach( var prize in content.prizeList ){
                var valText = "";
                if( prize.args.value > 1 ) {
                    valText = " x " + prize.args.value;
                }

                var findName = FindName(prize);
                if( !string.IsNullOrEmpty(findName) ){
                    name = findName + valText;
                    break;
                }
            }
        }

        string FindName( PrizeJsonWrap prize ){
            var findName = string.Empty;
            var prizeData = PrizeJsonUtility.GetPrizeContainerData(prize);
            var type = prizeData.itemIconType;
            if( type == ItemIconType.Character ){
                var character = Master.MasterManager.Instance.charaMaster.FindData( prize.args.mCharaId );
                if( character != null ) {
                    if( !string.IsNullOrEmpty(character.nickname) ) {
                        findName = character.nickname + character.name;
                    } else {
                        findName = character.name;
                    }
                    // Exキャラ
                    if(character.isExtraSupport){
                        findName = StringValueAssetLoader.Instance["gacha.extra_support_prefix"] + findName;
                    }
                }
            } else if ( type == ItemIconType.Item ){
                var point = Master.MasterManager.Instance.pointMaster.FindData( prize.args.mPointId );
                if( point != null ) {
                    findName = point.name;
                }
            } else if ( type == ItemIconType.CharacterPiece ){
                var character = Master.MasterManager.Instance.charaMaster.FindData( prize.args.pieceMCharaId );
                
                if( character != null ) {
                    if( !string.IsNullOrEmpty(character.nickname) ) {
                        findName = character.nickname + character.name;
                    } else {
                        findName = character.name;
                    }
                    findName = string.Format(StringValueAssetLoader.Instance["gacha.probability_chara_piece"], findName);
                }
            } else if ( type == ItemIconType.SupportEquipment ){
                var character = Master.MasterManager.Instance.charaMaster.FindData( prize.args.variableTrainerMCharaId );
                if( character != null ) {
                    if( !string.IsNullOrEmpty(character.nickname) ) {
                        findName = character.nickname + character.name;
                    } else {
                        findName = character.name;
                    }
                }
            } 
            else if ( type == ItemIconType.ProfilePart) 
            {
                ProfilePartMasterObject profilePartMaster = MasterManager.Instance.profilePartMaster.FindData(prize.args.mProfilePartId);
                switch (profilePartMaster.partType)
                {
                    case ProfilePartMasterObject.ProfilePartType.ProfileFrame:
                        findName = MasterManager.Instance.profileFrameMaster.FindData(profilePartMaster.imageId).name;
                        break;
                    case ProfilePartMasterObject.ProfilePartType.DisplayCharacter:
                        findName = MasterManager.Instance.profileCharaMaster.FindData(profilePartMaster.imageId).name;
                        break;
                    case ProfilePartMasterObject.ProfilePartType.Emblem:
                        findName = MasterManager.Instance.emblemMaster.FindData(profilePartMaster.imageId).name;
                        break;
                    // 未実装なのでエラー投げる
                    case ProfilePartMasterObject.ProfilePartType.ProfileVoice:
                    case ProfilePartMasterObject.ProfilePartType.DisplayCharacterBackground:
                        throw new NotImplementedException();
                }
            }
            return findName;
        }
    }

    public class GachaProbabilityListModal : ModalWindow
    {
        
        [SerializeField]
        ScrollGrid _scroll = null;

        [SerializeField]
        TextMeshProUGUI _title = null;
    

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            var gachaId = (long)args;

            GachaProbabilityDetailAPIRequest request = new GachaProbabilityDetailAPIRequest();
            GachaProbabilityDetailAPIPost post = new GachaProbabilityDetailAPIPost();
            post.mGachaSettingId = gachaId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            var probabilityDataList = new List<GachaProbabilityData>();
            foreach( var table in response.probabilityInfo.tableList ){
                var data = new GachaProbabilityData(table);
                probabilityDataList.Add(data);
            }
            _scroll.SetItems(probabilityDataList);
            _title.text = response.probabilityInfo.name;
        }   

        public void OnClickClose(){
            Close();
        }
    }
}
