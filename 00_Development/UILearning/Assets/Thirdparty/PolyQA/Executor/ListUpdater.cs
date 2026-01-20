using System.Collections.Generic;
using PolyQA.Extensions;

namespace PolyQA.Executor
{
    public class ListUpdater
    {
        private List<IUpdater> _updaters = new();

        public void Add(IUpdater updater)
        {
            _updaters.AddUnique(updater);
        }

        public void Remove(IUpdater updater)
        {
            _updaters.Remove(updater);
        }

        public void Update()
        {
            for (var i = 0; i < _updaters.Count; i++)
            {
                _updaters[i].Update();
            }
        }
    }
}