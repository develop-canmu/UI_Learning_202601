using System;
using System.Linq;
using Pjfb.Voice;

namespace Pjfb.Master {

    public partial class CharaLibraryVoiceMasterContainer : MasterContainerBase<CharaLibraryVoiceMasterObject> {
        long GetDefaultKey(CharaLibraryVoiceMasterObject masterObject){
            return masterObject.id;
        }
        
        /// <summary>
        /// セリフ掛け合いの両方を取得
        /// 1番目から再生想定
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public CharaLibraryVoiceMasterObject[] GetPairData(CharaVoiceLocationType firstLocationType, CharaMasterObject firstChara, long firstAbilityId, CharaVoiceLocationType secondLocationType = CharaVoiceLocationType.None, CharaMasterObject secondChara = null, long secondAbilityId = -1)
        {
            if (firstChara == null)
            {
                return null;
            }

            CharaLibraryVoiceMasterObject firstData = null;
            CharaLibraryVoiceMasterObject secondData = null;
            // １人目のデータ検索
            var firstResult = values.Where(value =>
                  // 種別マッチ
                  value.LocationType == firstLocationType 
                )
                .Where(value =>
                    // ボイスタイプが3,4の場合CharaIDでマッチ
                    ((value.voiceType == 3　|| value.voiceType == 4) &&
                    // CharaIDマッチ
                    (value.masterType == 1 && value.masterId == firstChara.parentMCharaId ||
                     value.masterType == 2 && value.masterId == firstChara.id))
                ).ToList();
            if (firstResult.Count <= 0)
            {
                return null;
            }
            
            // 優先度大きいものからランダムで
            var firstPriority = firstResult.Max(r => r.priority);
            firstData = firstResult.Where(value => value.priority == firstPriority).OrderBy(_ => Guid.NewGuid()).FirstOrDefault();

            // 相手指定の特別な掛け合いがある場合は２人目もそれを使用
            if (secondChara != null && firstData.groupNumber > 0 && (firstData.opponentMasterType == 1 && firstData.opponentMasterId == secondChara.parentMCharaId ||
                                                                     firstData.opponentMasterType == 2 && firstData.opponentMasterId == secondChara.id))
            {
                secondData = values.FirstOrDefault(value =>
                    // IDマッチ
                    (value.masterType == 1 && value.masterId == secondChara.parentMCharaId ||
                     value.masterType == 2 && value.masterId == secondChara.id) &&
                    (value.opponentMasterType == 1 && value.opponentMasterId == firstChara.parentMCharaId ||
                     value.opponentMasterType == 2 && value.opponentMasterId == firstChara.id) &&
                    // グループマッチ
                    value.groupNumber == firstData.groupNumber &&
                    // 種別マッチ
                    value.LocationType == secondLocationType);
            }

            if(secondChara != null && secondData == null)
            {
                // 無い場合は1人目も通常のに変更
                firstData = firstResult.Where(value => value.priority == firstPriority && value.groupNumber == 0).OrderBy(_ => Guid.NewGuid()).FirstOrDefault();
                // ２人目も通常のから検索
                var secondResult = values.Where(value =>
                       // 通常指定
                       value.opponentMasterType == 0 &&
                       // 種別マッチ
                       value.LocationType == secondLocationType
                    ).Where(value =>
                        // ボイスタイプが3,4の場合CharaIDでマッチ
                        ((value.voiceType == 3　|| value.voiceType == 4) &&
                        // CharaIDマッチ
                        (value.masterType == 1 && value.masterId == secondChara.parentMCharaId ||
                        value.masterType == 2 && value.masterId == secondChara.id))
                    ).ToList();
                if (secondResult.Count > 0)
                {
                    // 優先度大きいものからランダムで
                    var secondPriority = secondResult.Max(r => r.priority);
                    secondData = secondResult.Where(value => value.priority == secondPriority).OrderBy(_ => Guid.NewGuid()).FirstOrDefault();
                }
            }

            CharaLibraryVoiceMasterObject[] pairData = { firstData, secondData };
            return pairData;
        }

        /// <summary>
        /// 複数ある場合はそのうちのランダム.
        /// </summary>
        /// <param name="locationType"></param>
        /// <returns></returns>
        public CharaLibraryVoiceMasterObject GetDataByLocationType(CharaVoiceLocationType locationType)
        {
            return values.Where(value => value.LocationType == locationType).OrderBy(_ => Guid.NewGuid()).FirstOrDefault();
        }
        
        public CharaLibraryVoiceMasterObject[] GetDataArrayByLocationType(CharaVoiceLocationType locationType, long parentMCharaId)
        {
            return values.Where(value => value.LocationType == locationType && value.masterId == parentMCharaId).OrderBy(_ => Guid.NewGuid()).ToArray();
        }


        /// <summary>
        /// 必殺技ボイスの取得
        /// </summary>
        /// <param name="abilityId"></param>
        /// <returns></returns>
        public CharaLibraryVoiceMasterObject GetDataByAbilityId(long abilityId)
        {
            return values.FirstOrDefault(value => value.voiceType == 6 && value.useType == abilityId);
        }
    }
}
