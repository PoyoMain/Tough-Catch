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

    [Header("UI")]
    [SerializeField] private Slider leftArrowPromptSlider;
    [SerializeField] private Slider rightArrowPromptSlider;
    [SerializeField] private Animator leftArrowPromptAnim;
    [SerializeField] private Animator rightArrowPromptAnim;

    [Header("Broadcast Events")]
    [SerializeField] private VoidEventChannelSO damagePlayerSO;
    [SerializeField] private VoidEventChannelSO fishingRodStartDriftSO;
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
    private float MinigameFailTime
    {
        get
        {
            if (_useOptionValues) return Options.FishingRodMinigameOptions.minigameFailTime;
            else return minigameFailTime;
        }
    }



    protected override void OnEnable()
    {
        base.OnEnable();

        activeTimer = activeTime;
        driftTimer = Random.Range(MinDriftTime, MaxDriftTime);

        //Controls.FishingRodControl.performed += JoltRod;
        //Controls.FishingRodControl.started += FishingRodControl_started;
        //Controls.FishingRodControl.canceled += FishingRodControl_canceled;

    }

    private void FishingRodControl_canceled()
    {
        holdingButton = false;
        minigameHoldTimer = -1;
    }

    private void FishingRodControl_started()
    {
        holdingButton = true;
        minigameHoldTimer = minigameHoldTime;

        float playerDirection = Controls.FishingRodControl.ReadValue<float>();

        activePromptSlider = playerDirection < 0 ? leftArrowPromptSlider : rightArrowPromptSlider;
        activePromptAnim = playerDirection < 0 ? leftArrowPromptAnim : rightArrowPromptAnim;
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
                leftArrowPromptSlider.gameObject.SetActive(false);
                rightArrowPromptSlider.gameObject.SetActive(false);
                damagePlayerSO.RaiseEvent();
                isDrifting = false;
                fishingRodStopDriftSO.RaiseEvent();
                fishingRodAnim.SetTrigger("DriftStop");
                activePromptAnim.SetTrigger("Fail");
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

        fishingRodStartDriftSO.RaiseEvent();
    }

    private void JoltRod()
    {
        if (!isDrifting) return;

        leftArrowPromptSlider.gameObject.SetActive(false);
        rightArrowPromptSlider.gameObject.SetActive(false);

        float playerDirection = Controls.FishingRodControl.ReadValue<float>();
        if (driftDirection == Direction.Left && playerDirection < 0)
        {
            if (damagePlayerWhenFail) damagePlayerSO.RaiseEvent();
            fishingRodAnim.SetTrigger("DriftStop");
            fishingRodStopDriftSO.RaiseEvent();

            return;
        }
        else if (driftDirection == Direction.Right && playerDirection > 0)
        {
            if (damagePlayerWhenFail) damagePlayerSO.RaiseEvent();
            fishingRodAnim.SetTrigger("DriftStop");
            fishingRodStopDriftSO.RaiseEvent();
            return;
        }

        isDrifting = false;
        minigameFailTimer = 0;
        driftTimer = Random.Range(MinDriftTime, MaxDriftTime);
        activePromptSlider.value = 0;

        fishingRodStopDriftSO.RaiseEvent();
        fishingRodAnim.SetTrigger("DriftStop");
    }
}