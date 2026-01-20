using System.Collections.Generic;
using PolyQA.Extensions;

namespace PolyQA.Executor
{
    public class RoundRobinUpdater
    {
        private int _nextIndex;
        private List<IUpdater> _updaters = new();

        public void Add(IUpdater updater)
        {
            _updaters.AddUnique(updater);
        }

        public void Remove(IUpdater updater)
        {
            var index = _updaters.IndexOf(updater);
            if (index < 0) return;

            _updaters.RemoveAt(index);
            if (index < _nextIndex)
            {
                _nextIndex--;
            }
        }

        public void Update()
        {
            if (_updaters.Count == 0) return;

            _nextIndex %= _updaters.Count;
            _updaters[_nextIndex].Update();

            _nextIndex = (_nextIndex + 1) % _updaters.Count;
        }
    }
}