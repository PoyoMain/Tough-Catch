using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanTutorialManager : MonoBehaviour
{   
    public GameObject scanTutorial;
    public void start()
    {
        Time.timeScale = 0f;
    }
   

    public void Disable()
    {
        scanTutorial.SetActive(false);
        Time.timeScale = 1f;
        Debug.Log("Time Scale is now 1");
    }
    
}
