using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pjfb
{
    public interface IFade
    {
        UniTask FadeInAsync(CancellationToken token);
        
        UniTask FadeOutAsync(CancellationToken token);
    }
}
