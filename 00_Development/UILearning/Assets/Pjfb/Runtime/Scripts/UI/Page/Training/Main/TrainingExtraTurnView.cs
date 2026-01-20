using UnityEngine;
using TMPro;

namespace Pjfb.Training
{
    public class TrainingExtraTurnView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI addTurnText;
        [SerializeField] private TextMeshProUGUI maxTurnText;

        private bool isInitialized = false;
        
        public void UpdateTurn(long addedTurn)
        {
            gameObject.SetActive(true);
            addTurnText.text = addedTurn.ToString();
            if (!isInitialized)
            {
                isInitialized = true;
                maxTurnText.text = TrainingUtility.GetMaxAddTurn().ToString();
            }
        }
    }
}