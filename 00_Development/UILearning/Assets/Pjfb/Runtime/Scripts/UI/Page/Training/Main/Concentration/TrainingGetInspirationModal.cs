using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pjfb.Training
{
    /// <summary> インスピレーション獲得モーダル </summary>
    public class TrainingGetInspirationModal : ModalWindow
    {
        public class Arguments
        {
            private TrainingMainArguments mainArguments = null;
            /// <summary>TrainingMainArguments</summary>
            public TrainingMainArguments MainArguments{get{return mainArguments;}}
            
            private int selectCardIndex = 0;
            /// <summary>選択しているカード番号</summary>
            public int SelectCardIndex{get{return selectCardIndex;}}
            
            public Arguments(TrainingMainArguments mainArguments, int selectCardIndex)
            { 
                this.mainArguments = mainArguments;
                this.selectCardIndex = selectCardIndex;
            }
        }
        
        // インスピレーション一覧表示用View
        [SerializeField]
        private TrainingGetInspirationListView inspirationListView = null;
        
        // インスピレーションが付与された練習カード一覧表示用View
        [SerializeField]
        private TrainingGetInspirationCardListView practiceCardListView = null;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            Arguments arguments = (Arguments)args;
            
            // 対象のリストを初期化して表示
            // インスピレーションリストを初期化して表示
            inspirationListView.Initialize(arguments.MainArguments.Pending);
            // インスピレーションが付与された練習カードリストを初期化して表示
            practiceCardListView.Initialize(arguments.MainArguments, arguments.SelectCardIndex);
            
            return base.OnPreOpen(args, token);
        }
    }
}