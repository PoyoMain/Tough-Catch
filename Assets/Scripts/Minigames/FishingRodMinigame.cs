using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FishingRodMinigame : MinigameBase
{
    [Header("Settings")]
    [SerializeField] private float activeTime = 20f;
    [SerializeField] private float minDriftTime = 5f;
    [SerializeField] private float maxDriftTime = 15f;
    [SerializeField] private float minigameFailTime = 3f;
    [SerializeField] private float minigameHoldTime = 2f;
    [SerializeField] private bool damagePlayerWhenFail;

    [Header("Fishing Rod")]
    [SerializeField] private Animator fishingRodAnim;

    [Header("Flames")]
    [SerializeField] private GameObject leftFlames;
    [SerializeField] private GameObject rightFlames;

    [Header("UI")]
    [SerializeField] private Slider leftArrowPromptSlider;
    [SerializeField] private Slider rightArrowPromptSlider;
    [SerializeField] private Animator leftArrowPromptAnim;
    [SerializeField] private Animator rightArrowPromptAnim;
    [SerializeField] private Image _leftArrowButtonBG;
    [SerializeField] private Image _leftArrowButtonFG;
    [SerializeField] private Image _leftArrowButtonFail;
    [SerializeField] private Image _rightArrowButtonBG;
    [SerializeField] private Image _rightArrowButtonFG;
    [SerializeField] private Image _rightArrowButtonFail;

    [Header("Keyboard Sprites")]
    [SerializeField] private Sprite leftArrowButtonSelected_Keyboard;
    [SerializeField] private Sprite leftArrowButtonUnselected_Keyboard;
    [SerializeField] private Sprite leftArrowButtonFailed_Keyboard;
    [SerializeField] private Sprite rightArrowButtonSelected_Keyboard;
    [SerializeField] private Sprite rightArrowButtonUnselected_Keyboard;
    [SerializeField] private Sprite rightArrowButtonFailed_Keyboard;
    [Header("Controller Sprites")]
    [SerializeField] private Sprite leftArrowButtonSelected_Controller;
    [SerializeField] private Sprite leftArrowButtonUnselected_Controller;
    [SerializeField] private Sprite leftArrowButtonFailed_Controller;
    [SerializeField] private Sprite rightArrowButtonSelected_Controller;
    [SerializeField] private Sprite rightArrowButtonUnselected_Controller;
    [SerializeField] private Sprite rightArrowButtonFailed_Controller;

    [Header("Broadcast Events")]
    [SerializeField] private VoidEventChannelSO damagePlayerSO;
    [SerializeField] private VoidEventChannelSO fishingRodStartDriftLeftSO;
    [SerializeField] private VoidEventChannelSO fishingRodStopDriftSO;

    private float activeTimer;
    private float driftTimer;
    private float minigameFailTimer;
    private float minigameHoldTimer;
    private bool isDrifting;
    private bool holdingButton;
    private Direction driftDirection;
    private Slider activePromptSlider;
    private Animator activePromptAnim;

    private float MinDriftTime
    {
        get
        {
            if (_useOptionValues) return Options.FishingRodMinigameOptions.minDriftTime;
            else return minDriftTime;
        }
    }
    private float MaxDriftTime
    {
        get
        {
            if (_useOptionValues) return Options.FishingRodMinigameOptions.maxDriftTime;
            else return maxDriftTime;
        }
    }
    private float MinigameHoldTime
    {
        get
        {
            if (_useOptionValues) return Options.FishingRodMinigameOptions.minigameHoldTime;
            else return minigameHoldTime;
        }
    }
    private float MinigameFailTime
    {
        get
        {
            if (_useOptionValues) return Options.FishingRodMinigameOptions.minigameFailTime;
            else return minigameFailTime;
        }
    }

    private Sprite LeftArrowButtonUnselected
    {
        get => ControllerConnected ? leftArrowButtonUnselected_Controller : leftArrowButtonUnselected_Keyboard;
    }
    private Sprite LeftArrowButtonSelected
    {
        get => ControllerConnected ? leftArrowButtonSelected_Controller : leftArrowButtonSelected_Keyboard;
    }
    private Sprite LeftArrowButtonFailed
    {
        get => ControllerConnected ? leftArrowButtonFailed_Controller : leftArrowButtonFailed_Keyboard;
    }
    private Sprite RightArrowButtonUnselected
    {
        get => ControllerConnected ? rightArrowButtonUnselected_Controller : rightArrowButtonUnselected_Keyboard;
    }
    private Sprite RightArrowButtonSelected
    {
        get => ControllerConnected ? rightArrowButtonSelected_Controller : rightArrowButtonSelected_Keyboard;
    }
    private Sprite RightArrowButtonFailed
    {
        get => ControllerConnected ? rightArrowButtonFailed_Controller : rightArrowButtonFailed_Keyboard;
    }

    private void Start()
    {
        _leftArrowButtonBG.sprite = LeftArrowButtonUnselected;
        _leftArrowButtonFG.sprite = LeftArrowButtonSelected;
        _leftArrowButtonFail.sprite = LeftArrowButtonFailed;

        _rightArrowButtonBG.sprite = RightArrowButtonUnselected;
        _rightArrowButtonFG.sprite = RightArrowButtonSelected;
        _rightArrowButtonFail.sprite = RightArrowButtonFailed;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        activeTimer = activeTime;
        minigameHoldTimer = -1;
        driftTimer = Random.Range(MinDriftTime, MaxDriftTime);

        //Controls.FishingRodControl.performed += JoltRod;
        //Controls.FishingRodControl.started += FishingRodControl_started;
        //Controls.FishingRodControl.canceled += FishingRodControl_canceled;

    }

    private void FishingRodControl_canceled()
    {
        leftFlames.SetActive(false);
        rightFlames.SetActive(false);
        holdingButton = false;
        minigameHoldTimer = -1;
        minigameFailTimer = -1;
    }

    private void FishingRodControl_started()
    {
        Direction playerDirection = Controls.FishingRodControl.ReadValue<float>() < 0 ? Direction.Left : Direction.Right;
        GameObject activeFlames = playerDirection == Direction.Left ? leftFlames : rightFlames;
        activeFlames.SetActive(true);

        if (driftDirection != playerDirection)
        {
            holdingButton = true;
            minigameHoldTimer = MinigameHoldTime;
        }
        else
        {
            holdingButton = false;
            minigameFailTimer = MinigameFailTime;
        }
    }

    //private void OnDisable()
    //{
    //    //Controls.FishingRodControl.performed -= JoltRod;
    //    //.FishingRodControl.started -= FishingRodControl_started;
    //    //Controls.FishingRodControl.canceled -= FishingRodControl_canceled;
    //}

    private void Update()
    {
        if (isPaused) return;

        if (!isDrifting && activeTimer <= 0)
        {
            _minigameSuccess.RaiseEvent();
            this.enabled = false;
        }

        if (Controls.FishingRodControl.WasPressedThisFrame()) FishingRodControl_started();
        else if (Controls.FishingRodControl.WasReleasedThisFrame()) FishingRodControl_canceled();

        CheckTimers();
    }

    private void CheckTimers()
    {
        if (activeTimer > 0) activeTimer -= Time.deltaTime;

        if (driftTimer > 0)
        {
            driftTimer -= Time.deltaTime;

            if (driftTimer <= 0)
            {
                Direction dir = Random.Range(0,2) == 0 ? Direction.Left : Direction.Right;
                DriftRod(dir);
            }
        }

        if (holdingButton)
        {
            if (activePromptSlider == null) return;

            activePromptSlider.value += Time.deltaTime;

            if (activePromptSlider.value >= activePromptSlider.maxValue) JoltRod();
        }
        else
        {
            if (activePromptSlider == null) return;

            if (activePromptSlider.value >= 0) activePromptSlider.value -= Time.deltaTime;
        }

        if (minigameFailTimer > 0)
        {
            minigameFailTimer -= Time.deltaTime;

            if (minigameFailTimer <= 0)
            {
                isDrifting = false;
                holdingButton = false;

                leftArrowPromptSlider.gameObject.SetActive(false);
                rightArrowPromptSlider.gameObject.SetActive(false);

                damagePlayerSO.RaiseEvent();
                fishingRodStopDriftSO.RaiseEvent();

                fishingRodAnim.SetTrigger("DriftStop");
                activePromptAnim.SetTrigger("Fail");
                leftFlames.SetActive(false);
                rightFlames.SetActive(false);
            }
        }
    }

    private void DriftRod(Direction direction)
    {
        if (isDrifting) return;
        isDrifting = true;
        minigameFailTimer = MinigameFailTime;

        driftDirection = direction;
        if (direction == Direction.Left)
        {
            rightArrowPromptSlider.gameObject.SetActive(true);
            fishingRodAnim.SetTrigger("DriftLeft");
        }
        else
        {
            leftArrowPromptSlider.gameObject.SetActive(true);
            fishingRodAnim.SetTrigger("DriftRight");
        }

        activePromptSlider = direction != Direction.Left ? leftArrowPromptSlider : rightArrowPromptSlider;
        activePromptAnim = direction != Direction.Left ? leftArrowPromptAnim : rightArrowPromptAnim;

        fishingRodStartDriftLeftSO.RaiseEvent();
    }

    private void JoltRod()
    {
        if (!isDrifting) return;

        leftArrowPromptSlider.gameObject.SetActive(false);
        rightArrowPromptSlider.gameObject.SetActive(false);

        float playerDirection = Controls.FishingRodControl.ReadValue<float>();
        if (driftDirection == Direction.Left && playerDirection < 0)
        {
            minigameFailTimer = 0;
            holdingButton = false;

            if (damagePlayerWhenFail) damagePlayerSO.RaiseEvent();
            fishingRodStopDriftSO.RaiseEvent();

            fishingRodAnim.SetTrigger("DriftStop");

            return;
        }
        else if (driftDirection == Direction.Right && playerDirection > 0)
        {
            minigameFailTimer = 0;
            holdingButton = false;

            if (damagePlayerWhenFail) damagePlayerSO.RaiseEvent();
            fishingRodStopDriftSO.RaiseEvent();

            fishingRodAnim.SetTrigger("DriftStop");

            return;
        }

        isDrifting = false;
        holdingButton = false;

        minigameFailTimer = 0;
        driftTimer = Random.Range(MinDriftTime, MaxDriftTime);

        activePromptSlider.value = 0;
        activePromptSlider = null;

        fishingRodStopDriftSO.RaiseEvent();

        fishingRodAnim.SetTrigger("DriftStop");

        leftFlames.SetActive(false);
        rightFlames.SetActive(false);
    }
}