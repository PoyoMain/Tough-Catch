using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MinigameBase : MonoBehaviour
{
    [Header("ScriptableObjects")]
    [SerializeField] private OptionsSO _options;
    [SerializeField] private InputReaderSO _inputReader;

    public MinigameState Finished { get; private set; }
    protected OptionsSO Options => _options;
    protected PlayerControls.GameplayControlsActions Controls => _inputReader.Controls;

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