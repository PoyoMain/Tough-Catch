using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StunMinigame : MinigameBase
{
    [Header("Settings")]
    [SerializeField] private int numberOfButtonsForCombo;
    [SerializeField] private float minigameCompleteTime;
    [SerializeField] private GameObject stunPanel;
    [SerializeField] private Transform imageParent;
    [SerializeField] private Image imagePrefab;
    [SerializeField] private Slider timeBar;

    [Header("Keyboard Images")]
    [SerializeField] private Sprite upButtonSelected_Keyboard;
    [SerializeField] private Sprite downButtonSelected_Keyboard;
    [SerializeField] private Sprite leftButtonSelected_Keyboard;
    [SerializeField] private Sprite rightButtonSelected_Keyboard;
    [Space(5)]
    [SerializeField] private Sprite upButtonUnselected_Keyboard;
    [SerializeField] private Sprite downButtonUnselected_Keyboard;
    [SerializeField] private Sprite leftButtonUnselected_Keyboard;
    [SerializeField] private Sprite rightButtonUnselected_Keyboard;
    [Space(5)]
    [SerializeField] private Sprite upButtonFailed_Keyboard;
    [SerializeField] private Sprite downButtonFailed_Keyboard;
    [SerializeField] private Sprite leftButtonFailed_Keyboard;
    [SerializeField] private Sprite rightButtonFailed_Keyboard;

    [Header("Controller Images")]
    [SerializeField] private Sprite upButtonSelected_Controller;
    [SerializeField] private Sprite downButtonSelected_Controller;
    [SerializeField] private Sprite leftButtonSelected_Controller;
    [SerializeField] private Sprite rightButtonSelected_Controller;
    [Space(5)]
    [SerializeField] private Sprite upButtonUnselected_Controller;
    [SerializeField] private Sprite downButtonUnselected_Controller;
    [SerializeField] private Sprite leftButtonUnselected_Controller;
    [SerializeField] private Sprite rightButtonUnselected_Controller;
    [Space(5)]
    [SerializeField] private Sprite upButtonFailed_Controller;
    [SerializeField] private Sprite downButtonFailed_Controller;
    [SerializeField] private Sprite leftButtonFailed_Controller;
    [SerializeField] private Sprite rightButtonFailed_Controller;

    [Header("Broadcast Events")]
    [SerializeField] private VoidEventChannelSO damageFishSO;

    private FaceButton[] directionCombo;
    private int comboIndex = 0;
    private float minigameCompleteTimer;

    private FaceButton ActiveButton => directionCombo[comboIndex];

    private Sprite UpButtonSelected 
    {
        get => ControllerConnected ? upButtonSelected_Controller : upButtonSelected_Keyboard;
    }
    private Sprite DownButtonSelected 
    {
        get => ControllerConnected ? downButtonSelected_Controller : downButtonSelected_Keyboard;
    }
    private Sprite LeftButtonSelected 
    {
        get => ControllerConnected ? leftButtonSelected_Controller : leftButtonSelected_Keyboard;
    }
    private Sprite RightButtonSelected 
    {
        get => ControllerConnected ? rightButtonSelected_Controller : rightButtonSelected_Keyboard;
    }

    private Sprite UpButtonUnselected 
    {
        get => ControllerConnected ? upButtonUnselected_Controller : upButtonUnselected_Keyboard;
    }
    private Sprite DownButtonUnselected 
    {
        get => ControllerConnected ? downButtonUnselected_Controller: downButtonUnselected_Keyboard;
    }
    private Sprite LeftButtonUnselected 
    {
        get => ControllerConnected ? leftButtonUnselected_Controller : leftButtonUnselected_Controller;
    }
    private Sprite RightButtonUnselected 
    {
        get => ControllerConnected ? rightButtonUnselected_Controller: rightButtonUnselected_Controller;
    }
    
    private Sprite UpButtonFailed 
    {
        get => ControllerConnected ? upButtonFailed_Controller : upButtonFailed_Keyboard;
    }
    private Sprite DownButtonFailed 
    {
        get => ControllerConnected ? downButtonFailed_Controller: downButtonFailed_Keyboard;
    }
    private Sprite LeftButtonFailed 
    {
        get => ControllerConnected ? leftButtonFailed_Controller : leftButtonFailed_Keyboard;
    }
    private Sprite RightButtonFailed 
    {
        get => ControllerConnected ? rightButtonFailed_Controller : rightButtonFailed_Keyboard;
    }


    protected override void OnEnable()
    {
        base.OnEnable();

        minigameCompleteTimer = timeBar.value = timeBar.maxValue = minigameCompleteTime;

        SetCombo();

        stunPanel.SetActive(true);
        Controls.StunPanelButtons.performed += HitButtonInCombo;
    }

    private void OnDisable()
    {
        if (stunPanel != null) stunPanel.SetActive(false);

        foreach (FaceButton button in directionCombo)
        {
            Destroy(button.image.gameObject);
        }
        Controls.StunPanelButtons.performed -= HitButtonInCombo;
    }

    private void Update()
    {
        if (minigameCompleteTimer > 0)
        {
            timeBar.value = minigameCompleteTimer -= Time.deltaTime;

            if (minigameCompleteTimer <= 0)
            {
                _minigameSuccess.RaiseEvent();
                this.enabled = false;
            }
        }
    }

    private void SetCombo()
    {
        FaceButton[] combo = new FaceButton[numberOfButtonsForCombo];

        for (int i = 0; i < numberOfButtonsForCombo; i++)
        {
            Image img = Instantiate(imagePrefab, imageParent);
            int choice = Random.Range(0, 4);

            FaceButton button = choice switch
            {
                0 => new(img, LeftButtonSelected, LeftButtonUnselected, LeftButtonFailed, FaceButtonDirection.Left),
                1 => new(img, RightButtonSelected, RightButtonUnselected, RightButtonFailed, FaceButtonDirection.Right),
                2 => new(img, UpButtonSelected, UpButtonUnselected, UpButtonFailed, FaceButtonDirection.Up),
                3 => new(img, DownButtonSelected, DownButtonUnselected, DownButtonFailed, FaceButtonDirection.Down),
                _ => throw new System.NotImplementedException(),
            };

            combo[i] = button;
        }

        directionCombo = combo;
        comboIndex = 0;
    }

    private void HitButtonInCombo(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        FaceButtonDirection inputDir = input switch
        {
            Vector2 when input.Equals(Vector2.left) => FaceButtonDirection.Left,
            Vector2 when input.Equals(Vector2.right) => FaceButtonDirection.Right,
            Vector2 when input.Equals(Vector2.up) => FaceButtonDirection.Up,
            Vector2 when input.Equals(Vector2.down) => FaceButtonDirection.Down,
            _ => throw new System.NotImplementedException(),
        };

        if (inputDir == ActiveButton.direction)
        {
            ActiveButton.Activate();

            if (comboIndex < numberOfButtonsForCombo - 1)
            {
                comboIndex++;
                return;
            }

            damageFishSO.RaiseEvent();
        }

        _minigameSuccess.RaiseEvent();
        this.enabled = false;
    }

    private class FaceButton
    {
        public Image image;
        public Sprite selectedSprite;
        public Sprite unselectedSprite;
        public Sprite failedSprite;
        public FaceButtonDirection direction;

        public FaceButton(Image img, Sprite selected, Sprite unselected, Sprite failed, FaceButtonDirection dir)
        {
            image = img;
            selectedSprite = selected;
            unselectedSprite = unselected;
            failedSprite = failed;
            direction = dir;

            image.sprite = unselectedSprite;
        }

        public void Activate()
        {
            image.sprite = selectedSprite;
        }

        public void Fail()
        {
            image.sprite = failedSprite;
        }
    }

    private enum FaceButtonDirection
    {
        Left,
        Right,
        Up,
        Down,
    }
}
