using UnityEngine;


namespace Pjfb.Club {

    public class ClubToggleParam : MonoBehaviour {
        public int param => _param;

        [SerializeField]
        int _param = -1;
    }
}