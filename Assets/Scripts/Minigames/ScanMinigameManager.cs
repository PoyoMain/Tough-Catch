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

    [Header("Fish")]
    [SerializeField] private FishEventChannelSO FishFoundEvent;
    [SerializeField] List<FishSO> fishList;

    [Header("Scanner Movement")]
    [SerializeField] float speed;

    [Header("Bounds")]
    [SerializeField] float xMax;
    [SerializeField] float xMin;
    [SerializeField] float yMax;
    [SerializeField] float yMin;

    [Header("Elements")]
    [SerializeField] Animator animator;
    [SerializeField] Image scanner;
    [SerializeField] TextMeshProUGUI scannerTMP;

    bool moving;
    bool paused;
    bool caught;

    private void Start()
    {
        Pause();

        //Set up the time
        StartCoroutine(nameof(InitCatchTime));

        caught = false;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        Controls.Confirm.performed += Catch;
    }

    private void Update()
    {
        //Handle Input
        Vector2 input = Controls.MoveScanner.ReadValue<Vector2>();
        if (input.magnitude > 0) moving = true; else moving = false;

        //Calculate move
        Vector2 move = new Vector2(input.normalized.x, input.normalized.y) * speed * Time.deltaTime;
        if (paused) move = Vector2.zero; //No movement while paused


        Vector3 pos = scanner.rectTransform.position;
        pos += new Vector3(move.x, move.y, 0);

        //Check bounds
        if (pos.x > xMax) pos.x = xMax;
        if (pos.y > yMax) pos.y = yMax;
        if (pos.x < xMin) pos.x = xMin;
        if (pos.y < yMin) pos.y = yMin;

        scanner.rectTransform.position = pos;


        //Run the timer while the scanner is moving and the fish is not found
        if (!found && moving)
        {
            timeUntilCatch -= Time.deltaTime;
        }

        //Timer can only go off if fish is not found
        if (!found && timeUntilCatch < 0)
        {
            //Stop the movement and update the exclamation text
            Pause();

            //print("times up");

            scannerTMP.text = "!";

            //Player found the fish
            Find();
        }

        //While the fish is found 
        if (found && !(fishTimer < 0))
        {

            fishTimer -= Time.deltaTime;

            if (fishTimer <= 0)
            {
                //print("fleeing");

                //Fish leaves
                fishFlee();
            }
        }

    }

    IEnumerator InitCatchTime()
    {
        animator.SetTrigger("scanTrigger");

        yield return new WaitForSeconds(2f); // Wait for scan to go away and game to start

        SetCatchTime();
    }

    void SetCatchTime()
    {
        timeUntilCatch = Random.Range(minTime, maxTime);
        Unpause();
    }

    void Find()
    {
        found = true;
        fishTimer = maxFishTime;
    }

    //Resets the minigame after the player fails to confirm
    void fishFlee()
    {
        scannerTMP.text = "";
        SetCatchTime();
        found = false;
    }

    void Catch(InputAction.CallbackContext _)
    {
        //print("pressed");

        if (!found) return;
        if (caught) return;
        if (fishTimer < 0) return;

        caught = true;

        //Debug.Log("Caught the fish!");

        animator.SetTrigger("successTrigger");

        Invoke(nameof(Success), 2f);

    }

    void Success()
    {
        FishSO selected;

        //Selects random fish from the serialized list
        int index = Random.Range(0, fishList.Count);
        selected = fishList[index];

        //Raises FishFoundEvent with the random fish
        FishFoundEvent.RaiseEvent(new Fish(selected));

        //Succeeds at the minigame
        _minigameSuccess.RaiseEvent();
    }

    /// <summary>
    /// Pauses the movement of the scanner
    /// </summary>
    public void Pause()
    {
        paused = true;
    }

    /// <summary>
    /// Unpauses the movement of the scanner
    /// </summary>
    public void Unpause()
    {
        paused = false;
    }

    private void OnDestroy()
    {
        Controls.Confirm.performed -= Catch;
    }


}
