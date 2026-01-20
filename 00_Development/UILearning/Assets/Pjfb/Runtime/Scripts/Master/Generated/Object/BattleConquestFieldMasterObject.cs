//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class BattleConquestFieldMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _sizeX {get{ return sizeX;} set{ this.sizeX = value;}}
		[MessagePack.Key(2)]
		public long _sizeY {get{ return sizeY;} set{ this.sizeY = value;}}
		[MessagePack.Key(3)]
		public long _additionalMilitaryStrengthPerSpotBroken {get{ return additionalMilitaryStrengthPerSpotBroken;} set{ this.additionalMilitaryStrengthPerSpotBroken = value;}}
		[MessagePack.Key(4)]
		public long _winRecoveryMilitaryStrength {get{ return winRecoveryMilitaryStrength;} set{ this.winRecoveryMilitaryStrength = value;}}
		[MessagePack.Key(5)]
		public string _attackBonusRate {get{ return attackBonusRate;} set{ this.attackBonusRate = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long sizeX = 0; // $sizeX 地形のX軸方向の長さ。pjshinでは味方本陣から敵本陣までの距離として定義
		[UnityEngine.SerializeField] long sizeY = 0; // $sizeY 地形のY軸方向の長さ。pjshinでは味方および敵ユニットが移動するレーンの数として定義
		[UnityEngine.SerializeField] long additionalMilitaryStrengthPerSpotBroken = 0; // $additionalMilitaryStrengthPerSpotBroken 拠点制圧された時の追加量
		[UnityEngine.SerializeField] long winRecoveryMilitaryStrength = 0; // $winRecoveryMilitaryStrength 勝利時の回復量
		[UnityEngine.SerializeField] string attackBonusRate = ""; // $attackBonusRate 攻撃ボーナス係数のjson
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class BattleConquestFieldMasterObjectBase {
		public virtual BattleConquestFieldMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long sizeX => _rawData._sizeX;
		public virtual long sizeY => _rawData._sizeY;
		public virtual long additionalMilitaryStrengthPerSpotBroken => _rawData._additionalMilitaryStrengthPerSpotBroken;
		public virtual long winRecoveryMilitaryStrength => _rawData._winRecoveryMilitaryStrength;
		public virtual string attackBonusRate => _rawData._attackBonusRate;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        BattleConquestFieldMasterValueObject _rawData = null;
		public BattleConquestFieldMasterObjectBase( BattleConquestFieldMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class BattleConquestFieldMasterObject : BattleConquestFieldMasterObjectBase, IMasterObject {
		public BattleConquestFieldMasterObject( BattleConquestFieldMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class BattleConquestFieldMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Battle_Conquest_Field;

        [UnityEngine.SerializeField]
        BattleConquestFieldMasterValueObject[] m_Battle_Conquest_Field = null;
    }


}
