//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class BattleReserveFormationRoundMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public long _mBattleReserveFormationRoundGroupId {get{ return mBattleReserveFormationRoundGroupId;} set{ this.mBattleReserveFormationRoundGroupId = value;}}
		[MessagePack.Key(2)]
		public long _roundNumber {get{ return roundNumber;} set{ this.roundNumber = value;}}
		[MessagePack.Key(3)]
		public string _nameLabel {get{ return nameLabel;} set{ this.nameLabel = value;}}
		[MessagePack.Key(4)]
		public long _importanceLabelNumber {get{ return importanceLabelNumber;} set{ this.importanceLabelNumber = value;}}
		[MessagePack.Key(5)]
		public long _winningPoint {get{ return winningPoint;} set{ this.winningPoint = value;}}
		[MessagePack.Key(6)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] long mBattleReserveFormationRoundGroupId = 0; // $mBattleReserveFormationRoundGroupId グループ化ID（実テーブルはなく、roundをグループ化するための概念）
		[UnityEngine.SerializeField] long roundNumber = 0; // $roundNumber 試合番号（1からの連番で設定）
		[UnityEngine.SerializeField] string nameLabel = ""; // $nameLabel 名称ラベル
		[UnityEngine.SerializeField] long importanceLabelNumber = 0; // $importanceLabelNumber 重要度指標（サーバーサイドでは見ないので、クライアントで表示の出し分けに使ってもらう）
		[UnityEngine.SerializeField] long winningPoint = 0; // $winningPoint 回戦勝利時の獲得勝ち点
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class BattleReserveFormationRoundMasterObjectBase {
		public virtual BattleReserveFormationRoundMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual long mBattleReserveFormationRoundGroupId => _rawData._mBattleReserveFormationRoundGroupId;
		public virtual long roundNumber => _rawData._roundNumber;
		public virtual string nameLabel => _rawData._nameLabel;
		public virtual long importanceLabelNumber => _rawData._importanceLabelNumber;
		public virtual long winningPoint => _rawData._winningPoint;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        BattleReserveFormationRoundMasterValueObject _rawData = null;
		public BattleReserveFormationRoundMasterObjectBase( BattleReserveFormationRoundMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class BattleReserveFormationRoundMasterObject : BattleReserveFormationRoundMasterObjectBase, IMasterObject {
		public BattleReserveFormationRoundMasterObject( BattleReserveFormationRoundMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class BattleReserveFormationRoundMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Battle_Reserve_Formation_Round;

        [UnityEngine.SerializeField]
        BattleReserveFormationRoundMasterValueObject[] m_Battle_Reserve_Formation_Round = null;
    }


}
