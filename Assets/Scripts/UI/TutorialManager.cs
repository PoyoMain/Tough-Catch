using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial Panels")]
    public GameObject scanPanel;
    public GameObject castPanel;
    public GameObject tugglePanel;
    public GameObject reelPanel;

    [Header("Options")]
    public OptionsSO options;

    [Header("ListenEvent")]
    [SerializeField] private VoidEventChannelSO ScanCastEventSO;
    [SerializeField] private VoidEventChannelSO TuggleStartEventSO;
    [SerializeField] private VoidEventChannelSO ReelStartEventSO;
    [SerializeField] private VoidEventChannelSO CastStartEventSO;

    private void OnEnable()
    {
        ScanCastEventSO.OnEventRaised += ShowScanPanel;
        TuggleStartEventSO.OnEventRaised += ShowTugglePanel;
        ReelStartEventSO.OnEventRaised += ShowReelPanel;
        CastStartEventSO.OnEventRaised += ShowCastPanel;
    }

    private void OnDisable()
    {
        ScanCastEventSO.OnEventRaised -= ShowScanPanel;
        TuggleStartEventSO.OnEventRaised -= ShowTugglePanel;
        ReelStartEventSO.OnEventRaised -= ShowReelPanel;
        CastStartEventSO.OnEventRaised -= ShowCastPanel;
    }

    private void ShowScanPanel()
    {   
        if(options._tutorialPopUps == true)
        {
            scanPanel.SetActive(true);
        }
   
    }
    private void ShowTugglePanel()
    {
        tugglePanel.SetActive(true);
    }
    private void ShowReelPanel()
    {
        reelPanel.SetActive(true);
    }
    private void ShowCastPanel()
    {
        castPanel.SetActive(true);
    }



}
