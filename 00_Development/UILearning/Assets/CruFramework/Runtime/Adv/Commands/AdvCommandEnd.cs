using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Adv
{
    public class AdvCommandEnd : IAdvCommand
    {
        void IAdvCommand.Execute(AdvManager manager)
        {
            manager.End();
        }
    }
}