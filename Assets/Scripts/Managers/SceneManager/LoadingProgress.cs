using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingProgress : IProgress<float>
{
    public event Action<float> Progressed;
    const float ratio = 1f;
    public void Report(float value)
    {
        Progressed?.Invoke(value/ratio);
    }
}
