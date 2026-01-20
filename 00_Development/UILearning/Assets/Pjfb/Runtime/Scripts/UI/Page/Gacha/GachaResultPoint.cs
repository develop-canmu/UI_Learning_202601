using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Pjfb.Gacha
{
    /// <summary>
    /// ガチャリザルトのポイント表示コンポーネント
    /// </summary>
    public class GachaResultPoint : MonoBehaviour {
        [SerializeField]
        private GameObject pointObject = null;
        [SerializeField]
        private TextMeshProUGUI beforePoint = null;
        [SerializeField]
        private TextMeshProUGUI afterPoint = null; 
        [SerializeField]
        private Image arrow = null; 

               

        //表示更新
        public void UpdateView( long pointId, long before, long after ){
            pointObject.SetActive(pointId != 0);
            beforePoint.text = before.ToString();
            afterPoint.text = after.ToString();
            arrow.gameObject.SetActive(true);
            beforePoint.gameObject.SetActive(true);
        }

        public void UpdateView( long pointId, long after ){
            pointObject.SetActive(pointId != 0);
            afterPoint.text = after.ToString();
            arrow.gameObject.SetActive(false);
            beforePoint.gameObject.SetActive(false);
        }
    }
}