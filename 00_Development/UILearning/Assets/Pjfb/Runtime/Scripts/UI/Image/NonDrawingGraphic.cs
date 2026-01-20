using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{
    /// <summary>
    /// RectTransformをボタンとして扱うためのクラス. 標準のImage等だとalpha=0にしても無駄な描画が走るのでこちらを推奨
    /// </summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public class NonDrawingGraphic : Graphic
    {
        public override void SetMaterialDirty() { return; }
        public override void SetVerticesDirty() { return; }
        
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            return;
        }
    }
}