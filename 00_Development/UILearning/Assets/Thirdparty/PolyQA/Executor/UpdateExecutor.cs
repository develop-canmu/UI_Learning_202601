using PolyQA.Network;
using PolyQA.Observer;
using UnityEngine;

namespace PolyQA.Executor
{
    public interface IUpdater
    {
        void Update();
    }

    /// <summary>
    /// 各クラスがそれぞれMonoBehaviourのUpdateを実装するとパフォーマンスのコントロールが難しくなりそうなので、
    /// ここで集中管理する
    /// </summary>
    public class UpdateExecutor
    {
        private readonly RuntimeContext _context;
        private readonly ListUpdater _gameObjectObserverUpdater = new ();

        public UpdateExecutor(RuntimeContext context)
        {
            _context = context;
        }

        public void Update()
        {
            var mod = Time.frameCount % 10;
            if (mod == 0)
            {
                DataSenderInternal.Update();
            }
            else if(mod == 1)
            {
                _context.InputService.Update();
            }
            else
            {
                _gameObjectObserverUpdater.Update();
            }

            _context.InputRecordService.Update();
        }

        public void Add(GameObjectObserver receiver)
        {
            _gameObjectObserverUpdater.Add(receiver);
        }

        public void Remove(GameObjectObserver receiver)
        {
            _gameObjectObserverUpdater.Remove(receiver);
        }
    }
}