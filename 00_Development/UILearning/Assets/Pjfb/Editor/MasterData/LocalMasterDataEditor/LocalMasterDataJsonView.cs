using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;

namespace Pjfb.Editor.LocalMasterDataEditor
{
    public class LocalMasterDataJsonView : EditorWindow
    {
        public static LocalMasterDataJsonView Open(string json)
        {
            Json = json;
            return GetWindow<LocalMasterDataJsonView>();
        }

        // json
        private static string _json;
        private static string Json
        {
            set
            {
                _updateJson = true;
                _json = value;
            }
        }
        
        private static bool _updateJson;

        private void OnGUI()
        {
            if (_updateJson)
            {
                _updateJson = false;
                CreateVisual();
            }
        }

        private ScrollView _scrollView;
        private TextField _jsonField;
        
        private void CreateVisual()
        {
            rootVisualElement.Clear();
            _scrollView ??= new ScrollView();
            _scrollView.Clear();
            _jsonField = new TextField
            {
                value = _json
            };
            _scrollView.Add(_jsonField);
            rootVisualElement.Add(_scrollView);
            var applyButton = new Button
            {
                text = "Apply",
                clickable = new Clickable(ApplyClose)
            };
            rootVisualElement.Add(applyButton);
        }
        
        private void ApplyClose()
        {
            _json = _jsonField.value;
            Close();
        }

        public async UniTask<string> WaitCloseAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await UniTask.WaitWhile(()=> this != null, cancellationToken: token);
            return _json;
        }
    }
}