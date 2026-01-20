using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;


namespace CruFramework.Adv
{
    public interface IAdvCommand : IAdvCommandObject
    {
        void Execute(AdvManager manager);
    }
    
    public interface IAdvFastForward
    {
        bool OnNext(AdvManager manager);
    }
}