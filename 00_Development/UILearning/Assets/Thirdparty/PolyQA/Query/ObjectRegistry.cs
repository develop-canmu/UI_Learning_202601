using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Object = UnityEngine.Object;

namespace PolyQA.Query
{
    public class ObjectRegistry
    {
        private readonly ILogger _logger = Logging.CreateLogger<ObjectRegistry>();
        private readonly Dictionary<int, ObjectReference> _objectReferences = new();

        public T Get<T>(int instanceId) where T : Object
        {
            if (_objectReferences.TryGetValue(instanceId, out var reference))
            {
                return reference.Instance as T;
            }
            return null;
        }

        public int Register(Object obj)
        {
            if (!obj)
                throw new ArgumentException("Object has been destroyed", nameof(obj));

            var instanceId = obj.GetInstanceID();
            if (_objectReferences.TryGetValue(instanceId, out var reference))
            {
                reference.ReferenceCount++;
                return instanceId;
            }

            var newReference = new ObjectReference(obj);
            _objectReferences[instanceId] = newReference;
            return instanceId;
        }

        public void Unregister(int instanceId)
        {
            if (!_objectReferences.TryGetValue(instanceId, out var reference))
            {
                _logger.LogWarning("Object with instance ID {InstanceId} is not registered.", instanceId);
                return;
            }

            reference.ReferenceCount--;
            if (reference.ReferenceCount <= 0)
            {
                _objectReferences.Remove(instanceId);
                _logger.LogDebug("Object with instance ID {InstanceId} has been released.", instanceId);
            }
        }

        private class ObjectReference
        {
            public Object Instance { get; }
            public int ReferenceCount { get; set; }

            public ObjectReference(Object instance)
            {
                Instance = instance;
                ReferenceCount = 1;
            }
        }
    }
}