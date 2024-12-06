using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ScanMinigameManager : MinigameBase
{
    [Header("Catch Time")]
    [SerializeField] float minTime;
    [SerializeField] float maxTime;

    [Header("Confirm Time")]
    [SerializeField] float maxFishTime;

    [Header("Dynamic")]
    [SerializeField] float timeUntilCatch;
    [SerializeField] float fishTimer;
    [SerializeField] bool found;
    [SerializeField] bool caught;

    [Header("Fish")]
    [SerializeField] private FishEventChannelSO FishFoundEvent;
    [SerializeField] List<FishSO> fishList;
    FishSO selected;
    Fish caughtFish;

    [Header("Scanner Movement")]
    [SerializeField] float speed;
    [SerializeField] float shakeIntensity;
    [Range(0, 1)]
    [SerializeField] float shakeFrequency;

    [Header("Bounds")]
    [SerializeField] float xMax;
    [SerializeField] float xMin;
    [SerializeField] float yMax;
    [SerializeField] float yMin;

    [Header("Elements")]
    [SerializeField] Animator animator;
    [SerializeField] Image scanner;
    [SerializeField] Animator buttonAnim;
    
    //Added by Chris; Lettering animators
    [SerializeField] Animator scanStartLettering;
    [SerializeField] Animator scanSucceedLettering;

    //Added by Chris; Audio Source
    [SerializeField] AudioSource audioSouce;

    //Added by Chris; Star Gained Event for gigantic catches
    [SerializeField] VoidEventChannelSO starGainedEvent;

    //Added by Chris; Properties
    float MinTime
    {
        get => _useOptionValues ? Options.ScanMinigameOptions.minTime : minTime;
    }
    float MaxTime
    {
        get => _useOptionValues ? Options.ScanMinigameOptions.maxTime : maxTime;
    }
    float MaxFishTime
    {
        get => _useOptionValues ? Options.ScanMinigameOptions.maxConfirmTime : maxFishTime;
    }

    bool active; //Added by Chris
    bool moving;
    bool paused;

    Vector2 foundPosition;

    private void Start()
    {
        scanStartLettering.gameObject.SetActive(true);
        StartCoroutine(nameof(TextDisplayCoroutine));
    }

    //Added by Chris; Moved start into Activate
    public override void Activate()
    {
        Pause();

        //Set up the time
        StartCoroutine(nameof(InitCatchTime));

        active = true; // Added by Chris
        caught = false;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        Controls.Confirm.performed += Catch;
    }

    //Added by Chris
    private IEnumerator TextDisplayCoroutine()
    {

        while (scanStartLettering.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) yield return null;

        scanStartLettering.gameObject.SetActive(false);
        Activate();

        while (active == true) yield return null;

        scanSucceedLettering.gameObject.SetActive(true);

        while (scanSucceedLettering.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) yield return null;

        scanSucceedLettering.gameObject.SetActive(false);

        MinigameSuccessEvent.RaiseEvent();
        this.enabled = false;

        yield break;
    }

    private void Update()
    {
        if (!active) return;
        if (found) return; // While the fish is found logic will be handled in a coroutine

        //Handle Input
        Vector2 input = Controls.MoveScanner.ReadValue<Vector2>();
        if (input.magnitude > 0) moving = true; else moving = false;

        //Calculate move
        Vector2 move = new Vector2(input.normalized.x, input.normalized.y) * speed * Time.deltaTime;

        if (paused)
        {
            //No movement while paused
            move = Vector2.zero; 
            moving = false;
        }

        Vector3 pos = scanner.rectTransform.position;
        pos += new Vector3(move.x, move.y, 0);

        //Check bounds
        if (pos.x > xMax) pos.x = xMax;
        if (pos.y > yMax) pos.y = yMax;
        if (pos.x < xMin) pos.x = xMin;
        if (pos.y < yMin) pos.y = yMin;

        scanner.rectTransform.position = pos;


        //Run the timer while the scanner is moving and the fish is not found
        if (moving)
        {
            timeUntilCatch -= Time.deltaTime;
        }

        //Timer can only go off if fish is not found
        if (timeUntilCatch < 0)
        {
            //Stop the movement and update the exclamation text
            Pause();

            // Detect controller or keyboard and adjust prompt accordingly
            if (Gamepad.all.Count == 0)
            {
                buttonAnim.SetFloat("buttonType", -1);
            }
            else
            {
                buttonAnim.SetFloat("buttonType", 1);
            }

            // Show neutral button prompt
            buttonAnim.SetFloat("promptCorrectness", 0);
            buttonAnim.SetBool("ShowPrompt", true);
            

            //Player found the fish
            Find();
        }

    }

    IEnumerator InitCatchTime()
    {
        //animator.SetTrigger("scanTrigger");

        //yield return new WaitForSeconds(2f); // Wait for scan to go away and game to start

        SetCatchTime();

        yield break;
    }

    void SetCatchTime()
    {
        timeUntilCatch = Random.Range(MinTime, MaxTime);
        Unpause();
    }

    void Find()
    {
        found = true;
        fishTimer = MaxFishTime;

        //Selects random fish from the serialized list
        int index = Random.Range(0, fishList.Count);
        selected = fishList[index];

        // Create the fish from fishSO
        caughtFish = new Fish(selected);

        foundPosition = new Vector2(scanner.rectTransform.position.x, scanner.rectTransform.position.y); // This value will be used to make shake position changes relative to the original scanner position

        StartCoroutine(nameof(foundFish));
    }

    IEnumerator foundFish()
    {
        // Ensure prompt will not move with shake
        buttonAnim.gameObject.transform.SetParent(scanner.gameObject.transform.parent);

        print(caughtFish.Name + " " + caughtFish.Weight);

        while (fishTimer > 0 && !caught)
        {
            fishTimer -= Time.deltaTime;

            float randFreq = Random.Range(0f, .99f);

            // Scanner position will only change on some frames depending on shake frequency
            if (randFreq < shakeFrequency) 
            {
                int randInt = Random.Range(0, 4);
                Vector3 pos = scanner.rectTransform.position;

                pos = foundPosition;

                // Scanner position will change in a random direction by an amount dependent on weight and shake intensity
                switch (randInt)
                {
                    case 0:
                        pos.x += caughtFish.Weight * shakeIntensity;
                        break;
                    case 1:
                        pos.x -= caughtFish.Weight * shakeIntensity;
                        break;
                    case 2:
                        pos.y += caughtFish.Weight * shakeIntensity;
                        break;
                    case 3:
                        pos.y -= caughtFish.Weight * shakeIntensity;
                        break;
                }

                scanner.rectTransform.position = pos;
            }

            yield return null;
        }

        scanner.rectTransform.position = foundPosition;

        // Rebind prompt to scanner
        buttonAnim.gameObject.transform.SetParent(scanner.gameObject.transform);

        // If time is up and the fish has not been caught it will flee
        if (!caught)
        {
            fishFlee();
        }
        
    }

    //Resets the minigame after the player fails to confirm
    void fishFlee()
    {
        // Fish is gone
        selected = null;
        caughtFish = null;

        // Reset
        SetCatchTime();
        found = false;

        // Only call failure prompt if the fish was not caught
        if (!caught)
        {
            Pause();
            ShowPromptFeedback(false);
            Invoke(nameof(Unpause), 1);
        }

        caught = false;
    }

    void ShowPromptFeedback(bool correct)
    {
        // Adjust button prompt color to account for success or failure
        if (correct)
        {
            buttonAnim.SetFloat("promptCorrectness", 1);
        }
        else
        {
            buttonAnim.SetFloat("promptCorrectness", -1);
        }

        // Fade after a second
        Invoke(nameof(PromptFade), 1f);
        
    }

    // Call for button prompt fading after feedback has been shown
    private void PromptFade()
    {
        buttonAnim.SetBool("ShowPrompt", false);
    }

    void Catch(InputAction.CallbackContext _)
    {
        if (Options.IsPaused) return;

        if (!found) return;
        if (caught) return;
        if (fishTimer < 0) return;

        caught = true;

        // handle success animations
        //animator.SetTrigger("successTrigger");
        ShowPromptFeedback(true);

        Invoke(nameof(Success), 2f);

    }

    void Success()
    {

        //Raises FishFoundEvent with the random fish
        FishFoundEvent.RaiseEvent(caughtFish);

        //Succeeds at the minigame
        active = false;
        audioSouce.Stop();

        if (caughtFish.Weight >= 100) starGainedEvent.RaiseEvent();

        fishFlee(); // Reset
    }

    /// <summary>
    /// Pauses the movement of the scanner and scanner audio
    /// </summary>
    public void Pause()
    {
        paused = true;
        audioSouce.Pause();
    }

    /// <summary>
    /// Unpauses the movement of the scanner and scanner audio
    /// </summary>
    public void Unpause()
    {
        paused = false;
        audioSouce.UnPause();
    }

    private void OnDestroy()
    {
        Controls.Confirm.performed -= Catch;
    }


}
