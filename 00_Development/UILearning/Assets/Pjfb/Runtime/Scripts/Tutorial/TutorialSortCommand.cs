using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.UserData;

namespace Pjfb
{
    public class TutorialSortCommand : TutorialCommandSetting.TutorialCommandData
    {
        [SerializeField] private string focusObjectId;
        public string FocusObjectId => focusObjectId; 
        // ソート時先頭に配置したいキャラのデータを入力
        [SerializeField] private long mCharaId;
        public long MCharaId => mCharaId;

        public override async UniTask ActionCommand(CancellationToken token)
        {
            TutorialCommandTarget targetObject = await FindTarget(focusObjectId,token);
            UserCharacterScroll characterScroll = targetObject.GetComponent<UserCharacterScroll>();
            await UpdateCharaList(characterScroll); 
        }
        
        // キャラクターリストをソート
        private async UniTask UpdateCharaList(UserCharacterScroll characterScroll)
        {
            // charaIDをもとに並び替え
            List<UserDataChara> uCharaList = UserDataManager.Instance.GetUserDataCharaListByType(CardType.Character).ToList();

            foreach (var uCharaData in uCharaList)
            {
                if(uCharaData.charaId != mCharaId) continue;
                uCharaList.Remove(uCharaData);
                uCharaList.Insert(0,uCharaData);
                break;
            }

            // リストを反映
            characterScroll.SetSortCharaList(uCharaList);
            // リスト反映まで1フレーム待つ
            await UniTask.NextFrame();
        }
    }
}