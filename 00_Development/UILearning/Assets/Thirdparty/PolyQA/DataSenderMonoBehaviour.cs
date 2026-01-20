using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

#if POLYQA_DISABLE
#pragma warning disable CS0414
#endif

namespace PolyQA
{
    [DefaultExecutionOrder(10000)]
    public class DataSenderMonoBehaviour : MonoBehaviour
    {
#if !POLYQA_DISABLE
        private static DataSenderMonoBehaviour _instance;
        private readonly ILogger _logger = Logging.CreateLogger<DataSenderMonoBehaviour>();
        private RuntimeContext _context;

        private void Awake()
        {
            if(_instance)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (_instance != this) return;

#if !POLYQA_MANUAL_LAUNCH
            LaunchInternal();
#endif
        }

        internal static void Launch()
        {
            _instance.LaunchInternal();
        }

        private void LaunchInternal()
        {
            if (_context != null) return;
            _context = new RuntimeContext(this);
        }

        private void Update()
        {
            _context?.UpdateExecutor.Update();
        }

        private void OnDestroy()
        {
            _context?.Dispose();
        }
#endif
    }
}