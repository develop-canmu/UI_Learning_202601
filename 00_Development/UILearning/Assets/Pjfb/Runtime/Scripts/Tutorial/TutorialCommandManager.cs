using System.Threading;
using CruFramework.ResourceManagement;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace Pjfb
{
    public class TutorialCommandManager : MonoBehaviour
    {
        [SerializeField] private Transform focusRoot;
        [SerializeField] private Transform tapEffectRoot;
        [SerializeField] private GameObject backGround;
        [SerializeField] private GameObject tapEffectPrefab;
        
        private const string TutorialFocusSettingPath = "Tutorial/TutorialCommand/";
        private const string SortingLayer = "Tutorial";
        private const int SortingOrder = 1;
        private ResourcesLoader resourcesLoader = new ResourcesLoader(); 
        private GameObject tapEffect;
        
        private struct CanvasData
        {
            public string sortingLayer;
            public int sortingOrder;
            public GraphicRaycaster graphicRaycaster;
            public bool hasGraphicRaycaster;
        }
        
        // チュートリアルの実行
        public async UniTask ActionTutorial(string tutorialId)
        {
            CancellationToken token = this.GetCancellationTokenOnDestroy();
            string pass = TutorialFocusSettingPath + tutorialId + ".asset";
            
            TutorialCommandSetting tutorialSetting = await resourcesLoader.LoadAssetAsync<TutorialCommandSetting>(pass, token);
            
            // 各コマンドの実行
            foreach (var actionSetting in tutorialSetting.ActionSettingList)
            {
                await actionSetting.Command.ActionCommand(token);
            }
            resourcesLoader.Release();
        }
     
        // ボタンにフォーカスする処理
        public async UniTask FocusTarget(GameObject targetObject,CancellationToken token)
        {
            backGround.SetActive(true);
            bool hasCanvas;
            
            // まず自身にキャンバスを付ける
            Canvas targetCanvas = targetObject.GetComponent<Canvas>();
            if(targetCanvas == null)
            {
                hasCanvas = false;
                targetCanvas = targetObject.AddComponent<Canvas>();
            }
            else
            {
                hasCanvas = true;
            }
            // 子も含めたキャンバスの取得
            Canvas[] canvasList = targetObject.GetComponentsInChildren<Canvas>(true);
            // 各種キャンバスの値の変更
            CanvasData[] canvasData = new CanvasData[canvasList.Length];
            for(int i = 0; i < canvasList.Length; i++)
            {
                // 元の値を保持
                canvasData[i].sortingLayer = canvasList[i].sortingLayerName;
                canvasData[i].sortingOrder = canvasList[i].sortingOrder;
                canvasData[i].graphicRaycaster = canvasList[i].GetComponent<GraphicRaycaster>();
                if (canvasData[i].graphicRaycaster == null)
                {
                    canvasData[i].graphicRaycaster = canvasList[i].gameObject.AddComponent<GraphicRaycaster>();
                    canvasData[i].hasGraphicRaycaster = false;
                }
                else
                {
                    canvasData[i].hasGraphicRaycaster = true;
                }
                
                // 現在の描画順を取得
                int renderOrder = canvasList[i].renderOrder;
                
                // キャンバスのデータを更新
                canvasList[i].overrideSorting = true;
                canvasList[i].sortingLayerName = SortingLayer;
                canvasList[i].sortingOrder = SortingOrder + renderOrder;
            }
            
            // エフェクトをたく
            // モーダルを開いている最中の場合座標がずれるため待機
            await UniTask.WaitWhile(() => AppManager.Instance.UIManager.ModalManager.IsRunOpen , cancellationToken: token); 
            RectTransform rectTransform = (RectTransform)targetObject.transform;
            Vector3 position = targetObject.transform.position;
            // 真ん中の位置を取得する
            var scale = rectTransform.transform.lossyScale;
            var rect = rectTransform.rect;
            var width = rect.width * 0.5f * scale.x;
            var height = rect.height * 0.5f * scale.y;
            position.x += Mathf.Lerp(width, -width, rectTransform.pivot.x);
            position.y += Mathf.Lerp(height, -height, rectTransform.pivot.y);

            TapEffect(position);
            
            var targetButton = targetObject.GetComponentInChildren<UIButton>();
            
            // このままだとロングタップが反応してしまうので一時的に無効にする
            var longTapMode = targetButton.LongTapTriggerMode;
            targetButton.LongTapTriggerMode = CruFramework.UI.UIButton.LongTapTriggerType.None;
            
            
            // ボタン押下待ち処理
            var eventHandler = targetButton.onClick.GetAsyncEventHandler(token);
            await eventHandler.OnInvokeAsync();
            
            // キャンパスの状態を元に戻す
            for (int i = 0; i < canvasList.Length; i++)
            {
                canvasList[i].sortingOrder = canvasData[i].sortingOrder;
                canvasList[i].sortingLayerName = canvasData[i].sortingLayer;
                if (canvasData[i].hasGraphicRaycaster == false)
                {
                    Destroy(canvasData[i].graphicRaycaster);
                }
            }
            if (hasCanvas == false)
            {
                Destroy(targetCanvas);
            }
            
            targetButton.LongTapTriggerMode = longTapMode;
            
            ExitAction();
        }

        // ボタンのフォーカスを解除する処理
        private void ExitAction()
        {
            backGround.SetActive(false);
            if(tapEffect != null)
            {
                tapEffect.SetActive(false);
            }
        }
        
        // タップエフェクトの表示
        private void TapEffect(Vector3 pos)
        {
            if (tapEffect == null)
            {
                tapEffect = Instantiate(tapEffectPrefab,tapEffectRoot);
            }
            tapEffect.SetActive(true);
            tapEffect.transform.position = pos; 
        }

        private void OnDestroy()
        {
            resourcesLoader.Release();
        }
    }
}