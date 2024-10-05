using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Broadcast Events")]
    [SerializeField] private VoidEventChannelSO damagePlayerSO;

    private float activeTimer;
    private float driftTimer;
    private float minigameFailTimer;
    private float minigameHoldTimer;
    private bool isDrifting;
    private Direction driftDirection;

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

    private bool HoldingButton => minigameHoldTimer > 0;


    protected override void OnEnable()
    {
        base.OnEnable();

        activeTimer = activeTime;
        driftTimer = Random.Range(MinDriftTime, MaxDriftTime);

        //Controls.FishingRodControl.performed += JoltRod;
        Controls.FishingRodControl.started += FishingRodControl_started;
        Controls.FishingRodControl.canceled += FishingRodControl_canceled;

    }

    private void FishingRodControl_canceled(InputAction.CallbackContext obj)
    {
        minigameHoldTimer = -1;
    }

    private void FishingRodControl_started(InputAction.CallbackContext obj)
    {
        minigameHoldTimer = minigameHoldTime;
    }

    private void OnDisable()
    {
        //Controls.FishingRodControl.performed -= JoltRod;
        Controls.FishingRodControl.started -= FishingRodControl_started;
        Controls.FishingRodControl.canceled -= FishingRodControl_canceled;
    }

    private void Update()
    {
        if (isPaused) return;

        if (!isDrifting && activeTimer <= 0)
        {
            _minigameSuccess.RaiseEvent();
            this.enabled = false;
        }

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
                Direction dir = Random.Range(0f,1f) == 0 ? Direction.Left : Direction.Right;
                DriftRod(dir);
            }
        }

        if (HoldingButton)
        {
            minigameHoldTimer -= Time.deltaTime;

            if (minigameHoldTimer <= 0)
            {
                JoltRod();
            }
        }

        if (minigameFailTimer > 0)
        {
            minigameFailTimer -= Time.deltaTime;

            if (minigameFailTimer <= 0)
            {
                damagePlayerSO.RaiseEvent();
                isDrifting = false;
                fishingRodAnim.SetTrigger("DriftStop");
            }
        }
    }

    private void DriftRod(Direction direction)
    {
        if (isDrifting) return;
        isDrifting = true;
        minigameFailTimer = MinigameFailTime;

        driftDirection = direction;
        if (direction == Direction.Left) fishingRodAnim.SetTrigger("DriftLeft");
        else fishingRodAnim.SetTrigger("DriftRight");

        
    }

    private void JoltRod(InputAction.CallbackContext context)
    {
        if (!isDrifting) return;

        float playerDirection = context.ReadValue<float>();
        if (driftDirection == Direction.Left && playerDirection < 0)
        {
            if (damagePlayerWhenFail)
            {
                damagePlayerSO.RaiseEvent();
                fishingRodAnim.SetTrigger("DriftStop");
            }
            return;
        }
        else if (driftDirection == Direction.Right && playerDirection > 0)
        {
            if (damagePlayerWhenFail) 
            { 
                damagePlayerSO.RaiseEvent();
                fishingRodAnim.SetTrigger("DriftStop");
            }
            return;
        }

        isDrifting = false;
        minigameFailTimer = 0;
        driftTimer = Random.Range(MinDriftTime, MaxDriftTime);

        fishingRodAnim.SetTrigger("DriftStop");
    }

    private void JoltRod()
    {
        if (!isDrifting) return;

        float playerDirection = Controls.FishingRodControl.ReadValue<float>();
        if (driftDirection == Direction.Left && playerDirection < 0)
        {
            if (damagePlayerWhenFail)
            {
                damagePlayerSO.RaiseEvent();
                fishingRodAnim.SetTrigger("DriftStop");
            }
            return;
        }
        else if (driftDirection == Direction.Right && playerDirection > 0)
        {
            if (damagePlayerWhenFail)
            {
                damagePlayerSO.RaiseEvent();
                fishingRodAnim.SetTrigger("DriftStop");
            }
            return;
        }

        isDrifting = false;
        minigameFailTimer = 0;
        driftTimer = Random.Range(MinDriftTime, MaxDriftTime);

        fishingRodAnim.SetTrigger("DriftStop");
    }
}