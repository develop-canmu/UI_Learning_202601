using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;

namespace Pjfb
{
    public class TrainingFriendFollowModal : ModalWindow
    {
    
        public class Arguments
        {
            private TrainingFriend friend = null;
            /// <summary>フレンド</summary>
            public TrainingFriend Friend{get{return friend;}}
            
            private long characterId = 0;
            /// <summary>Lv</summary>
            public long CharacterId{get{return characterId;}}
            
            private long characterLv = 0;
            /// <summary>Lv</summary>
            public long CharacterLv{get{return characterLv;}}
            
            private long liberationLv = 0;
            /// <summary>liberationLv</summary>
            public long LiberationLv{get{return liberationLv;}}
            
            public Arguments(TrainingFriend friend, long characterId, long characterLv, long liberationLv)
            {
                this.friend = friend;
                this.characterId = characterId;
                this.characterLv = characterLv;
                this.liberationLv = liberationLv;
            }
        }
    
        [SerializeField]
        private TrainingFriendFollowView followView = null;


        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            Arguments arguments = (Arguments)args; 
            
            followView.SetView(arguments.Friend);
            followView.SetCharacter(arguments.CharacterId, arguments.CharacterLv, arguments.LiberationLv);
            
            return base.OnPreOpen(args, token);
        }
        
        /// <summary>
        ///UGUI
        /// </summary>
        public void OnFollowButton()
        {
            OnFollowButtonAsync().Forget();
        }
        
        private async UniTask OnFollowButtonAsync()
        {
            Arguments arguments = (Arguments)ModalArguments; 
            
            // Request
            CommunityFollowAPIRequest request = new CommunityFollowAPIRequest();
            // Post
            CommunityFollowAPIPost post = new CommunityFollowAPIPost();
            post.targetUMasterId = arguments.Friend.communityUserStatus.uMasterId;
            request.SetPostData(post);
            // API
            await APIManager.Instance.Connect(request);
            
            // 閉じる
            await CloseAsync();
        }
    }
}