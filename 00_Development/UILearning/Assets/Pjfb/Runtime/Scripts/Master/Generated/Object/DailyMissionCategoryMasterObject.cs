//
// This file is auto-generated
//

namespace Pjfb.Master {

    [System.Serializable]
    [MessagePack.MessagePackObject]
    public class DailyMissionCategoryMasterValueObject : IMasterValueObject {
		[MessagePack.Key(0)]
		public long _id {get{ return id;} set{ this.id = value;}}
		[MessagePack.Key(1)]
		public string _name {get{ return name;} set{ this.name = value;}}
		[MessagePack.Key(2)]
		public long _tabType {get{ return tabType;} set{ this.tabType = value;}}
		[MessagePack.Key(3)]
		public long _categoryGroupId {get{ return categoryGroupId;} set{ this.categoryGroupId = value;}}
		[MessagePack.Key(4)]
		public long _displayJoinType {get{ return displayJoinType;} set{ this.displayJoinType = value;}}
		[MessagePack.Key(5)]
		public long _progress {get{ return progress;} set{ this.progress = value;}}
		[MessagePack.Key(6)]
		public long _contributionCoefficient {get{ return contributionCoefficient;} set{ this.contributionCoefficient = value;}}
		[MessagePack.Key(7)]
		public string _typeName {get{ return typeName;} set{ this.typeName = value;}}
		[MessagePack.Key(8)]
		public long _challengeConditionType {get{ return challengeConditionType;} set{ this.challengeConditionType = value;}}
		[MessagePack.Key(9)]
		public long _challengeConditionValue {get{ return challengeConditionValue;} set{ this.challengeConditionValue = value;}}
		[MessagePack.Key(10)]
		public string _conditionJson {get{ return conditionJson;} set{ this.conditionJson = value;}}
		[MessagePack.Key(11)]
		public string _startAt {get{ return startAt;} set{ this.startAt = value;}}
		[MessagePack.Key(12)]
		public string _endAt {get{ return endAt;} set{ this.endAt = value;}}
		[MessagePack.Key(13)]
		public string _receiveEndAt {get{ return receiveEndAt;} set{ this.receiveEndAt = value;}}
		[MessagePack.Key(14)]
		public long _stepDay {get{ return stepDay;} set{ this.stepDay = value;}}
		[MessagePack.Key(15)]
		public long _sortNumber {get{ return sortNumber;} set{ this.sortNumber = value;}}
		[MessagePack.Key(16)]
		public long _trainingSortNumber {get{ return trainingSortNumber;} set{ this.trainingSortNumber = value;}}
		[MessagePack.Key(17)]
		public string _symbolName {get{ return symbolName;} set{ this.symbolName = value;}}
		[MessagePack.Key(18)]
		public string _endDescription {get{ return endDescription;} set{ this.endDescription = value;}}
		[MessagePack.Key(19)]
		public string _receiveEndDescription {get{ return receiveEndDescription;} set{ this.receiveEndDescription = value;}}
		[MessagePack.Key(20)]
		public long _displayType {get{ return displayType;} set{ this.displayType = value;}}
		[MessagePack.Key(21)]
		public long[] _tagListForReceive {get{ return tagListForReceive;} set{ this.tagListForReceive = value;}}
		[MessagePack.Key(22)]
		public bool _deleteFlg {get{ return deleteFlg;} set{ this.deleteFlg = value;}}


		[UnityEngine.SerializeField] long id = 0; // ID
		[UnityEngine.SerializeField] string name = ""; // 名称
		[UnityEngine.SerializeField] long tabType = 0; // ネイティブタイトル等にて、対象のカテゴリのミッションをどのタブに配置するかを決定する番号（0 => タブに出さない、 1 => デイリー、 2 => 通算、 3 => イベント、 4 => ビンゴ）
		[UnityEngine.SerializeField] long categoryGroupId = 0; // ミッションカテゴリのグループ。ミッションイベントの開催単位ごとに同一の値を設定する。現状は表示をまとめることに用途を限定しており、この値をもとにミッション遂行機能の制御は行わない
		[UnityEngine.SerializeField] long displayJoinType = 0; // 参加タイプ。1 => 個人での参加、2 => ギルドでの参加。現状は表示をまとめることに用途を限定しており、この値をもとにミッション遂行機能の制御は行わない（実際の制御は m_daily_mission.joinType で行う）
		[UnityEngine.SerializeField] long progress = 0; // 同一カテゴリグループ内でいくつめのミッションカテゴリか。現状は表示をまとめることに用途を限定しており、この値をもとにミッション遂行機能の制御は行わない
		[UnityEngine.SerializeField] long contributionCoefficient = 0; // ギルド参加ミッション限定で使用する貢献度ぼかし係数。各ユーザーの貢献度として表示する値は、ミッションに対する寄与度（%）*ぼかし係数 で計算される
		[UnityEngine.SerializeField] string typeName = ""; // 種別名。このミッションカテゴリがどのような種別（「デイリー」「通算」など）に分類されているかクライアント側で表示するためのカラム
		[UnityEngine.SerializeField] long challengeConditionType = 0; // 挑戦条件種別0: 誰でも挑戦可能（挑戦条件なし）101: 指定のタグを所持しているユーザのみ挑戦可能
		[UnityEngine.SerializeField] long challengeConditionValue = 0; // 挑戦条件値。challengeConditionType に応じて入力すべき値が変わる。specifyType = 0 の場合、0 を指定。specifyType = 101 の場合、任意の adminTagId を指定。
		[UnityEngine.SerializeField] string conditionJson = ""; // 表示条件
		[UnityEngine.SerializeField] string startAt = ""; // 開始日時
		[UnityEngine.SerializeField] string endAt = ""; // 終了日時
		[UnityEngine.SerializeField] string receiveEndAt = ""; // 受取期限日時
		[UnityEngine.SerializeField] long stepDay = 0; // 1ミッションの周期日数。0の場合1度限りのミッション。1の場合デイリーミッション。7の場合ウィークリー等
		[UnityEngine.SerializeField] long sortNumber = 0; // ソート順（基本、昇順）
		[UnityEngine.SerializeField] long trainingSortNumber = 0; // トレーニング画面でのソート順（基本、昇順）
		[UnityEngine.SerializeField] string symbolName = ""; // 画像やスタイルを取得する際に使うキー文字列
		[UnityEngine.SerializeField] string endDescription = ""; // 終了日時説明文（ここに空文字列以外の値がある場合、この値を「endAt」から計算する文字列の代わりに表示する）
		[UnityEngine.SerializeField] string receiveEndDescription = ""; // 受取期限説明文（ここに空文字列以外の値がある場合、この値を「receiveEndAt」から計算する文字列の代わりに表示する）
		[UnityEngine.SerializeField] long displayType = 0; // 表示種別。1: 常に表示、2: 未受取のミッションがあれば表示
		[UnityEngine.SerializeField] long[] tagListForReceive = null; // 達成タグOR条件。該当タグが未所持の場合もミッションは更新されるが受け取りはできない。例：[1,2]
		[UnityEngine.SerializeField] bool deleteFlg = false; // 論理削除状態 1 => 未削除, 2 => 削除

    }

    public class DailyMissionCategoryMasterObjectBase {
		public virtual DailyMissionCategoryMasterValueObject rawData => _rawData;

		public virtual long id => _rawData._id;
		public virtual string name => _rawData._name;
		public virtual long tabType => _rawData._tabType;
		public virtual long categoryGroupId => _rawData._categoryGroupId;
		public virtual long displayJoinType => _rawData._displayJoinType;
		public virtual long progress => _rawData._progress;
		public virtual long contributionCoefficient => _rawData._contributionCoefficient;
		public virtual string typeName => _rawData._typeName;
		public virtual long challengeConditionType => _rawData._challengeConditionType;
		public virtual long challengeConditionValue => _rawData._challengeConditionValue;
		public virtual string conditionJson => _rawData._conditionJson;
		public virtual string startAt => _rawData._startAt;
		public virtual string endAt => _rawData._endAt;
		public virtual string receiveEndAt => _rawData._receiveEndAt;
		public virtual long stepDay => _rawData._stepDay;
		public virtual long sortNumber => _rawData._sortNumber;
		public virtual long trainingSortNumber => _rawData._trainingSortNumber;
		public virtual string symbolName => _rawData._symbolName;
		public virtual string endDescription => _rawData._endDescription;
		public virtual string receiveEndDescription => _rawData._receiveEndDescription;
		public virtual long displayType => _rawData._displayType;
		public virtual long[] tagListForReceive => _rawData._tagListForReceive;
		public virtual bool deleteFlg => _rawData._deleteFlg;

        DailyMissionCategoryMasterValueObject _rawData = null;
		public DailyMissionCategoryMasterObjectBase( DailyMissionCategoryMasterValueObject rawData ){
			_rawData = rawData;
		}
	}


	public partial class DailyMissionCategoryMasterObject : DailyMissionCategoryMasterObjectBase, IMasterObject {
		public DailyMissionCategoryMasterObject( DailyMissionCategoryMasterValueObject rawData ) : base(rawData){

		}
	}

	[System.Serializable]
    public class DailyMissionCategoryMasterDeserializeObject : IMasterDeserializeObject
    {
        public System.Collections.IList values => m_Daily_Mission_Category;

        [UnityEngine.SerializeField]
        DailyMissionCategoryMasterValueObject[] m_Daily_Mission_Category = null;
    }


}
