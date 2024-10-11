using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class ReelMinigame : MinigameBase
    
{
    //grabs the image and slider UI elements for button to press, and the reel meter
    public UnityEngine.UI.Image buttonFrame;
    public UnityEngine.UI.Slider reelMeter;

    //holds image sprites for which button to mash
    //***SET ORDER OF SPRITES IN UP, DOWN LEFT, RIGHT FOR PROPER FUNCTION***
    public Sprite[] buttonSpriteList = new Sprite[4];
    private Sprite buttonSprite;

    //quantity of how much meter changes from button press, or from decreasing overtime
    public float reelStrength, meterDecay;

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

    }

    // Update is called once per frame
    void Update()
    {
        //meter constantly decreases until it becomes full
        if (reelMeter.value < 1) {
            reelMeter.value -= meterDecay;
        }
        
        PhaseChange();

    }

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


    void ReelCheck(InputAction.CallbackContext context)
    {
        var buttonValue = context.ReadValue<Vector2>();
        //When player presses an input, a check is conducted for the right or wrong input and changes meter accordingly
        if (buttonValue == Vector2.up)
        {
            if (buttonFrame.sprite == buttonSpriteList[0])
            {
                reelMeter.value += reelStrength;
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
            }
            else
            {
                reelMeter.value -= reelStrength;
            }
        }
    }


}
