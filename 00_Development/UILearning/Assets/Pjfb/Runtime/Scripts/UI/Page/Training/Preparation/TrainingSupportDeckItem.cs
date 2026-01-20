using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Page;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

using CruFramework;
using CruFramework.UI;
using Pjfb.Networking.App.Request;

namespace Pjfb.Training
{
    public class TrainingSupportDeckItem<T> : ScrollGridItem where T : TrainingSupportDeckView
    {
        public enum EventType
        {
            Reset, Recommend, SelectCharacter, Equipment
        }
        
        public struct EventData
        {
            public EventType Type;
            public object Value;
            
            public EventData(EventType type, object value)
            {
                Type = type;
                Value = value;
            }
        }
        
        [SerializeField]
        private T deckView = null;
        /// <summary>View</summary>
        public T DeckView{get{return deckView;}}
        
        private void Awake()
        {
            deckView.OnSelected += OnSelected;
            deckView.OnRecommend += OnRecommend;
            deckView.OnReset += OnReset;
        }

        private void OnReset(TrainingSupportDeckView view)
        {
            TriggerEvent( new EventData(EventType.Reset, view));
        }
        
        private void OnRecommend(TrainingSupportDeckView view)
        {
            TriggerEvent( new EventData(EventType.Recommend, view));
        }
        
        private void OnSelected(TrainingSupportDeckView.SelectData selectData)
        {
            TriggerEvent( new EventData( selectData.Type != TrainingDeckMemberType.Equipment ? EventType.SelectCharacter : EventType.Equipment, selectData));
        }

        protected override void OnSetView(object value)
        {
            TrainingPreparationArgs args = (TrainingPreparationArgs)ParentScrollGrid.CommonItemValue;
            deckView.SetView(args, (DeckData)value );
        }
    }
}