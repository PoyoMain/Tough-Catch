using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ReelMinigame : MinigameBase
    
{
    //grabs the image and slider UI elements for button to press, reel/success texts, and the reel meter
    public UnityEngine.UI.Image buttonFrame, reelText, successText;
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
    private int shakeActive = 0;

    //keeps track of how many times the button changes
    private int MeterPhase = -1;

    //prevents coroutine from being called more than once at a time
    private bool couroutineSwitch = false;

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
        Controls.Reeling.Disable();
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
        
        buttonChange();

        //allows the button to shake when set to true
        if (shakeActive > 0) buttonShake();
        //displays reel text at game start
        if (reelText.gameObject.activeSelf) displayReelText();
        //displays success text for win
        if (MeterPhase >= 3)
        {
            displaySuccessText();
        }
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
        shakeActive ++;
        yield return new WaitForSeconds(.3f);
        shakeActive --;
        //resets button frame to original position
        buttonFrame.transform.position = buttonPosition;
    }

    //changes required button when player reaches certain thresholds of reel meter
    void buttonChange()
    {
        //button changes once meter reacher a third of the amount
        if (reelMeter.value >= .33 && MeterPhase == 0)
        {
            MeterPhase++;
            do
                buttonSprite = buttonSpriteList[Random.Range(0, buttonSpriteList.Length)];
            while (buttonFrame.sprite == buttonSprite);
            buttonFrame.sprite = buttonSprite;
        }

        //button changes one last time once it reaches two-thirds the amount
        if (reelMeter.value >= .66 && MeterPhase == 1)
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
            Controls.Reeling.Disable();
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

    void displayReelText()
    {
        //fades in reel text
        if (reelText.color.a < 1)
        {
            var reelTextColor = reelText.color;
            reelTextColor.a += 0.01f;
            reelText.color = reelTextColor;
        }
        //text holds for short time
        if(couroutineSwitch == false && MeterPhase == -1) StartCoroutine(TextDuration());
        //text shifts out of view
        if (reelText.color.a >= 1 && MeterPhase > -1)
        {
            var reelTextPos = reelText.transform.position;
            reelTextPos.y += 1;
            reelText.transform.position = reelTextPos;
        }
        //disables text gameobject once out of view
        if (reelText.transform.position.y >= 700 && reelText.gameObject.activeSelf)
        {
            Controls.Reeling.Enable();
            reelText.gameObject.SetActive(false);
            Debug.Log("start game");
        }
    }

    void displaySuccessText()
    {
        //fades in success text
        if (successText.color.a < 1 && MeterPhase == 3)
        {
            var successTextColor = successText.color;
            successTextColor.a += 0.01f;
            successText.color = successTextColor;
        }
        //text holds for short time
        if(couroutineSwitch == false && MeterPhase == 3) StartCoroutine(TextDuration());
        //text fades out
        if (successText.color.a > 0 && MeterPhase == 4)
        {
            var successTextColor = successText.color;
            successTextColor.a -= 0.005f;
            successText.color = successTextColor;
        }
        //minigame ends once text disappears
        if (successText.color.a <= 0 && MeterPhase == 4)
        {
            MeterPhase++;
            _minigameSuccess.RaiseEvent();
            Debug.Log("success event raised");
        }
    }

    IEnumerator TextDuration()
    {
        couroutineSwitch = true;
        yield return new WaitForSeconds(1.5f);
        couroutineSwitch = false;
        MeterPhase ++;
    }
}
