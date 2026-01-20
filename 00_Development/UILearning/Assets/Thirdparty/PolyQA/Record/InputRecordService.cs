using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace PolyQA.Record
{
    public interface IInputRecorder
    {
        void Update();
    }

    public class InputRecordService
    {
        private readonly ILogger _logger = Logging.CreateLogger<InputRecordService>();
        private readonly RuntimeContext _context;
        private readonly HashSet<IInputRecorder> _recorders = new();

        public InputRecordService(RuntimeContext context)
        {
            _context = context;
        }

        public void StartRecording()
        {
            _logger.LogInformation("Start recording");
#if POLYQA_USE_INPUT_SYSTEM
            _recorders.Add(new UnityInputSystemRecorder(_context));
            return;
#endif
#if POLYQA_USE_UGUI
#pragma warning disable CS0162 // Unreachable code detected
            _recorders.Add(new UnityLegacyInputRecorder(_context));
#pragma warning restore CS0162 // Unreachable code detected
#endif
        }

        public void StopRecording()
        {
            _logger.LogInformation("Stop recording");
            _recorders.Clear();
        }

        public void Update()
        {
            foreach (var r in _recorders)
            {
                r.Update();
            }
        }
    }
}