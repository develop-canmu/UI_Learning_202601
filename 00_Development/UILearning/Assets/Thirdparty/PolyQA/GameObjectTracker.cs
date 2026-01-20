using UnityEngine;

namespace PolyQA
{
    public interface IGameObjectTracker
    {
        public string Label { get; }
    }

    public class GameObjectTracker : MonoBehaviour, IGameObjectTracker
    {
        [SerializeField]
        private string _label;

        public string Label
        {
            get => _label;
            set => _label = value;
        }
    }
}
