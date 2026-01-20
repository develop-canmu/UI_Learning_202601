using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Page;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

using CruFramework;
using CruFramework.UI;

using System;
using System.Linq;
using Pjfb.Character;
using Pjfb.Master;
using Pjfb.UserData;

using UnityEngine.UI;

namespace Pjfb.Training
{
    public class TrainingDeckCharacterView : MonoBehaviour
    {
        
        [SerializeField]
        private CharacterIcon characterIcon = null;
        
        [SerializeField]
        protected UIButton addButton = null;
        
        [SerializeField]
        private ImageCrossfade crossfade = null;
        
        private List<Sprite> spriteList = new List<Sprite>();
        
        [SerializeField]
        private Sprite impossibleSprite = null;
        [SerializeField]
        private Sprite followSprite = null;
        [SerializeField]
        private Sprite mutualFollowSprite = null;
        [SerializeField]
        private Sprite specialAttackSprite = null;
        
        [SerializeField]
        private Sprite friendSprite = null;
        
        private long mCharId = DeckUtility.EmptyDeckSlotId;
        /// <summary>キャラId</summary>
        public long MCharId{get{return mCharId;}}
        
        public CharacterType CharType
        {
            get
            {
                if(mCharId == DeckUtility.EmptyDeckSlotId)
                {
                    return CharacterType.None;
                }
                return MasterManager.Instance.charaMaster.FindData(mCharId).charaType;
            }
        }
        
        private int order = 0;
        /// <summary>場所</summary>
        public int Order{get{return order;}}
        
        private Action<int> onSelected = null;
        
        
        public void ClearLabel()
        {
            spriteList.Clear();
            crossfade.SetSpriteList(spriteList);
        }
        
        public void SetOrder(int order)
        {
            this.order = order;
        }
        
        public void SetOnSelected(Action<int> action)
        {
            onSelected = action;
        }
        
        public void SetUserCharacterId(long id)
        {
            // Empty
            if(id == DeckUtility.EmptyDeckSlotId)
            {
                SetCharacterId(id, 0, 0);
                return;
            }
            
            characterIcon.gameObject.SetActive(true);
            characterIcon.BaseCharaType = BaseCharacterType.SupportCharacter;
            UserDataChara uChar = UserDataManager.Instance.chara.data[id];
            SetCharacterId(uChar.charaId, uChar.level, uChar.newLiberationLevel);
        }
        
        public void SetCharacterId(long id, long lv, long liberationLv)
        {
            mCharId = id;
            // Empty
            if(id == DeckUtility.EmptyDeckSlotId)
            {
                characterIcon.gameObject.SetActive(false);
                addButton.gameObject.SetActive(true);
                return;
            }
            characterIcon.gameObject.SetActive(true);
            addButton.gameObject.SetActive(false);
            characterIcon.BaseCharaType = BaseCharacterType.SupportCharacter;
            characterIcon.SetIcon(id, lv, liberationLv);
        }
        
        private void SetActiveSprite(Sprite sprite, bool active)
        {
            if(active)
            {
                if(spriteList.Contains( sprite ) == false)
                {
                    spriteList.Add( sprite );
                }
            }
            else
            {
                spriteList.Remove(sprite);
            }
        }
        
        /// <summary>編成不可の表示</summary>
        public void SetImpossible(bool active)
        {
            SetActiveSprite(impossibleSprite, active);
            crossfade.SetSpriteList(spriteList);
        }
        
        /// <summary>特攻の表示</summary>
        public void SetSpecialAttack(bool active)
        {
            SetActiveSprite(specialAttackSprite, active);
            crossfade.SetSpriteList(spriteList);
        }
        
        /// <summary>フレンドの表示</summary>
        public void SetFriend(bool active)
        {
            SetActiveSprite(friendSprite, active);
            crossfade.SetSpriteList(spriteList);
        }
        
        /// <summary>フォロー表示</summary>
        public void SetFollowIcon(TrainingUtility.FriendFollowType type)
        {
            switch(type)
            {
                case TrainingUtility.FriendFollowType.None:
                {
                    spriteList.Remove( followSprite );
                    spriteList.Remove( mutualFollowSprite );
                    break;
                }
                    
                case TrainingUtility.FriendFollowType.Followed:
                {
                    if(spriteList.Contains( followSprite ) == false)
                    {
                        spriteList.Add(followSprite);
                    }
                    break;
                }
                    
                case TrainingUtility.FriendFollowType.MutualFollow:
                {
                    if(spriteList.Contains( mutualFollowSprite ) == false)
                    {
                        spriteList.Add(mutualFollowSprite);
                    }
                    break;
                }
            }         
            
            crossfade.SetSpriteList(spriteList);
        }
        
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnSelected()
        {
            onSelected?.Invoke(order);
        }
        
        public void SetDetailOrderList(SwipeableParams<CharacterDetailData> swipeableParams)
        {
            characterIcon.SwipeableParams = swipeableParams;
        }
    }
}