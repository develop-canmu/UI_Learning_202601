//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class DeckExtraMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _useType {get{ return useType;} set{ this.useType = value;}}
		[MessagePack.Key(3)]
		public long _partyNumberMin {get{ return partyNumberMin;} set{ this.partyNumberMin = value;}}
		[MessagePack.Key(4)]
		public long _partyNumberMax {get{ return partyNumberMax;} set{ this.partyNumberMax = value;}}
		[MessagePack.Key(5)]
		public long _mDeckFormatIdInGroup {get{ return mDeckFormatIdInGroup;} set{ this.mDeckFormatIdInGroup = value;}}
		[MessagePack.Key(6)]
		public long _ruleType {get{ return ruleType;} set{ this.ruleType = value;}}
		[MessagePack.Key(7)]
		public bool _isRequired {get{ return isRequired;} set{ this.isRequired = value;}}
		[MessagePack.Key(8)]
		public NativeApiPeriodTime[] _lockTimeJson {get{ return lockTimeJson;} set{ this.lockTimeJson = value;}}
		[MessagePack.Key(9)]
		public long[] _mDeckExtraIdListLinked {get{ return mDeckExtraIdListLinked;} set{ this.mDeckExtraIdListLinked = value;}}
		[MessagePack.Key(10)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string name = ""; // $name 名称
		[UnityEngine.SerializeField] long useType = 0; // $useType 特殊用途番号。現在1001, 1101, 1201, 1301が存在。重複しないように設定する。
		[UnityEngine.SerializeField] long partyNumberMin = 0; // $partyNumberMin パーティ番号最小（1001～3999までの範囲で指定可能。別なuseTypeと、重複した範囲の指定は避ける）
		[UnityEngine.SerializeField] long partyNumberMax = 0; // $partyNumberMax パーティ番号最大（1001～3999までの範囲で指定可能。別なuseTypeと、重複した範囲の指定は避ける）
		[UnityEngine.SerializeField] long mDeckFormatIdInGroup = 0; // $mDeckFormatIdInGroup デッキグループ内での制限ルール指定（全デッキをまたいで、キャラを使い回さない～等の指定、親キャラIDの使いまわし数を制限したり～等。slotに対するバリデーションは現状はなし）
		[UnityEngine.SerializeField] long ruleType = 0; // $ruleType 特殊デッキごとに存在する、特殊なルール運用（1 => 特に無し、 2 => 疲労度付き。）
		[UnityEngine.SerializeField] bool isRequired = false; // $isRequired この特殊デッキ用途において、1つ以上のデッキが作成されていることを必須とするか。必須であればユーザー作成時やログイン時に既存のデッキデータなどから自動作成されるが、必須でない場合は自動作成されない
		[UnityEngine.SerializeField] NativeApiPeriodTime[] lockTimeJson = null; // 特定の時間内は編成させないという設定。["0:00", "2:00"]の場合は0時から2時、["23:00", "1:00"]の場合は23時から24時及び0時から1時が編成不可能になる。指定可能な文字列は0:00～24:00まで。（25:00等の指定には対応しない。）2要素単位でペアで判定される。奇数の場合、端数部分は判定されない。nullの場合は0要素とみなす。(hh:mm:ss形式。時・分・秒まで記載。時は2時の場合は02と記載)
		[UnityEngine.SerializeField] long[] mDeckExtraIdListLinked = null; // 関連特殊デッキIDリスト。m_deck_format_condition 内で m_deck_extra 単位で指定した制約を複数の m_deck_extra 間で共有する際に指定する
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class DeckExtraMasterObjectBase {
		public virtual DeckExtraMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long useType => _rawData._useType;
		public virtual long partyNumberMin => _rawData._partyNumberMin;
		public virtual long partyNumberMax => _rawData._partyNumberMax;
		public virtual long mDeckFormatIdInGroup => _rawData._mDeckFormatIdInGroup;
		public virtual long ruleType => _rawData._ruleType;
		public virtual bool isRequired => _rawData._isRequired;
		public virtual NativeApiPeriodTime[] lockTimeJson => _rawData._lockTimeJson;
		public virtual long[] mDeckExtraIdListLinked => _rawData._mDeckExtraIdListLinked;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        DeckExtraMasterValueObject _rawData = null;
		public DeckExtraMasterObjectBase( DeckExtraMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class DeckExtraMasterObject : DeckExtraMasterObjectBase, IMasterObject {
		public DeckExtraMasterObject( DeckExtraMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class DeckExtraMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Deck_Extra;

        [UnityEngine.SerializeField]
        DeckExtraMasterValueObject[] m_Deck_Extra = null;
    }


}
