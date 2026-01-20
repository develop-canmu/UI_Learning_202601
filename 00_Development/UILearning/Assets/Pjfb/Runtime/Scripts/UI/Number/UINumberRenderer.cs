using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace CruFramework.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class UINumberRenderer : Graphic
    {
        /// <summary>置換文字列</summary>
        public static readonly string ReplaceString = "{value}";
        
        /// <summary>スプライトの種類</summary>
        public const int SpriteTypeCount = 11;
        /// <summary>マイナス</summary>
        public const int Minus = 10;
        
        [SerializeField]
        private SpriteAtlas atlas = null;
        /// <summary>アトラス</summary>
        public SpriteAtlas Atlas
        {
            get
            {
                return atlas;
            }
            set
            {
                atlas = value;
                UpdateSprites();
            }
        }
        
        [SerializeField]
        private int value = 0;
        /// <summary>表示する値</summary>
        public int Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                SetVerticesDirty();
            }
        }
        
        [SerializeField]
        private float scale = 1.0f;
        public float Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
                SetVerticesDirty();
            }
        }

        [SerializeField]
        private string spriteName = string.Empty;
        /// <summary>
        /// スプライト名
        /// {value} は値で置換される
        /// </summary>
        public string SpriteName
        {
            get
            {
                return spriteName;
            }
            set
            {
                spriteName = value;
                UpdateSprites();
            }
        }
        
        [SerializeField]
        private Vector2 pivot = new Vector2(0.5f, 0.5f);
        /// <summary>Pivot</summary>
        public Vector2 Pivot
        {
            get
            {
                return pivot;
            }
            set
            {
                pivot = value;
                SetVerticesDirty();
            }
        }
        
        [SerializeField]
        private Vector2 anchor = new Vector2(0.5f, 0.5f);
        /// <summary>Anchor</summary>
        public Vector2 Anchor
        {
            get
            {
                return anchor;
            }
            set
            {
                anchor = value;
                SetVerticesDirty();
            }
        }

        public override Texture mainTexture
        {
            get
            {
                return sprites[0].texture;
            }
        }


        // 数字リスト
        private List<int> numList = new List<int>();
        // スプライトのキャッシュ
        private Sprite[] sprites = new Sprite[SpriteTypeCount];

        // スプライトの更新
        private void UpdateSprites()
        {
            // 数字スプライト
            for(int i=0;i<10;i++)
            {
                sprites[i] = atlas.GetSprite( spriteName.Replace( ReplaceString, $"{i}" ) );
            }
            // マイナス記号
            sprites[Minus] = atlas.GetSprite( spriteName.Replace( ReplaceString, "-" ) );
        }

        protected override void Awake()
        {
            UpdateSprites();
            SetVerticesDirty();
            base.Awake();
        }
        
#if UNITY_EDITOR

        protected override void OnValidate()
        {
            UpdateSprites();
            SetVerticesDirty();
            base.OnValidate();
        }
        
#endif

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            // 幅
            float width = 0;
            // 高さ
            float height = 0;
            // リスト初期化
            numList.Clear();

            int v = Mathf.Abs(value);
            // 数字をバラにしてリスト化
            while(true)
            {
                // １桁分取得
                int n = v % 10;
                // リストに追加
                numList.Insert(0, n);
                // サイズを取得
                Vector2 size = sprites[n].textureRect.size;
                width += size.x;
                height = Mathf.Max(height, size.y);
                // チェック終了
                if(v < 10)break;
                // 次の桁
                v /= 10;
            }
            
            // マイナス
            if(value < 0)
            {
                numList.Insert(0, Minus);
                // サイズを取得
                Vector2 size = sprites[Minus].textureRect.size;
                width += size.x;
                height = Mathf.Max(height, size.y);
            }
            
            
            
            // Pivotの計算
            Vector2 offset = new Vector2(-width * pivot.x, -height * pivot.y);
            // Anchorの計算
            Vector2 rectSize = rectTransform.rect.size;
            offset.x += (anchor.x- 0.5f) * rectSize.x;
            offset.y += (anchor.y- 0.5f) * rectSize.y;
            
            offset.x -= rectSize.x * (rectTransform.pivot.x - 0.5f);
            offset.y -= rectSize.y * (rectTransform.pivot.y - 0.5f);
            // 頂点座標
            Vector2 pos = offset;
            
            int index = 0;
            vh.Clear();
            // メッシュを構築
            foreach(int num in numList)
            {
                Sprite sprite = sprites[num];
                Vector2 p = sprite.textureRect.position * scale;
                Vector2 size = sprite.textureRect.size * scale;

                float w = sprite.texture.width * scale;
                float h = sprite.texture.height * scale;
                
                vh.AddVert( pos                             , color, new Vector2(  p.x / w     ,  p.y / h));
                vh.AddVert( pos + new Vector2(0, size.y)    , color, new Vector2(  p.x / w     , (p.y + size.y) / h) );
                vh.AddVert( pos + new Vector2(size.x, size.y), color, new Vector2( (p.x + size.x) / w, (p.y + size.y) / h) );
                vh.AddVert( pos + new Vector2(size.x, 0)     , color, new Vector2( (p.x + size.x) / w,  p.y / h));
                
                pos.x += size.x;
                
                vh.AddTriangle(index + 0, index + 1, index + 2);
                vh.AddTriangle(index + 0, index + 2, index + 3);
                index += 4;
                
            }
        }
    }
}
