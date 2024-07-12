using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameState _state;

    [Header("Cameras")]
    [SerializeField] private CinemachineVirtualCamera povCam;
    [SerializeField] private CinemachineVirtualCamera dockCam;
    [SerializeField] private CinemachineVirtualCamera lakeCam;

    [Space(20)]
    [SerializeField] private PauseMenu pauseMenu;

    private bool temp = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            Debug.LogError("More than one instance of GameManager in scene." +
                "\n Located on " + this.name + " and " + Instance.name);
        }

        Instance = this;

        VariableCheck();
    }

    void VariableCheck()
    {
        if (povCam == null) Debug.LogError("Pov Camera not set in Inspector");
        if (dockCam == null) Debug.LogError("Dock Camera not set in Inspector");
        if (lakeCam == null) Debug.LogError("Lake Camera not set in Inspector");
    }

    void Start()
    {
        ChangeState(GameState.Scan);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pauseMenu.IsPaused) pauseMenu.Pause();
            else pauseMenu.UnPause();
        }

        if (pauseMenu.IsPaused) return;

        if (Input.GetKeyDown(KeyCode.O)) temp = true;
    }

    private void ChangeState(GameState newState)
    {
        _state = newState;

        switch (newState)
        {
            case GameState.Menu:
                break;
            case GameState.Scan:
                StartCoroutine(ScanCoroutine());
                break;
            case GameState.Cast:
                StartCoroutine(CastCoroutine());
                break;
            case GameState.Tuggle:
                StartCoroutine(TuggleCoroutine());
                break;
            case GameState.Reel:
                StartCoroutine(ReelCoroutine());
                break;
            case GameState.Results:
                break;
        }
    }

    #region Gameplay Methods

    private IEnumerator ScanCoroutine()
    {
        ActivateCamera(lakeCam);

        while (!temp)
        {
            yield return null;
        }
        temp = false;

        ChangeState(GameState.Cast);
        yield break;
    }

    private IEnumerator CastCoroutine()
    {
        ActivateCamera(dockCam);

        while (!temp)
        {
            yield return null;
        }
        temp = false;

        ChangeState(GameState.Tuggle);
        yield break;
    }

    private IEnumerator TuggleCoroutine()
    {
        ActivateCamera(povCam);

        while (!temp)
        {
            yield return null;
        }
        temp = false;

        ChangeState(GameState.Reel);
        yield break;
    }

    private IEnumerator ReelCoroutine()
    {
        ActivateCamera(dockCam);

        yield break;
    }

    #endregion

    private void ActivateCamera(CinemachineVirtualCamera newCam)
    {
        povCam.enabled = (povCam == newCam);
        dockCam.enabled = (dockCam == newCam);
        lakeCam.enabled = (lakeCam == newCam);
    }
}

public enum GameState
{
    Menu,
    Scan,
    Cast,
    Tuggle,
    Reel,
    Results
}