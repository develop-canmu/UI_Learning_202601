using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using UnityEditor;
using UnityEngine.UI;

namespace Pjfb
{
    public class ScrollGridEditor : CruFramework.Editor.UI.ScrollGridEditor
    {
        [MenuItem("GameObject/UI/Pjfb/ScrollGrid/Horizontal")]
        public static void CreatePjfbCustomHorizontalMenu()
        {
            CreateHorizontalMenu();
            
            CustomizeGrid();
        }
        [MenuItem("GameObject/UI/Pjfb/ScrollGrid/Vertical")]
        public static void CreatePjfbCustomVerticalMenu()
        {
            CreateVerticalMenu();

            CustomizeGrid();
        }

        /// <summary> PJFB用カスタマイズ </summary>
        private static void CustomizeGrid()
        {
            ScrollGrid scrollGrid = Selection.activeGameObject.GetComponent<ScrollGrid>();
            
            // マスク・NonDrawingGraphicを置き換える
            DestroyImmediate(scrollGrid.viewport.GetComponent<Mask>());
            DestroyImmediate(scrollGrid.viewport.GetComponent<Image>());
            // マスクを追加
            scrollGrid.viewport.gameObject.AddComponent<NonDrawingGraphic>();
            scrollGrid.viewport.gameObject.AddComponent<RectMask2D>();
            
            // ScrollGridの背景を設定
            Image backgroundImage = scrollGrid.GetComponent<Image>();
            backgroundImage.sprite = LoadCommonSprite("common_shape_square_s");
            backgroundImage.raycastTarget = false;
            // 285F8C 20%
            ColorUtility.TryParseHtmlString("#285F8C33", out Color backgroundColor);
            backgroundImage.color = backgroundColor;
            // Stretchにする
            RectTransform rectTransform = (RectTransform)scrollGrid.transform;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            // ズレ調整
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            
            // ViewportのTransformを設定
            scrollGrid.viewport.offsetMin = Vector2.zero;
            scrollGrid.viewport.offsetMax = Vector2.zero;
            
            // スクロールバー
            // スクロールバーの向きを取得
            bool isVertical = scrollGrid.Type == ScrollGrid.ScrollType.Vertical || scrollGrid.Type == ScrollGrid.ScrollType.VerticalPage;
            // スクロールバー取得
            RectTransform scrollbarRect = isVertical ? (RectTransform)scrollGrid.verticalScrollbar.transform : (RectTransform)scrollGrid.horizontalScrollbar.transform;
            if (isVertical == false)
            {
                // Horizontalの場合は運用上バーを出すことが少ないので削除
                DestroyImmediate(scrollbarRect.gameObject);
                scrollGrid.horizontalScrollbar = null;
                return;
            }
            
            // VerticalScrollBar
            // スクロールバーの設定を取得
            Sprite scrollbarSprite = LoadCommonSprite("common_base_scrollbar");
            ColorUtility.TryParseHtmlString("#A9A9A9FF", out Color scrollbarColor);
            
            // Imageを上書き
            Image scrollbarImage = scrollbarRect.GetComponent<Image>();
            scrollbarImage.type = Image.Type.Sliced;
            scrollbarImage.sprite = scrollbarSprite;
            scrollbarImage.color = scrollbarColor;
            // sizeDeltaを上書き
            scrollbarRect.sizeDelta = new Vector2(10, 0);
            // Top, Bottomをそれぞれ40にする
            scrollbarRect.offsetMin += new Vector2(0, 40);
            scrollbarRect.offsetMax += new Vector2(0, -40);
            // PosXを-10
            scrollbarRect.anchoredPosition += new Vector2(-10, 0);
            
            // SlidingAreaのTransformを上書き
            RectTransform slidingArea = (RectTransform)scrollbarRect.GetChild(0).transform;
            slidingArea.offsetMin = Vector2.zero;
            slidingArea.offsetMax = Vector2.zero;
            
            // Handleの設定を取得
            Sprite handleSprite = LoadCommonSprite("common_handle_scrollbar");
            ColorUtility.TryParseHtmlString("#FFFFFFFF", out Color handleColor);
            RectTransform handleRect = scrollGrid.verticalScrollbar.handleRect;
            
            // Imageを上書き
            Image handleImage = handleRect.GetComponent<Image>();
            handleImage.type = Image.Type.Sliced;
            handleImage.sprite = handleSprite;
            handleImage.color = handleColor;
            // Transformを上書き
            handleRect.offsetMin = Vector2.zero;
            handleRect.offsetMax = Vector2.zero;
        }
        
        
        private static Sprite LoadCommonSprite(string commonName)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Pjfb/Runtime/AssetBundles/Local/Images/UI/Common/Images/{commonName}.png");
        }
    }
}