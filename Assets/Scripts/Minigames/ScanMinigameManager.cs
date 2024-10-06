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

    TextMeshProUGUI tmp;

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

    bool moving;
    bool paused;

    Image scanner;




    private void Start()
    {
        //Find the exclamation point
        tmp = GetComponentInChildren<TextMeshProUGUI>();
        tmp.text = "";

        //Find the scanner among other images
        Image[] images = GetComponentsInChildren<Image>();

        foreach (Image image in images)
        {
            if (image.transform.gameObject.CompareTag("Scanner"))
            {
                scanner = image;
                print("found!");
            }
        }

        //Set up the time
        SetCatchTime();
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
        Vector2 move = speed * Time.deltaTime * new Vector2(input.normalized.x, input.normalized.y);
        if (paused) move = Vector2.zero; //No movement while paused


        Vector3 pos = scanner.rectTransform.position;
        pos += (Vector3)move;

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
            print("times up");
            tmp.text = "!";

            //Player found the fish
            Find();
        }

        //While the fish is found 
        if (found && !(fishTimer < 0))
        {

            fishTimer -= Time.deltaTime;

            if (fishTimer < 0)
            {
                //Fish leaves
                FishFlee();
            }
        }

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
    void FishFlee()
    {
        tmp.text = "";
        SetCatchTime();
        found = false;
    }

    void Catch(InputAction.CallbackContext _)
    {
        print("pressed");
        if (!found) return;
        if (fishTimer < 0) return; 

        Debug.Log("Caught the fish!");

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

    private void OnDisable()
    {
        Controls.Confirm.performed -= Catch;
    }


}
