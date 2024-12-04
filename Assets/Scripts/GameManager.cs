using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private GameState _state;

    [Header("Cameras")]
    [SerializeField] private CinemachineBrain _cinemachineBrain;
    [Space(5)]
    [SerializeField] private CinemachineVirtualCamera _povCam;
    [SerializeField] private CinemachineVirtualCamera _castCam;
    [SerializeField] private CinemachineVirtualCamera _reelCam;
    [SerializeField] private CinemachineVirtualCamera _lakeCam;

    [Header("ScriptableObjects")]
    [SerializeField] private OptionsSO _options;
    [SerializeField] private InputReaderSO _inputReader;

    [Header("Broadcast Events")]
    [SerializeField] private VoidEventChannelSO _gameStart;
    [SerializeField] private VoidEventChannelSO _gamePaused;
    [SerializeField] private VoidEventChannelSO _gameReset;
    [SerializeField] private VoidEventChannelSO _gameWon;
    [Space(10)]
    [SerializeField] private VoidEventChannelSO _scanStart;
    [SerializeField] private VoidEventChannelSO _castStart;
    [SerializeField] private VoidEventChannelSO _tuggleStart;
    [SerializeField] private VoidEventChannelSO _reelStart;
    [SerializeField] private VoidEventChannelSO _resultsShow;
    [SerializeField] private FloatEventChannelSO _timerStopped;
    [Space(10)]
    [SerializeField] private VoidEventChannelSO _scanTutorialStartEvent;
    [SerializeField] private VoidEventChannelSO _castTutorialStartEvent;
    [SerializeField] private VoidEventChannelSO _laserTutorialStartEvent;
    [SerializeField] private VoidEventChannelSO _fishingRodTutorialStartEvent;
    [SerializeField] private VoidEventChannelSO _stunTutorialStartEvent;
    [SerializeField] private VoidEventChannelSO _reelTutorialStartEvent;
    [Space(10)]
    [SerializeField] private VoidEventChannelSO _starGained;

    [Header("Listen Events")]
    [SerializeField] private VoidEventChannelSO _damageTaken;
    [SerializeField] private VoidEventChannelSO _scanSucceed;
    [SerializeField] private VoidEventChannelSO _castSucceed;
    [SerializeField] private VoidEventChannelSO _tuggleSucceed;
    [SerializeField] private VoidEventChannelSO _endCutsceneFinished;
    [SerializeField] private VoidEventChannelSO _gameLost;
    [Space(5)]
    [SerializeField] private VoidEventChannelSO _scanTutorialSucceedEvent;
    [SerializeField] private VoidEventChannelSO _castTutorialSucceedEvent;
    [SerializeField] private VoidEventChannelSO _laserTutorialSucceedEvent;
    [SerializeField] private VoidEventChannelSO _fishingRodTutorialSucceedEvent;
    [SerializeField] private VoidEventChannelSO _stunTutorialSucceedEvent;
    [SerializeField] private VoidEventChannelSO _reelTutorialSucceedEvent;

    //private bool temp = false;
    private bool gameRunning = false;
    private bool tutorialShowing = false;
    private float timer;
    private Coroutine _controllerShakeCoroutine;
    private Coroutine _controllerFlashCoroutine;

    private PlayerControls.GameplayControlsActions Controls => _inputReader.Controls;
    private CinemachineVirtualCamera ActiveCam => GetActiveCamera();
    private bool IsBlendingBetweenCams => _cinemachineBrain.IsBlending;
    public OptionsSO Options => _options;

     

    private void Awake()
    {
        //DontDestroyOnLoad(this);

        VariableSetUp();
    }
    
    void VariableSetUp()
    {
        if (_povCam == null) Debug.LogError("Pov Camera not set in Inspector");
        if (_castCam == null) Debug.LogError("Cast Camera not set in Inspector");
        if (_reelCam == null) Debug.LogError("Reel Camera not set in Inspector");
        if (_lakeCam == null) Debug.LogError("Lake Camera not set in Inspector");
    }

    private void OnEnable()
    {
        Controls.Pause.performed += PauseGame;
        //Controls.Select.performed += TempMethod;

        _gameReset.OnEventRaised += DestroyGameManager;
        _gameLost.OnEventRaised += GameLost;

        _damageTaken.OnEventRaised += TakeDamage;

        _scanSucceed.OnEventRaised += ActivateCastPhase;
        _castSucceed.OnEventRaised += ActivateTugglePhase;
        _tuggleSucceed.OnEventRaised += ActivateReelPhase;
        _endCutsceneFinished.OnEventRaised += ActivateResultsPhase;

        _scanTutorialSucceedEvent.OnEventRaised += StopTutorial;
        _castTutorialSucceedEvent.OnEventRaised += StopTutorial;
        _laserTutorialSucceedEvent.OnEventRaised += StopTutorial;
        _fishingRodTutorialSucceedEvent.OnEventRaised += StopTutorial;
        _stunTutorialSucceedEvent.OnEventRaised += StopTutorial;
        _reelTutorialSucceedEvent.OnEventRaised += StopTutorial;
    }

    private void OnDisable()
    {
        Controls.Pause.performed -= PauseGame;
        //Controls.Select.performed -= TempMethod;

        _gameReset.OnEventRaised -= DestroyGameManager;
        _gameLost.OnEventRaised -= GameLost;

        _damageTaken.OnEventRaised -= TakeDamage;

        _scanSucceed.OnEventRaised -= ActivateCastPhase;
        _castSucceed.OnEventRaised -= ActivateTugglePhase;
        _tuggleSucceed.OnEventRaised -= ActivateReelPhase;
        _endCutsceneFinished.OnEventRaised -= ActivateResultsPhase;

        _scanTutorialSucceedEvent.OnEventRaised -= StopTutorial;
        _castTutorialSucceedEvent.OnEventRaised -= StopTutorial;
        _laserTutorialSucceedEvent.OnEventRaised -= StopTutorial;
        _fishingRodTutorialSucceedEvent.OnEventRaised -= StopTutorial;
        _stunTutorialSucceedEvent.OnEventRaised -= StopTutorial;
        _reelTutorialSucceedEvent.OnEventRaised -= StopTutorial;
    }


    void Start()
    {
        _gameStart.RaiseEvent();
        gameRunning = true;

        if (Options.Difficulty == Difficulty.Hard) _starGained.RaiseEvent();

        ChangeState(GameState.Scan);
        InvokeRepeating(nameof(Test), 0, 2f);
    }

    private void Update()
    {
        if (gameRunning) timer += Time.deltaTime;
    }

    void Test()
    {
        //Gamepad.current.SetMotorSpeeds(0.1f, 0.1f);
        //ShakeCamera(1);
    }

    //void TempMethod(InputAction.CallbackContext context)
    //{
    //    if (!IsBlendingBetweenCams) temp = true;
    //}

    #region StateChangeMethods

    private void ActivateScanPhase() => ChangeState(GameState.Scan);

    private void ActivateCastPhase() => ChangeState(GameState.Cast);

    private void ActivateTugglePhase() => ChangeState(GameState.Tuggle);

    private void ActivateReelPhase() => ChangeState(GameState.Reel);

    private void ActivateResultsPhase() => ChangeState(GameState.Results);

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
                StartCoroutine(ResultsCoroutine());
                break;
        }
    }

    #endregion

    #region Gameplay Methods

    private IEnumerator ScanCoroutine()
    {
        ActivateCamera(_lakeCam);
        while (IsBlendingBetweenCams) yield return null;

        if (Options.GuideImages)
        {
            tutorialShowing = true;
            _scanTutorialStartEvent.RaiseEvent();
            while (tutorialShowing) yield return null;
            yield return 0;
        }
        
        _scanStart.RaiseEvent();
        yield return 0;

        //while (!temp) yield return null;

        //temp = false;
        //_scanSucceed.RaiseEvent();
        //ChangeState(GameState.Cast);
        yield break;
    }

    private IEnumerator CastCoroutine()
    {
        ActivateCamera(_castCam);
        while (IsBlendingBetweenCams) yield return null;

        if (Options.GuideImages)
        {
            tutorialShowing = true;
            _castTutorialStartEvent.RaiseEvent();
            while (tutorialShowing) yield return null;
        }

        _castStart.RaiseEvent();
        yield return 0;

        //while (!temp) yield return null;

        //temp = false;
        //_castSucceed.RaiseEvent();
        //ChangeState(GameState.Tuggle);
        yield break;
    }

    private IEnumerator TuggleCoroutine()
    {
        ActivateCamera(_povCam);
        while (IsBlendingBetweenCams) yield return null;

        if (Options.GuideImages)
        {
            tutorialShowing = true;
            _laserTutorialStartEvent.RaiseEvent();
            while (tutorialShowing) yield return null;
            yield return 0;

            tutorialShowing = true;
            _fishingRodTutorialStartEvent.RaiseEvent();
            while (tutorialShowing) yield return null;
            yield return 0;

            tutorialShowing = true;
            _stunTutorialStartEvent.RaiseEvent();
            while (tutorialShowing) yield return null;
        }

        _tuggleStart.RaiseEvent();
        yield return 0;

        //while (!temp) yield return null;

        //temp = false;
        //_tuggleSucceed.RaiseEvent();
        //ChangeState(GameState.Reel);
        yield break;
    }

    private IEnumerator ReelCoroutine()
    {
        ActivateCamera(_reelCam);
        while (IsBlendingBetweenCams) yield return null;

        if (Options.GuideImages)
        {
            tutorialShowing = true;
            _reelTutorialStartEvent.RaiseEvent();
            while (tutorialShowing) yield return null;
        }

        _reelStart.RaiseEvent();
        yield return 0;

        //while (!temp) yield return null;

        //temp = false;
        //_reelSucceed.RaiseEvent();
        //ChangeState(GameState.Results);

        yield break;
    }

    private IEnumerator EndCutsceneCoroutine()
    {
        //ActivateCamera(_dockCam);
        //while (IsBlendingBetweenCams) yield return null;

        //yield return new WaitForSeconds(2);

        gameRunning = false;
        _resultsShow.RaiseEvent();
        _timerStopped.RaiseEvent(timer);

        yield break;
    }

    private IEnumerator ResultsCoroutine()
    {
        //ActivateCamera(_dockCam);
        //while (IsBlendingBetweenCams) yield return null;

        //yield return new WaitForSeconds(2);

        gameRunning = false;
        _resultsShow.RaiseEvent();
        _timerStopped.RaiseEvent(timer);

        yield break;
    }

    #endregion

    #region Camera Methods

    private void ActivateCamera(CinemachineVirtualCamera newCam)
    {
        _povCam.enabled = (_povCam == newCam);
        _castCam.enabled = (_castCam == newCam);
        _reelCam.enabled = (_reelCam == newCam);
        _lakeCam.enabled = (_lakeCam == newCam);
    }

    private CinemachineVirtualCamera GetActiveCamera()
    {
        if (_povCam.enabled) return _povCam;
        else if (_lakeCam.enabled) return _lakeCam;
        else if (_castCam.enabled) return _castCam;
        else return _reelCam;
    }

    #endregion

    #region Controller Methods

    public void ShakeController(float low = 0.4f, float high = 0.4f, float timeTillStop = 0.05f)
    {
        if (!Options.ControllerConnected) return; 

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
        Gamepad.current.ResetHaptics();


        yield break;
    }

    public void FlashControllerColor(Color flashColor, float flashTime = 0.5f)
    {
        if (!Options.ControllerConnected) return;

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
        if (_options.ControlRumble) ShakeController();
        FlashControllerColor(Color.red);
    }

    private void StopTutorial()
    {
        tutorialShowing = false;
    }

    private void PauseGame(InputAction.CallbackContext _)
    {
        _gamePaused.RaiseEvent();
    }

    private void DestroyGameManager()
    {
        gameRunning = false;
        Destroy(this.gameObject);
    }

    private void GameLost()
    {
        SceneManager.LoadScene("LossScene");
        _gameReset.RaiseEvent();
    }

    #endregion

    private void OnApplicationQuit()
    {
        _gameReset.RaiseEvent();
    }
}

public enum GameState
{
    Menu,
    Scan,
    Cast,
    Tuggle,
    Reel,
    EndCutscene,
    Results
}