using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameState _state;

    [Header("Cameras")]
    [SerializeField] private CinemachineBrain _cinemachineBrain;
    [Space(5)]
    [SerializeField] private CinemachineVirtualCamera _povCam;
    [SerializeField] private CinemachineVirtualCamera _dockCam;
    [SerializeField] private CinemachineVirtualCamera _lakeCam;

    [Header("Menus")]
    [SerializeField] private PauseMenu _pauseMenu;

    [Header("Options")]
    [SerializeField] private OptionsSO _options;

    private PlayerControls _playerControls;
    private TuggleMinigameManager _tuggleMinigameManager;
    private bool temp = false;


    private CinemachineVirtualCamera ActiveCam => GetActiveCamera();
    private bool IsBlendingBetweenCams => _cinemachineBrain.IsBlending;
    public bool IsPaused => _pauseMenu.IsPaused;    
    public OptionsSO Options => _options;
    public PlayerControls.GameplayControlsActions Controls { get; private set; }


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
        if (_povCam == null) Debug.LogError("Pov Camera not set in Inspector");
        if (_dockCam == null) Debug.LogError("Dock Camera not set in Inspector");
        if (_lakeCam == null) Debug.LogError("Lake Camera not set in Inspector");
        if (_pauseMenu == null) Debug.LogError("Pause Menu not set in Inspector");

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
        InvokeRepeating(nameof(Test), 0, 2f);
    }

    void Test()
    {
        //Gamepad.current.SetMotorSpeeds(0.1f, 0.1f);
        //ShakeCamera(1);
    }

    private void Update()
    {
        if (_pauseMenu.IsPaused) return;
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
        ActivateCamera(_lakeCam);
        while (IsBlendingBetweenCams) yield return null;

        while (!temp) yield return null;

        temp = false;
        ChangeState(GameState.Cast);
        yield break;
    }

    private IEnumerator CastCoroutine()
    {
        ActivateCamera(_dockCam);
        while (IsBlendingBetweenCams) yield return null;

        while (!temp) yield return null;

        temp = false;
        ChangeState(GameState.Tuggle);
        yield break;
    }

    private IEnumerator TuggleCoroutine()
    {
        ActivateCamera(_povCam);
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
        ActivateCamera(_dockCam);

        yield break;
    }

    #endregion

    public void TakeDamage()
    {
        Utilities.Instance.ShakeCamera();
        if (_options.ControlRumble) Utilities.Instance.ShakeController();
        Utilities.Instance.FlashControllerColor(Color.red);
    }

    private void ActivateCamera(CinemachineVirtualCamera newCam)
    {
        _povCam.enabled = (_povCam == newCam);
        _dockCam.enabled = (_dockCam == newCam);
        _lakeCam.enabled = (_lakeCam == newCam);
    }

    private CinemachineVirtualCamera GetActiveCamera()
    {
        if (_povCam.enabled) return _povCam;
        else if (_lakeCam.enabled) return _lakeCam;
        else return _dockCam;
    }

    private void PauseGame(InputAction.CallbackContext _)
    {
        //if (IsPaused) Controls.Enable();
        //else Controls.Disable();
        _pauseMenu.Pause();
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