using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MinigameBase : MonoBehaviour
{
    public MinigameState Finished
    {
        get;
        private set;
    }

    public virtual void Start()
    {
        Finished = MinigameState.Unfinished;
    }

    protected void Finish(bool success)
    {
        Finished = success ? MinigameState.Suceeded : MinigameState.Failed;
    }
}

public enum MinigameState
{
    Unfinished = 0,
    Suceeded = 1,
    Failed = -1,
}
