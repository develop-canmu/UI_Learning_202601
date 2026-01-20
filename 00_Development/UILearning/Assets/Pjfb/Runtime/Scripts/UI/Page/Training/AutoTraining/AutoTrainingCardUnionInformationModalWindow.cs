using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Training;
using UnityEngine;

namespace Pjfb
{
    public class AutoTrainingCardUnionInformationModalWindow : ModalWindow
    {
        public class ModalData
        {
           private TrainingMainArguments mainArguments = null;
           public TrainingMainArguments MainArguments => mainArguments;
           
            public ModalData(TrainingMainArguments trainingMainArguments)
            {
                this.mainArguments = trainingMainArguments;
            }
        }
        
        [SerializeField]
        private AutoTrainingCardUnionInformationListView cardUnionInformationListView = null;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            ModalData modalData = (ModalData)args;
            cardUnionInformationListView.Initialize(modalData.MainArguments);
            
            return base.OnPreOpen(args, token);
        }
    }
}