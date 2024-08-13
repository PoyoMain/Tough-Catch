using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MinigameBase : MonoBehaviour
{
    public MinigameState Finished { get; private set; }

    protected PlayerControls.GameplayControlsActions Controls => GameManager.Instance.Controls;
    protected ScriptableOptions Options => GameManager.Instance.Options;


    public virtual void OnEnable()
    {
        Finished = MinigameState.Unfinished;
    }

    protected void Finish(bool success = true)
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
