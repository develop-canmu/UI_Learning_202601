//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class BattleWarFieldMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _sizeX {get{ return sizeX;} set{ this.sizeX = value;}}
		[MessagePack.Key(2)]
		public long _sizeY {get{ return sizeY;} set{ this.sizeY = value;}}
		[MessagePack.Key(3)]
		public long _marginY {get{ return marginY;} set{ this.marginY = value;}}
		[MessagePack.Key(4)]
		public long _allyRegularMilitaryStrength {get{ return allyRegularMilitaryStrength;} set{ this.allyRegularMilitaryStrength = value;}}
		[MessagePack.Key(5)]
		public long _enemyRegularMilitaryStrength {get{ return enemyRegularMilitaryStrength;} set{ this.enemyRegularMilitaryStrength = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long sizeX = 0; // $sizeX 地形のX軸方向の長さ。pjshinでは味方本陣から敵本陣までの距離として定義
		[UnityEngine.SerializeField] long sizeY = 0; // $sizeY 地形のY軸方向の長さ。pjshinでは味方および敵ユニットが移動するレーンの数として定義
		[UnityEngine.SerializeField] long marginY = 0; // $marginY 地形のY軸方向1単位間の距離。pjshinではレーン間を移動する際の距離として定義
		[UnityEngine.SerializeField] long allyRegularMilitaryStrength = 0; // $allyRegularMilitaryStrength この地形に紐付けられた味方側の規定兵力値。pjshinではこの規定兵力値が各ユニットに割り振られる
		[UnityEngine.SerializeField] long enemyRegularMilitaryStrength = 0; // $enemyRegularMilitaryStrength この地形に紐付けられた敵側の規定兵力値。pjshinではこの規定兵力値が各ユニットに割り振られる
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class BattleWarFieldMasterObjectBase {
		public virtual BattleWarFieldMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long sizeX => _rawData._sizeX;
		public virtual long sizeY => _rawData._sizeY;
		public virtual long marginY => _rawData._marginY;
		public virtual long allyRegularMilitaryStrength => _rawData._allyRegularMilitaryStrength;
		public virtual long enemyRegularMilitaryStrength => _rawData._enemyRegularMilitaryStrength;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        BattleWarFieldMasterValueObject _rawData = null;
		public BattleWarFieldMasterObjectBase( BattleWarFieldMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class BattleWarFieldMasterObject : BattleWarFieldMasterObjectBase, IMasterObject {
		public BattleWarFieldMasterObject( BattleWarFieldMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class BattleWarFieldMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Battle_War_Field;

        [UnityEngine.SerializeField]
        BattleWarFieldMasterValueObject[] m_Battle_War_Field = null;
    }


}
