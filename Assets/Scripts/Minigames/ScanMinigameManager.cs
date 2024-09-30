using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    ScannerMove scanner;
    TextMeshProUGUI tmp;

    [Header("Fish")]
    [SerializeField] private FishEventChannelSO FishFoundEvent;
    [SerializeField] List<FishSO> fishList;



    private void Start()
    {
        scanner = ScannerMove.GET_SCANNER(); // Must happen after Awake() is called for the ScannerMove class
        tmp = scanner.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        tmp.text = "";
        SetCatchTime();
    }

    private void Update()
    {
        //Run the timer while the scanner is moving and the fish is not found
        if (!found && scanner.moving)
        {
            timeUntilCatch -= Time.deltaTime;
        }

        //Timer can only go off if fish is not found
        if (!found && timeUntilCatch < 0)
        {
            //Stop the movement and update the exclamation text
            scanner.Pause();
            print("times up");
            tmp.text = "!";

            //Player found the fish
            Find();
        }

        //While the fish is found 
        if (found && !(fishTimer < 0))
        {
            if (Controls.Confirm.IsPressed() || Input.GetKeyDown(KeyCode.Space))
            {
                //Caught the fish
                Catch();
            }

            fishTimer -= Time.deltaTime;

            if (fishTimer < 0)
            {
                //Fish leaves
                fishFlee();
            }
        }
    }

    void SetCatchTime()
    {
        timeUntilCatch = Random.Range(minTime, maxTime);
    }

    void Find()
    {
        found = true;
        fishTimer = maxFishTime;
    }

    //Resets the minigame after the player fails to confirm
    void fishFlee()
    {
        tmp.text = "";
        scanner.Unpause();
        SetCatchTime();
        found = false;
    }

    void Catch()
    {
        Debug.Log("Caught the fish!");

        FishSO selected;

        //Selects random fish from the serialized list
        int index = Random.Range(0, fishList.Count);
        selected = fishList[index];
        

        //Raises FishFoundEvent with the random fish
        FishFoundEvent.RaiseEvent(new Fish(selected));
        
    }


}
