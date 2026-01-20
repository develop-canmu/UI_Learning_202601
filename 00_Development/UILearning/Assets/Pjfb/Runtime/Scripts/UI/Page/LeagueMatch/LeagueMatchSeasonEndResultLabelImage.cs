using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace Pjfb.LeagueMatch
{
    public class LeagueMatchSeasonEndResultLabelImage : CancellableImageWithId
    {
        protected override string GetKey(long id)
        {
            return PageResourceLoadUtility.GetLeagueMatchSeasonEndLabelPath(id);
        }
        
        /// <summary>画像セット</summary>
        public UniTask SetTextureAsync(SeasonEndResultType value)
        {
            return SetTextureAsync((long)value);
        }
    }
}