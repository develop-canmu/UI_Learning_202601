using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

#if CRUFRAMEWORK_WEBVIEW_SUPPORT

namespace CruFramework
{
    public class Webview : UniWebView
    {
        public static Webview Create(RectTransform parent, string url, string scheme)
        {
            // GameObject
            GameObject obj = new GameObject();
            // 名前
            obj.name = "Webview";
            // Recttransform
            RectTransform uiTransform = obj.AddComponent<RectTransform>();
            // 親の設定
            obj.transform.SetParent(parent);
            // Wevview
            Webview webview = obj.AddComponent<Webview>();
            
            // UIの設定
            uiTransform.anchorMin = Vector2.zero;
            uiTransform.anchorMax = Vector2.one;
            
            uiTransform.position = Vector3.zero;
            uiTransform.localScale = Vector3.one;
            
            uiTransform.sizeDelta = parent != null ? parent.sizeDelta : Vector2.zero;
            uiTransform.anchoredPosition = Vector2.zero;
            
            webview.ReferenceRectTransform = uiTransform;
            
            // スキーマ
            if(string.IsNullOrEmpty(scheme) == false)
            {
                webview.AddUrlScheme(scheme);
            }
            
            // サイズ設定
            webview.SetSize(parent);
            
            // Url読み込み
            webview.Load(url);

            return webview;
        }
        
        public void SetSize(RectTransform uiTransform)
        {
            if(uiTransform != null)
            {
                Frame = new Rect(0, 0, uiTransform.rect.width, uiTransform.rect.height);
            }
        }
        
        public  void LoadFromAsset(string url)
        {
        	url = UniWebViewHelper.StreamingAssetURLForPath(url);
        	Load(url);
        }
        
        /// <summary>JavaScript側の関数呼び出し</summary>
        public void CallJavaScript(string function)
        {
            CallJavaScript(function, null);
        }
        
        /// <summary>JavaScript側の関数呼び出し</summary>
        public void CallJavaScript(string function, params object[] args)
        {
            // 実行する文字列
            StringBuilder sb = new StringBuilder();
            sb.Append(function);
            sb.Append("(");
			
            if(args != null)
            {
                for(int i=0;i<args.Length;i++)
                {
                    if(i > 0)sb.Append(',');
                    switch(args[i])
                    {
                        case string v:
                            sb.Append('"');
                            sb.Append(v);
                            sb.Append('"');
                            break;
                        default:
                            sb.Append(args[i].ToString());
                            break;
                    }
                }
            }
			
            sb.Append(")");
            CallJavaScriptCore(sb.ToString());
            sb.Clear();
        }
        
        private void CallJavaScriptCore(string function)
        {
            EvaluateJavaScript(function, null);
        }
        
        private void OnRectTransformDimensionsChange()
        {
            UpdateFrame();
        }
    }
}

#endif // CRUFRAMEWORK_WEBVIEW_SUPPORT