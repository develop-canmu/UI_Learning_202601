using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using TMPro;
using CruFramework.UI;

namespace Pjfb.Club {
    public class FindClubPageSheet : Sheet {

        protected FindClubPage _page = null;

        public void SetPage(FindClubPage page){
            _page = page;
        }

    }
}