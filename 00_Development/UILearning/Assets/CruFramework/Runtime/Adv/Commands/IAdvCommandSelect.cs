using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;


namespace CruFramework.Adv
{
    public interface IAdvCommandSelect : IAdvCommandObject
    {
        int GetSelectCount();
    }
}