using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace PolyQA.Input
{
    public interface IInputServiceImpl
    {
    }

    public interface IInputServiceUpdate
    {
        void Update();
    }

    public class InputService : IDisposable
    {
        private readonly ILogger _logger = Logging.CreateLogger<InputService>();
        private readonly IInputServiceImpl[] _implementations;

        public InputService(InputState inputState)
        {
            _implementations = new IInputServiceImpl[]
            {
#if POLYQA_USE_UGUI
                new UnityUIInputService(inputState),
#endif
#if POLYQA_USE_INPUT_SYSTEM
                new UnityInputSystemService(inputState),
#endif
            };
        }

        public void Update()
        {
            foreach (var s in _implementations.OfType<IInputServiceUpdate>())
            {
                try
                {
                    s.Update();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to update input service");
                }
            }
        }

        public void Dispose()
        {
            foreach (var s in _implementations.OfType<IDisposable>())
            {
                s.Dispose();
            }
        }
    }
}