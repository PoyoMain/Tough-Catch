using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConceptNavigation : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private List<CinemachineVirtualCamera> cams;
    private int camIndex = 0;

    private CinemachineVirtualCamera ActiveCam
    {
        get => cams[camIndex];
    }

    private void Start()
    {
        SetCameraActiveness();
    }

    public void GoLeft()
    {
        if (camIndex > 0)
        {
            camIndex--;
            SetCameraActiveness();
        }
    }

    public void GoRight()
    {
        if (camIndex < cams.Count - 1)
        {
            camIndex++;
            SetCameraActiveness();
        }
    }

    public void SetCameraActiveness()
    {
        foreach (var cam in cams)
        {
            if (cam ==  ActiveCam) cam.gameObject.SetActive(true);
            else cam.gameObject.SetActive(false);
        }
    }
}
