using Microsoft.Extensions.Logging;
using UnityEngine;

namespace PolyQA.Query
{
    public partial class GameObjectQueryService
    {
        public int GetComponent(int gameObjectInstanceId, string componentClass)
        {
            var go = _objectRegistry.Get<GameObject>(gameObjectInstanceId);
            if (!go)
            {
                _logger.LogWarning("GameObject not found. Instance ID: {id}", gameObjectInstanceId);
                return 0;
            }

            var component = go.GetComponent(componentClass);
            return component ? _objectRegistry.Register(component) : 0;
        }

        public bool IsEnabledComponent(int componentInstanceId)
        {
            var component = _objectRegistry.Get<Behaviour>(componentInstanceId);
            if (!component)
            {
                _logger.LogWarning("Component not found. Instance ID: {id}", componentInstanceId);
                return false;
            }

            return component.enabled;
        }

        public string GetComponentValue(int componentInstanceId, string propertyName)
        {
            var component = _objectRegistry.Get<Component>(componentInstanceId);
            if (!component)
            {
                _logger.LogWarning("Component not found. Instance ID: {id}", componentInstanceId);
                return string.Empty;
            }

            var property = component.GetType().GetProperty(propertyName);
            if (property == null)
            {
                _logger.LogWarning("Property '{property}' not found on component {component}.", propertyName, component.GetType().Name);
                return string.Empty;
            }

            var value = property.GetValue(component);
            return value?.ToString() ?? string.Empty;
        }
    }
}