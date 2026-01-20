using System;
using UnityEngine;

namespace Pjfb
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class SerializeReferenceDropdownAttribute : PropertyAttribute
    {
    }
}
