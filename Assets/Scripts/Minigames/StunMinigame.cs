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

    [Header("Images")]
    [SerializeField] private Sprite upButtonSelected;
    [SerializeField] private Sprite downButtonSelected;
    [SerializeField] private Sprite leftButtonSelected;
    [SerializeField] private Sprite rightButtonSelected;
    [Space(5)]
    [SerializeField] private Sprite upButtonUnselected;
    [SerializeField] private Sprite downButtonUnselected;
    [SerializeField] private Sprite leftButtonUnselected;
    [SerializeField] private Sprite rightButtonUnselected;

    [Header("Broadcast Events")]
    [SerializeField] private VoidEventChannelSO damageFishSO;

    private FaceButton[] directionCombo;
    private int comboIndex = 0;
    private float minigameCompleteTimer;

    private FaceButton ActiveButton => directionCombo[comboIndex];


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
        stunPanel.SetActive(false);

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
                0 => new(img, leftButtonSelected, leftButtonUnselected, FaceButtonDirection.Left),
                1 => new(img, rightButtonSelected, rightButtonUnselected, FaceButtonDirection.Right),
                2 => new(img, upButtonSelected, upButtonUnselected, FaceButtonDirection.Up),
                3 => new(img, downButtonSelected, downButtonUnselected, FaceButtonDirection.Down),
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
        public FaceButtonDirection direction;

        public FaceButton(Image img, Sprite selected, Sprite unselected, FaceButtonDirection dir)
        {
            image = img;
            selectedSprite = selected;
            unselectedSprite = unselected;
            direction = dir;

            image.sprite = unselectedSprite;
        }

        public void Activate()
        {
            image.sprite = selectedSprite;
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
