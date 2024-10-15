using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ReelMinigame : MinigameBase
    
{
    //grabs the image and slider UI elements for button to press, and the reel meter
    public UnityEngine.UI.Image buttonFrame;
    public UnityEngine.UI.Slider reelMeter;
    private Vector3 buttonPosition, shakePosition;

    //holds image sprites for which button to mash
    //***SET ORDER OF SPRITES IN UP, DOWN LEFT, RIGHT FOR PROPER FUNCTION***
    public Sprite[] buttonSpriteList = new Sprite[4];
    private Sprite buttonSprite;

    //quantity of how much meter changes from button press, or from decreasing overtime
    public float reelStrength, meterDecay;

    //values for button shake intensity
    private float shakeValue = 20.0f;
    private float shakeSpeed = 30.0f;
    private bool shakeActive = false;

    //keeps track of how many times the button changes
    private int MeterPhase = 0;

    //enabling and disabling reeling controls
    protected override void OnEnable() 
    { 
        base.OnEnable();
        Controls.Reeling.performed += ReelCheck;
    }

    private void OnDisable()
    {
        Controls.Reeling.performed -= ReelCheck;
    }

    // Start is called before the first frame update
    void Start()
    {
        //sets random sprite for button ui
        buttonSprite = buttonSpriteList[Random.Range(0, buttonSpriteList.Length)];
        buttonFrame.sprite = buttonSprite;
        buttonPosition = buttonFrame.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //meter constantly decreases until it becomes full
        if (reelMeter.value < 1) {
            reelMeter.value -= meterDecay;
        }
        
        PhaseChange();

        //allows the button to shake when set to true
        if (shakeActive) buttonShake();
    }

    //funtion to have button erratically shake in the update
    void buttonShake()
    {
        shakePosition = new Vector3(Random.Range(-shakeValue, shakeValue), Random.Range(-shakeValue, shakeValue), 0f);
        shakePosition *= shakeSpeed * Time.deltaTime;

        buttonFrame.transform.position += shakePosition;
    }

    //timer to disable shaking
    IEnumerator shakeDuration()
    {
        shakeActive = true;
        yield return new WaitForSeconds(.3f);
        shakeActive = false;
        //resets button frame to original position
        buttonFrame.transform.position = buttonPosition;
    }

    //changes required button when player reaches certain thresholds of reel meter
    void PhaseChange()
    {
        //button changes once meter reacher a third of the amount
        if (reelMeter.value >= .33 && MeterPhase < 1)
        {
            MeterPhase++;
            do
                buttonSprite = buttonSpriteList[Random.Range(0, buttonSpriteList.Length)];
            while (buttonFrame.sprite == buttonSprite);
            buttonFrame.sprite = buttonSprite;
        }

        //button changes one last time once it reaches two-thirds the amount
        if (reelMeter.value >= .66 && MeterPhase < 2)
        {
            MeterPhase++;
            do
                buttonSprite = buttonSpriteList[Random.Range(0, buttonSpriteList.Length)];
            while (buttonFrame.sprite == buttonSprite);
            buttonFrame.sprite = buttonSprite;
        }
        //calls success event once the meter becomes full
        if (MeterPhase == 2 && reelMeter.value == reelMeter.maxValue)
        {
            MeterPhase++;
            _minigameSuccess.RaiseEvent();
            Debug.Log("reel game succeeded!");
        }
    }

    //input check for button
    void ReelCheck(InputAction.CallbackContext context)
    {
        var buttonValue = context.ReadValue<Vector2>();
        //When player presses an input, a check is conducted for the right or wrong input and changes meter accordingly
        //correct button input will enable the button shake state
        if (buttonValue == Vector2.up)
        {
            if (buttonFrame.sprite == buttonSpriteList[0])
            {
                reelMeter.value += reelStrength;
                StartCoroutine(shakeDuration());
            }
            else
            {
                reelMeter.value -= reelStrength;
            }
        }
        if (buttonValue == Vector2.down)
        {
            if (buttonFrame.sprite == buttonSpriteList[1])
            {
                reelMeter.value += reelStrength;
                StartCoroutine(shakeDuration());
            }
            else
            {
                reelMeter.value -= reelStrength;
            }
        }
        if (buttonValue == Vector2.left)
        {
            if (buttonFrame.sprite == buttonSpriteList[2])
            {
                reelMeter.value += reelStrength;
                StartCoroutine(shakeDuration());
            }
            else
            {
                reelMeter.value -= reelStrength;
            }
        }
        if (buttonValue == Vector2.right)
        {
            if (buttonFrame.sprite == buttonSpriteList[3])
            {
                reelMeter.value += reelStrength;
                StartCoroutine(shakeDuration());
            }
            else
            {
                reelMeter.value -= reelStrength;
            }
        }
    }


}
