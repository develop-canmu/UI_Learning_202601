using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


namespace CruFramework.Editor.Adv
{
    [System.Serializable]
    public class AdvEditorUndoRedoData : ScriptableObject
    {
        
        [Serializable]
        public class UndoNodePosition
        {
            [SerializeField]
            public ulong nodeId = 0;
            [SerializeField]
            public Vector2 position = Vector2.zero;
        }
    
        [Serializable]
        public class UndoNodeDelete
        {
            [SerializeReference]
            public AdvCommandNode node = null;
            [SerializeField]
            public List<ulong> connectionNodes = new List<ulong>();
        }
    
        [Serializable]
        public class UndoNodeCreate
        {
            [SerializeReference]
            public AdvCommandNode node = null;
            [SerializeField]
            public Vector2 position = Vector2.zero;
        }
    
        [Serializable]
        public class UndoEdgeDelete
        {
            [SerializeField]
            public ulong inputNodeId = 0;
            [SerializeField]
            public ulong outputNodeId = 0;
        }
    
        [Serializable]
        public class UndoEdgeCreate
        {
            [SerializeField]
            public ulong inputNodeId = 0;
            [SerializeField]
            public ulong outputNodeId = 0;
        }
        
        [SerializeField]
        public int id = 0;
        [SerializeField]
        public Vector2 moveDelta = Vector2.zero;
        [SerializeField]
        public List<UndoNodePosition> nodePositions = new List<UndoNodePosition>();
        [SerializeField]
        public List<UndoNodeCreate> nodeCreates = new List<UndoNodeCreate>();
        [SerializeField]
        public List<UndoNodeDelete> nodeDeletes = new List<UndoNodeDelete>();
        [SerializeField]
        public List<UndoEdgeCreate> edgeCreates = new List<UndoEdgeCreate>();
        [SerializeField]
        public List<UndoEdgeDelete> edgeDeletes = new List<UndoEdgeDelete>();
        [SerializeField]
        public List<UndoEdgeDelete> edgeSystemDeletes = new List<UndoEdgeDelete>();

    }
}