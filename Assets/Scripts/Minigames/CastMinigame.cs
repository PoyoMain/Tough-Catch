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

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [Space(5)]
    [SerializeField] private AudioClip ringSuccess1;
    [SerializeField] private AudioClip ringSuccess2;
    [SerializeField] private AudioClip ringSuccess3;
    [SerializeField] private AudioClip ringFail;

    [Header("Krey")]
    [SerializeField] private Animator kreyAnim;

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
    private bool coroutinePlaying = false;

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
        kreyAnim.speed = 1;


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

        kreyAnim.SetTrigger("CastStart");
        kreyAnim.speed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!active) return;
        if (coroutinePlaying) return;

        if (targetRing != null && TargetRingScale.x < maxScale)
        {
            // Increase the scale over time at the specified rate
            float newScale = TargetRingScale.x + ScaleRate * Time.deltaTime;
            newScale = Mathf.Min(newScale, maxScale); // Limit the scale to the maxScale value

            // Apply the new scale uniformly in all directions
            targetRing.transform.localScale = new Vector3(newScale, newScale, newScale);

            //Set Krey's Animation
            kreyAnim.Play("Krey_Cast", 0, (newScale/ring6.transform.localScale.x));

            if (ring1.transform.localScale.x < TargetRingScale.x && TargetRingScale.x < ring2.transform.localScale.x && !CompRing1) buttonPromptImage.enabled = true;
            else if (ring3.transform.localScale.x < TargetRingScale.x && TargetRingScale.x < ring4.transform.localScale.x && !CompRing2) buttonPromptImage.enabled = true;
            else if (ring5.transform.localScale.x < TargetRingScale.x && TargetRingScale.x < ring6.transform.localScale.x && !CompRing3) buttonPromptImage.enabled = true;
            else buttonPromptImage.enabled = false;
        }

        if ((ring2.transform.localScale.x < targetRing.transform.localScale.x && !CompRing1 && !isFlashing1))
        {
            isFlashing1 = true;
            audioSource.PlayOneShot(ringFail);
            kreyAnim.speed = -1;
        }
        else if ((ring4.transform.localScale.x < targetRing.transform.localScale.x && !CompRing2 && !isFlashing2))
        {
            isFlashing2 = true;
            audioSource.PlayOneShot(ringFail);
            kreyAnim.speed = -1;
        }
        else if (ring6.transform.localScale.x < targetRing.transform.localScale.x && !CompRing3 && !isFlashing3)
        {
            isFlashing3 = true;
            audioSource.PlayOneShot(ringFail);
            kreyAnim.speed = -1;
        }

        if (isFlashing1)
        {
            isFlashing1 = false;
            StartCoroutine(FlashRedForOneSecond(targetRenderer1.GetComponent<Animator>()));
            MinigameDone = true;
        }
        else if (isFlashing2)
        {
            isFlashing2 = false;
            StartCoroutine(FlashRedForOneSecond(targetRenderer2.GetComponent<Animator>()));
            MinigameDone = true;
        }
        else if (isFlashing3)
        {
            isFlashing3 = false;
            StartCoroutine(FlashRedForOneSecond(targetRenderer3.GetComponent<Animator>()));
            MinigameDone = true;
        }
    }

    private void SizeCheck(InputAction.CallbackContext Context)
    {
        if (Options.IsPaused) return;

        if (targetRing.transform.localScale.x < ring1.transform.localScale.x)
        {
            isFlashing1 = true;
            isFlashing3 = true;
            isFlashing2 = true;
        }


        if(ring1.transform.localScale.x < targetRing.transform.localScale.x && targetRing.transform.localScale.x < ring2.transform.localScale.x)
        {
            CompRing1 = true;
            targetRenderer1.GetComponent<Animator>().SetTrigger("Correct");
            audioSource.PlayOneShot(ringSuccess1);
        }

        if ((ring2.transform.localScale.x < targetRing.transform.localScale.x && targetRing.transform.localScale.x < ring3.transform.localScale.x))
        {
            isFlashing1 = true;
            audioSource.PlayOneShot(ringFail);
        }

        if (ring3.transform.localScale.x < targetRing.transform.localScale.x && targetRing.transform.localScale.x < ring4.transform.localScale.x)
        {
            CompRing2 = true;
            targetRenderer2.GetComponent<Animator>().SetTrigger("Correct");
            audioSource.PlayOneShot(ringSuccess2);
        }

        if ((ring4.transform.localScale.x < targetRing.transform.localScale.x && targetRing.transform.localScale.x < ring5.transform.localScale.x))
        {
            isFlashing2 = true;
            audioSource.PlayOneShot(ringFail);
        }


        if (ring5.transform.localScale.x < targetRing.transform.localScale.x && targetRing.transform.localScale.x < ring6.transform.localScale.x)
        {
            CompRing3 = true;
            targetRenderer3.GetComponent<Animator>().SetTrigger("Correct");
            MinigameDone = true;
            Win = true;
            active = false;
            audioSource.PlayOneShot(ringSuccess3);
        }

        return;
    }

    // Coroutine to flash the color red for the specified duration
    private IEnumerator FlashRedForOneSecond(Animator target)
     {
        if (target == null) yield break;
        if (target.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) yield break;

        coroutinePlaying = true;
        target.SetTrigger("Flash");

        float stoppedTime = kreyAnim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        kreyAnim.Play("Krey_CastReverse", 0, 1 - stoppedTime);
        kreyAnim.speed = 1;

        yield return 0;
        while (target.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) yield return null;

        while (kreyAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) yield return null;

        targetRing.transform.localScale = Vector3.zero;
        targetRenderer1.color = originalColor;
        targetRenderer2.color = originalColor;
        targetRenderer3.color = originalColor;
        targetRenderer1.GetComponent<Animator>().SetTrigger("Reset");
        targetRenderer2.GetComponent<Animator>().SetTrigger("Reset");
        targetRenderer3.GetComponent<Animator>().SetTrigger("Reset");
        CompRing1 = false;
        CompRing2 = false;
        CompRing3 = false;
        MinigameDone = false;

        kreyAnim.SetTrigger("CastStart");
        //kreyAnim.speed = 0;

        coroutinePlaying = false;

        yield break;  
    }
}
