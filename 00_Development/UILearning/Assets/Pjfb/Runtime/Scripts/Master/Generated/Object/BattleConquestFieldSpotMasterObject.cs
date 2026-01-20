//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class BattleConquestFieldSpotMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mBattleConquestFieldId {get{ return mBattleConquestFieldId;} set{ this.mBattleConquestFieldId = value;}}
		[MessagePack.Key(2)]
		public long _positionX {get{ return positionX;} set{ this.positionX = value;}}
		[MessagePack.Key(3)]
		public long _positionY {get{ return positionY;} set{ this.positionY = value;}}
		[MessagePack.Key(4)]
		public bool _isBase {get{ return isBase;} set{ this.isBase = value;}}
		[MessagePack.Key(5)]
		public long _occupyingSide {get{ return occupyingSide;} set{ this.occupyingSide = value;}}
		[MessagePack.Key(6)]
		public long _index {get{ return index;} set{ this.index = value;}}
		[MessagePack.Key(7)]
		public long _hp {get{ return hp;} set{ this.hp = value;}}
		[MessagePack.Key(8)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mBattleConquestFieldId = 0; // $mBattleConquestFieldId 地形ID
		[UnityEngine.SerializeField] long positionX = 0; // $positionX 拠点が配置されているX座標。左側（味方本陣）を基準とする
		[UnityEngine.SerializeField] long positionY = 0; // $positionY 拠点が配置されているY座標。pjshinではレーン番号を指す
		[UnityEngine.SerializeField] bool isBase = false; // $isBase 本拠地かどうか
		[UnityEngine.SerializeField] long occupyingSide = 0; // $occupyingSide 初期状態でどちらの陣営の拠点になっているか。どの数字がどの状態に対応するかはクライアント側で決定。（SHINGVGでは、0 => 左側陣営 1 => 右側陣営 2 => どちらでもない）
		[UnityEngine.SerializeField] long index = 0; // $index 同一の occupyingSide に設定された拠点内での識別番号（1始まり）
		[UnityEngine.SerializeField] long hp = 0; // $hp 拠点の耐久度
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class BattleConquestFieldSpotMasterObjectBase {
		public virtual BattleConquestFieldSpotMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mBattleConquestFieldId => _rawData._mBattleConquestFieldId;
		public virtual long positionX => _rawData._positionX;
		public virtual long positionY => _rawData._positionY;
		public virtual bool isBase => _rawData._isBase;
		public virtual long occupyingSide => _rawData._occupyingSide;
		public virtual long index => _rawData._index;
		public virtual long hp => _rawData._hp;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        BattleConquestFieldSpotMasterValueObject _rawData = null;
		public BattleConquestFieldSpotMasterObjectBase( BattleConquestFieldSpotMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class BattleConquestFieldSpotMasterObject : BattleConquestFieldSpotMasterObjectBase, IMasterObject {
		public BattleConquestFieldSpotMasterObject( BattleConquestFieldSpotMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class BattleConquestFieldSpotMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Battle_Conquest_Field_Spot;

        [UnityEngine.SerializeField]
        BattleConquestFieldSpotMasterValueObject[] m_Battle_Conquest_Field_Spot = null;
    }


}
