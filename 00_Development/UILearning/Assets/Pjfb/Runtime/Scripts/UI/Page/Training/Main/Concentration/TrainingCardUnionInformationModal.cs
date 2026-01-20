using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pjfb.Training
{
    /// <summary> カードユニオン情報モーダル </summary>
    public class TrainingCardUnionInformationModal : ModalWindow
    {
        public class Arguments
        {
            private TrainingMainArguments mainArguments = null;
            /// <summary>TrainingMainArguments</summary>
            public TrainingMainArguments MainArguments{get{return mainArguments;}}
            
            public Arguments(TrainingMainArguments mainArguments)
            { 
                this.mainArguments = mainArguments;
            }
        }
        
        // インスピレーション一覧表示用View
        [SerializeField]
        private TrainingGetInspirationListView inspirationListView = null;
        
        // インスピレーションが付与された練習カード一覧表示用View
        [SerializeField]
        private TrainingGetInspirationCardListView practiceCardListView = null;
        
        // カードユニオン一覧表示用View
        [SerializeField]
        private TrainingCardUnionInformationListView cardUnionListView = null;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            Arguments arguments = (Arguments)args;
            
            // 対象のリストを初期化して表示
            // インスピレーションリストを初期化して表示
            inspirationListView.Initialize(arguments.MainArguments.Pending);
            // インスピレーションが付与された練習カードリストを初期化して表示
            practiceCardListView.Initialize(arguments.MainArguments, arguments.MainArguments.SelectedTrainingCardIndex);
            // カードユニオンリストを初期化して表示
            cardUnionListView.Initialize(arguments.MainArguments);
            
            return base.OnPreOpen(args, token);
        }
    }
}

