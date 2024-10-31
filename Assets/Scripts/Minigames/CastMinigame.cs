using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CastMinigame : MinigameBase
{
    public bool Win = false;
    public bool CompRing1;
    public bool CompRing2;
    public bool CompRing3;
    public GameObject targetRing;
    public GameObject ring1;
    public GameObject ring2;
    public GameObject ring3;
    public GameObject ring4;
    public GameObject ring5;
    public GameObject ring6;
    public float scaleRate = 0.5f;
    private float maxScale = 5f;
    public float flashDuration = 2.0f;
    public float flashInterval = 0.25f;
    private Color originalColor;
    private Image targetRenderer1;
    private Image targetRenderer2;
    private Image targetRenderer3;
    public bool isFlashing1 = false;
    public bool isFlashing2 = false;
    public bool isFlashing3 = false;
    private bool MinigameDone = false;

    [Header("Set in Inspector Objects")]
    [SerializeField] private Image buttonPromptImage;
    [SerializeField] private Animator castStartLettering;
    [SerializeField] private Animator castSucceedLettering;

    [Header("Keyboard Button Sprites")]
    [SerializeField] private Sprite confirmButton_Keyboard;

    [Header("Controller Button Sprites")]
    [SerializeField] private Sprite confirmButton_Controller;

    private Sprite ConfirmButtonSprite
    {
        get => Options.ControllerConnected ? confirmButton_Controller : confirmButton_Keyboard;
    }
    private Vector3 TargetRingScale => targetRing.transform.localScale;
    private float ScaleRate
    {
        get => _useOptionValues ? Options.CastMinigameOptions.scaleRate : scaleRate;
    }

    private bool active = false;

    protected override void OnEnable()
    {
        base.OnEnable();

        buttonPromptImage.sprite = ConfirmButtonSprite;

        Controls.HookRingSelect.performed += SizeCheck;

    }
    private void OnDisable()
    {
        Controls.HookRingSelect.performed -= SizeCheck;

    }

    private IEnumerator TextDisplayCoroutine()
    {

            while (castStartLettering.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) yield return null;

            castStartLettering.gameObject.SetActive(false);
        active = true;

            while (active == true) yield return null;

            castSucceedLettering.gameObject.SetActive(true);

            while (castSucceedLettering.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) yield return null;

            castSucceedLettering.gameObject.SetActive(false);

            MinigameSuccessEvent.RaiseEvent();
            this.enabled = false;

        yield break;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (targetRing != null)
        {
            targetRing.transform.localScale = Vector3.zero; // Starting at scale 1
        }
        if (ring2 != null)
        {
            targetRenderer1 = ring2.GetComponent<Image>();
            if (targetRenderer1 != null)
            {
                originalColor = targetRenderer1.color;
            }
        }
        if (ring4 != null)
        {
            targetRenderer2 = ring4.GetComponent<Image>();

        }
        if (ring6 != null)
        {
            targetRenderer3 = ring6.GetComponent<Image>();

        }

        castStartLettering.gameObject.SetActive(true);
        StartCoroutine(nameof(TextDisplayCoroutine));
    }

    // Update is called once per frame
    void Update()
    {
        if (!active) return;

        if (targetRing != null && targetRing.transform.localScale.x < maxScale && MinigameDone == false)
        {
            // Increase the scale over time at the specified rate
            float newScale = targetRing.transform.localScale.x + ScaleRate * Time.deltaTime;
            newScale = Mathf.Min(newScale, maxScale); // Limit the scale to the maxScale value

            // Apply the new scale uniformly in all directions
            targetRing.transform.localScale = new Vector3(newScale, newScale, newScale);

            if (ring1.transform.localScale.x < TargetRingScale.x && TargetRingScale.x < ring2.transform.localScale.x) buttonPromptImage.enabled = true;
            else if (ring3.transform.localScale.x < TargetRingScale.x && TargetRingScale.x < ring4.transform.localScale.x) buttonPromptImage.enabled = true;
            else if (ring5.transform.localScale.x < TargetRingScale.x && TargetRingScale.x < ring6.transform.localScale.x) buttonPromptImage.enabled = true;
            else buttonPromptImage.enabled = false;
        }

        if(isFlashing1)
        {
            StartCoroutine(FlashRedForOneSecond(targetRenderer1, isFlashing1));
            isFlashing1 = false;
            MinigameDone = true;
        }
        if (isFlashing2)
        {
            StartCoroutine(FlashRedForOneSecond(targetRenderer2, isFlashing2));
            isFlashing2 = false;
            MinigameDone = true;
        }
        if (isFlashing3)
        {
            StartCoroutine(FlashRedForOneSecond(targetRenderer3, isFlashing3));
            isFlashing3 = false;
            MinigameDone = true;
        }

        if ((ring2.transform.localScale.x < targetRing.transform.localScale.x && CompRing1 == false))
        {
            isFlashing1 = true;
        }

        if ((ring4.transform.localScale.x < targetRing.transform.localScale.x && CompRing2 == false))
        {
            isFlashing2 = true;
        }
        if (ring6.transform.localScale.x < targetRing.transform.localScale.x && CompRing3 == false)
        {
            isFlashing3 = true;
        }

    }

    private void SizeCheck(InputAction.CallbackContext Context)
    {
        if (targetRing.transform.localScale.x < ring1.transform.localScale.x)
        {
            isFlashing1 = true;
            isFlashing3 = true;
            isFlashing2 = true;
        }


        if(ring1.transform.localScale.x < targetRing.transform.localScale.x && targetRing.transform.localScale.x < ring2.transform.localScale.x)
        {
            CompRing1 = true;
            targetRenderer1.color = Color.green;
        }

        if ((ring2.transform.localScale.x < targetRing.transform.localScale.x && targetRing.transform.localScale.x < ring3.transform.localScale.x))
        {
            isFlashing1 = true;
        }

        if (ring3.transform.localScale.x < targetRing.transform.localScale.x && targetRing.transform.localScale.x < ring4.transform.localScale.x)
        {
            CompRing2 = true;
            targetRenderer2.color = Color.green;
        }

        if ((ring4.transform.localScale.x < targetRing.transform.localScale.x && targetRing.transform.localScale.x < ring5.transform.localScale.x))
        {
            isFlashing2 = true;
        }


        if (ring5.transform.localScale.x < targetRing.transform.localScale.x && targetRing.transform.localScale.x < ring6.transform.localScale.x)
        {
            CompRing3 = true;
            targetRenderer3.color = Color.green;
            MinigameDone = true;
            Win = true;
            active = false;
        }

        return;
    }

    // Coroutine to flash the color red for the specified duration
    private IEnumerator FlashRedForOneSecond(Image target, bool isFlashing)
    {
        float elapsedTime = 0f;

        while (elapsedTime < flashDuration && isFlashing)
        {
            if (target != null)
            {
                // Change color to red
                target.color = Color.red;

                // Wait for the flash interval
                yield return new WaitForSeconds(flashInterval);

                // Change back to original color
                target.color = originalColor;

                // Wait for the same interval before switching again
                yield return new WaitForSeconds(flashInterval);

                // Update the elapsed time (each flash cycle is 2 * flashInterval)
                elapsedTime += 2 * flashInterval;
            }
        }
        isFlashing = false;
        target.color = originalColor;
        targetRing.transform.localScale = Vector3.zero;
        targetRenderer1.color = originalColor;
        targetRenderer2.color = originalColor;
        targetRenderer3.color = originalColor;
        CompRing1 = false;
        CompRing2 = false;
        CompRing3 = false;
        MinigameDone = false;

        yield return null;

       
    }
    

}
