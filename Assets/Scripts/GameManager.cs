using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameState _state;

    [SerializeField] private bool DebugMode = false;

    [Header("Cameras")]
    [SerializeField] private CinemachineBrain cinemachineBrain;
    [Space(5)]
    [SerializeField] private CinemachineVirtualCamera povCam;
    [SerializeField] private CinemachineVirtualCamera dockCam;
    [SerializeField] private CinemachineVirtualCamera lakeCam;
    private bool IsBlendingBetweenCams
    {
        get { return cinemachineBrain.IsBlending; }
    }

    [Space(20)]
    [SerializeField] private PauseMenu pauseMenu;
    public bool IsPaused
    {
        get { return pauseMenu.IsPaused; }
    }

    private bool temp = false;

    private TuggleMinigameManager _tuggleMinigameManager;

    private PlayerControls _playerControls;
    public PlayerControls.GameplayControlsActions Controls
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
        Controls = _playerControls.GameplayControls;

        VariableSetUp();
        ControlSetUp();
    }

    void VariableSetUp()
    {
        if (povCam == null) Debug.LogError("Pov Camera not set in Inspector");
        if (dockCam == null) Debug.LogError("Dock Camera not set in Inspector");
        if (lakeCam == null) Debug.LogError("Lake Camera not set in Inspector");
        if (pauseMenu == null) Debug.LogError("Pause Menu not set in Inspector");

        if (!TryGetComponent<TuggleMinigameManager>(out _tuggleMinigameManager)) Debug.LogError("No Laser Minigame component on GameManager");
    }

    void ControlSetUp()
    {
        Controls.Pause.performed += PauseGame;
        Controls.Confirm.performed += TempMethod;
    }

    void Start()
    {
        ChangeState(GameState.Scan);
        //InvokeRepeating(nameof(Vibrate), 0, 2f);
    }

    //void Vibrate()
    //{
    //    Gamepad.current.SetMotorSpeeds(0.1f, 0.1f);
    //}

    private void Update()
    {
        if (pauseMenu.IsPaused) return;
    }

    void TempMethod(InputAction.CallbackContext context)
    {
        if (!IsBlendingBetweenCams) temp = true;
    }

    private void ChangeState(GameState newState)
    {
        _state = newState;

        StopAllCoroutines();

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
        while (IsBlendingBetweenCams) yield return null;

        while (!temp) yield return null;

        temp = false;
        ChangeState(GameState.Cast);
        yield break;
    }

    private IEnumerator CastCoroutine()
    {
        ActivateCamera(dockCam);
        while (IsBlendingBetweenCams) yield return null;

        while (!temp) yield return null;

        temp = false;
        ChangeState(GameState.Tuggle);
        yield break;
    }

    private IEnumerator TuggleCoroutine()
    {
        ActivateCamera(povCam);
        while (IsBlendingBetweenCams) yield return null;

        _tuggleMinigameManager.enabled = true;

        while (!temp) yield return null;

        temp = false;
        _tuggleMinigameManager.enabled = false;
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

    private void PauseGame(InputAction.CallbackContext _)
    {
        //if (IsPaused) Controls.Enable();
        //else Controls.Disable();
        pauseMenu.Pause();
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