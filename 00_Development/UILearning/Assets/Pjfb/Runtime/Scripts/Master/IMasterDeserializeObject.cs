using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Master
{
    public interface IMasterDeserializeObject
    {
        System.Collections.IList values { get; }
    }
}