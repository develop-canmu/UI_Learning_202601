using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace CruFramework.Page
{
    
    public abstract class SheetManager : MonoBehaviour
    {
    }
    
    [FrameworkDocument("Page", nameof(SheetManager), "シート管理用クラス。タブでUIの切り替えを行う")]
    public abstract class SheetManager<T> : SheetManager where T : System.Enum
    {
        [SerializeField]
        private bool openOnAwake = true;
        [SerializeField]
        private T firstSheet = default;
        public T FirstSheet{get{return firstSheet;}}
        
        // シートのボタン
        private Dictionary<T, SheetSwitchButton> sheetSwitchButtons = new SerializedDictionary<T, SheetSwitchButton>();
        // シート
        private Dictionary<T, Sheet> sheetObjects = new SerializedDictionary<T, Sheet>();
        
        private Sheet currentSheet = null;
        /// <summary>現在開いているシート</summary>
        public Sheet CurrentSheet{get{return currentSheet;}}
        
        private T currentSheetType = default;
        /// <summary>現在開いているシート</summary>
        public T CurrentSheetType{get{return currentSheetType;}}
        
        private bool canOpenSheet = true;
        /// <summary>シートが開けるか</summary>
        public bool CanOpenSheet{get{return canOpenSheet;}}
        
        public event System.Action<T> OnOpenSheet = null;
        public event System.Action<T> OnPreOpenSheet = null;
        
        private bool isOpenedSheet = false;
        
        private bool isInitialized = false;
        
        private void Awake()
        {
            Initialize();
            if(openOnAwake)
            {
                currentSheetType = firstSheet;
                // 最初のシートを開く
                OpenSheet(firstSheet, null);
            }
        }
        
        private  void Initialize()
        {
            // 初期化済み
            if(isInitialized)return;
            isInitialized = true;
            
            // シート切り替えボタンを取得
            foreach(T value in System.Enum.GetValues(typeof(T)))
            {
                // 名前で取得
                Transform sheetObject = transform.Find(value.ToString());
                // オブジェクトが見つからない
                if(sheetObject == null)
                {
                    continue;
                }
                
                // シートを取得
                Sheet sheet = sheetObject.GetComponent<Sheet>();
                
                if(sheet == null)
                {
                    continue;
                }
                // リストに追加
                sheetObjects.Add(value, sheet);
                // 一旦閉じる
                sheet.OnClosedInternal();
                sheet.gameObject.SetActive(false);
            }

        }

        protected virtual void OnDisable()
        {
            isOpenedSheet = false;
        }

        /// <summary>ボタンの登録</summary>
        internal void RegisterButton(T sheetType, SheetSwitchButton button)
        {
            sheetSwitchButtons.Add(sheetType, button);
            if(currentSheetType.Equals(sheetType))
            {
                button.OnOpenInternal();
            }
            else
            {
                button.OnCloseInternal();
            }
        }
        
        [FrameworkDocument("シートを開く")]
        /// <summary>シートを開く</summary>
        public void OpenSheet(T sheetType, object args, bool isForceOpen = false)
        {
            OpenSheetAsync(sheetType, args, isForceOpen).Forget();
        }
        
        /// <summary>シートを開く</summary>
        public async UniTask OpenSheetAsync(T sheetType, object args, bool isForceOpen = false)
        {
            Initialize();
            if(canOpenSheet == false)return;
            
            // 同じシートは開けない
            if(isForceOpen == false && isOpenedSheet == true && currentSheetType.Equals(sheetType))return;
            
            // 一度シートを開いた
            isOpenedSheet = true;
            
            

            
            // シートを開けないように
            canOpenSheet = false;
                        
            if(sheetSwitchButtons.TryGetValue(currentSheetType, out SheetSwitchButton closeButton))
            {
                closeButton.OnCloseInternal();
            }
            
            if(sheetSwitchButtons.TryGetValue(sheetType, out SheetSwitchButton openButton))
            {
                openButton.OnOpenInternal();
            }
            
            // 現在のシートタイプを設定
            currentSheetType = sheetType;
            
            // 開く前の通知
            if(OnPreOpenSheet != null)
            {
                OnPreOpenSheet(sheetType);
            }
            
            if(sheetObjects.TryGetValue(sheetType, out Sheet sheetObject))
            {
                // 開く
                await sheetObject.OnPreOpenInternal(args);
            }
            
            if(currentSheet != null)
            {
                // 開いているシートを閉じる
                await currentSheet.OnPreCloseInternal();
                // 閉じたときの通知
                currentSheet.OnClosedInternal();
                currentSheet.gameObject.SetActive(false);
            }

            // 開いているシートを保持
            currentSheet = sheetObject;
            
            if(sheetObject != null)
            {
                sheetObject.gameObject.SetActive(true);
                // 開いたときの通知
                await sheetObject.OnOpenInternal(args);
                // 開いたときの通知
                sheetObject.OnOpenedInternal(args);
            }
            
            // 通知
            if(OnOpenSheet != null)
            {
                OnOpenSheet(sheetType);
            }
            
            // シートを開けるように
            canOpenSheet = true;
        }
    }
}