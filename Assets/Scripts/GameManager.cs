using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameState _state;

    [Header("Minigames")]
    [SerializeField] private TuggleMinigameManager _tuggleMinigameManager;

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

    [Header("Events")]
    [SerializeField] private VoidEventChannelSO _onScanSucceed;
    [SerializeField] private VoidEventChannelSO _onCastSucceed;
    [SerializeField] private VoidEventChannelSO _onTuggleSucceed;
    [SerializeField] private VoidEventChannelSO _onReelSucceed;
    [Space(5)]
    [SerializeField] private VoidEventChannelSO _onDamageTaken;

    private PlayerControls _playerControls;
    private bool temp = false;
    private Coroutine _controllerShakeCoroutine;
    private Coroutine _controllerFlashCoroutine;

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
    }

    void VariableSetUp()
    {
        if (_povCam == null) Debug.LogError("Pov Camera not set in Inspector");
        if (_dockCam == null) Debug.LogError("Dock Camera not set in Inspector");
        if (_lakeCam == null) Debug.LogError("Lake Camera not set in Inspector");
        if (_pauseMenu == null) Debug.LogError("Pause Menu not set in Inspector");

        if (_tuggleMinigameManager == null) Debug.LogError("No Tuggle Minigame set in Inspector");
    }

    private void OnEnable()
    {
        Controls.Enable();

        Controls.Pause.performed += PauseGame;
        Controls.Confirm.performed += TempMethod;

        _onDamageTaken.OnEventRaised += TakeDamage;
    }

    private void OnDisable()
    {
        Controls.Disable();

        Controls.Pause.performed -= PauseGame;
        Controls.Confirm.performed -= TempMethod;

        _onDamageTaken.OnEventRaised -= TakeDamage;
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
        if (IsPaused) return;
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
        _onScanSucceed.RaiseEvent();
        ChangeState(GameState.Cast);
        yield break;
    }

    private IEnumerator CastCoroutine()
    {
        ActivateCamera(_dockCam);
        while (IsBlendingBetweenCams) yield return null;

        while (!temp) yield return null;

        temp = false;
        _onCastSucceed.RaiseEvent();
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
        _onTuggleSucceed.RaiseEvent();
        ChangeState(GameState.Reel);
        yield break;
    }

    private IEnumerator ReelCoroutine()
    {
        ActivateCamera(_dockCam);

        yield break;
    }

    #endregion

    #region Camera Methods

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

    public void ShakeCamera(float intensity = 0.5f)
    {
        if (TryGetComponent<CinemachineImpulseSource>(out CinemachineImpulseSource impulseSource))
        {
            impulseSource.GenerateImpulseWithForce(intensity);
        }
    }

    #endregion

    #region Controller Methods

    public void ShakeController(float low = 0.5f, float high = 0.5f, float timeTillStop = 0.1f)
    {
        if (_controllerShakeCoroutine != null) StopCoroutine(_controllerShakeCoroutine);

        _controllerShakeCoroutine = StartCoroutine(ShakingController(low, high, timeTillStop));

    }

    private IEnumerator ShakingController(float low, float high, float timeTillStop)
    {
        Gamepad.current.SetMotorSpeeds(low, high);

        float initLow = low;
        float initHigh = high;

        float shakeTimer = timeTillStop;
        while (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            low = initLow * (shakeTimer / timeTillStop);
            high = initHigh * (shakeTimer / timeTillStop);

            Gamepad.current.SetMotorSpeeds(low, high);

            yield return null;
        }

        Gamepad.current.SetMotorSpeeds(0, 0);


        yield break;
    }

    public void FlashControllerColor(Color flashColor, float flashTime = 0.5f)
    {
        if (_controllerFlashCoroutine != null) StopCoroutine(_controllerFlashCoroutine);

        _controllerFlashCoroutine = StartCoroutine(FlashingControllerColor(flashColor, flashTime));
    }

    private IEnumerator FlashingControllerColor(Color flashColor, float flashTime)
    {
        if (Gamepad.current is not DualShockGamepad psController) yield break;

        Color initColor = flashColor;
        psController.SetLightBarColor(initColor);

        float elapsedTime = 0f;

        while (elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;
            flashColor = Color.Lerp(initColor, Color.clear, elapsedTime / flashTime);
            psController.SetLightBarColor(flashColor);
            yield return null;
        }

        yield break;
    }

    #endregion

    #region Miscellaneous Methods

    public void TakeDamage()
    {
        ShakeCamera();
        if (_options.ControlRumble) ShakeController();
        FlashControllerColor(Color.red);
    }

    private void PauseGame(InputAction.CallbackContext _)
    {
        //if (IsPaused) Controls.Enable();
        //else Controls.Disable();
        _pauseMenu.Pause();
    }

    #endregion
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