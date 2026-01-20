using UnityEngine;

namespace PolyQA.Extensions
{
    public static class GameObjectExtensions
    {
        public static T AddUniqueComponent<T>(this GameObject go) where T : Component
        {
            var comp = go.GetComponent<T>();
            return comp ? comp : go.AddComponent<T>();
        }
    }
}