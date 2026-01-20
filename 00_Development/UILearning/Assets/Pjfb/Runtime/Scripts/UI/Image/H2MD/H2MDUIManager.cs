using System.Collections;
using System.Collections.Generic;
using CruFramework.H2MD;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.UI
{
    public class H2MDUIManager : MonoBehaviour
    {
        private enum H2MDState
        {
            // 再生中
            Play,
            // 停止中
            Stop,
        }
        
        private List<H2MDController> effectControllerList = new List<H2MDController>();
        
        private H2MDState effectState = H2MDState.Play;
        // 停止処理を呼んだモーダルの数
        private int stopperCount = 0;
        
        
        // マネージャーにコントローラーを登録
        public void AddEffect(H2MDController effectController)
        {
            effectControllerList.Add(effectController);
            // マネージャーで停止中の場合停止する
            switch (effectState)
            {
                case H2MDState.Play:
                    effectController.Play();
                    break;
                case H2MDState.Stop:
                    effectController.Stop();
                    break;
            }
        }
        
        public void RemoveEffect(H2MDController effectController)
        {
            effectControllerList.Remove(effectController);
        }
        
        public void PlayEffect()
        {
            stopperCount--;
            // 再生中もしくは停止処理を呼んだモーダルがまだ存在する場合そのまま
            if(effectState == H2MDState.Play || stopperCount > 0) return;
            effectState = H2MDState.Play;
            // リストに登録されているH2MDPlayerを全て再生
            foreach (H2MDController effectController in effectControllerList)
            {
                effectController.Play();
            }
        }
        
        public void StopEffect()
        {
            stopperCount++;
            if(effectState == H2MDState.Stop) return;
            effectState = H2MDState.Stop;
            foreach (H2MDController h2mdController in effectControllerList)
            {
                if (h2mdController.isPlaying)
                {
                    h2mdController.Stop();
                }
            }
        }
    }
}