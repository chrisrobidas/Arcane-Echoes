using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsyncOperationGroup
{
    public readonly List<AsyncOperation> Operations = new List<AsyncOperation>();

    public float Progress => Operations.Count == 0 ? 0 : Operations.Average(o => o.progress);
    public bool IsDone => Operations.All(o => o.isDone);
}
