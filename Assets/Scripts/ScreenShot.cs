using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShot : MonoBehaviour
{
    [SerializeField] private string filename = "image.png";
    [SerializeField] private int size = 1;
    [SerializeField] private InputReaderSO input;

    
    void Update()
    {
        if (input.Controls.Confirm.WasPerformedThisFrame())
        {
            ScreenCapture.CaptureScreenshot("Screenshots/" + filename + ".png", size);
            print("Screenshot taken. Located at " + "Screenshots/" + filename + ".png");
        }
    }
}
