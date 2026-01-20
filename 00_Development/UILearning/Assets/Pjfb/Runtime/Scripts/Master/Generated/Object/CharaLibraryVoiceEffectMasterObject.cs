//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class CharaLibraryVoiceEffectMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _effectPatternJson {get{ return effectPatternJson;} set{ this.effectPatternJson = value;}}
		[MessagePack.Key(2)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // $id
		[UnityEngine.SerializeField] string effectPatternJson = ""; // $effectPatternJson セリフ表示UIの演出パターン。顔アイコンの表示種別、吹き出しデザイン、グレースケール有無、エフェクト設定等をjson形式で指定。クライアントでのみ参照する
		[UnityEngine.SerializeField] bool deleteFlg = false; // $deleteFlg 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class CharaLibraryVoiceEffectMasterObjectBase {
		public virtual CharaLibraryVoiceEffectMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string effectPatternJson => _rawData._effectPatternJson;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        CharaLibraryVoiceEffectMasterValueObject _rawData = null;
		public CharaLibraryVoiceEffectMasterObjectBase( CharaLibraryVoiceEffectMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class CharaLibraryVoiceEffectMasterObject : CharaLibraryVoiceEffectMasterObjectBase, IMasterObject {
		public CharaLibraryVoiceEffectMasterObject( CharaLibraryVoiceEffectMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class CharaLibraryVoiceEffectMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Chara_Library_Voice_Effect;

        [UnityEngine.SerializeField]
        CharaLibraryVoiceEffectMasterValueObject[] m_Chara_Library_Voice_Effect = null;
    }


}
