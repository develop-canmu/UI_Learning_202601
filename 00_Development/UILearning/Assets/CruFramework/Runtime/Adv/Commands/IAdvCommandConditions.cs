using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Adv
{

    public interface IAdvCommandConditions : IAdvCommandObject
    {
        bool GetConditions(AdvManager manager);
    }
}