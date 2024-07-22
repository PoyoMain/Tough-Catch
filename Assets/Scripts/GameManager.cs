using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private LaserMinigame _laserMinigame;

    private PlayerControls _playerControls;
    public PlayerControls.ControlsActions Controls
    {
        get;
        private set;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            Debug.LogError("More than one instance of GameManager in scene." +
                "\n Located on " + this.name + " and " + Instance.name);
        }

        Instance = this;

        _playerControls = new();
        Controls = _playerControls.Controls;

        VariableSetUp();
        ControlSetUp();
    }

    void VariableSetUp()
    {
        if (povCam == null) Debug.LogError("Pov Camera not set in Inspector");
        if (dockCam == null) Debug.LogError("Dock Camera not set in Inspector");
        if (lakeCam == null) Debug.LogError("Lake Camera not set in Inspector");
        if (pauseMenu == null) Debug.LogError("Pause Menu not set in Inspector");

        if (!TryGetComponent<LaserMinigame>(out _laserMinigame)) Debug.LogError("No Laser Minigame component on GameManager");
    }

    void ControlSetUp()
    {
        Controls.Pause.performed += pauseMenu.Pause;
        Controls.Confirm.performed += TempMethod;
    }

    void Start()
    {
        ChangeState(GameState.Scan);
    }

    private void Update()
    {
        if (pauseMenu.IsPaused) return;
    }

    void TempMethod(InputAction.CallbackContext context)
    {
        temp = true;
    }

    private void ChangeState(GameState newState)
    {
        _state = newState;

        switch (_state)
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
        _laserMinigame.enabled = true;

        while (!temp)
        {
            yield return null;
        }
        temp = false;

        _laserMinigame.enabled = false;

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

    private void OnEnable()
    {
        Controls.Enable();
    }

    private void OnDisable()
    {
        Controls.Disable();
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