using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class TutorialPanel : MonoBehaviour
{
    [Header("Inspector Objects")]
    [SerializeField] private InputReaderSO inputReader;
    [SerializeField] private OptionsSO options;
    [Space(10)]
    [SerializeField] private VideoPlayer vidPlayer;
    [Space(5)]
    [SerializeField] private VideoClip keyboardVideoClip;
    [SerializeField] private VideoClip controllerVideoClip;


    [Header("Broadcast Events")]
    [SerializeField] private VoidEventChannelSO endEventSO;

    private bool active = false;

    private void OnEnable()
    {
        inputReader.Controls.Confirm.performed += Deactivate;

        InputSystem.onDeviceChange += ChangeVideo;

        Activate();
    }

    private void OnDisable()
    {
        inputReader.Controls.Confirm.performed -= Deactivate;

        InputSystem.onDeviceChange -= ChangeVideo;
    }

    private void Activate()
    {
        if (active) return;
        active = true;

        Time.timeScale = 0;
        ChangeVideo();
    }

    private void Deactivate(InputAction.CallbackContext _)
    {
        if (!active) return;

        Time.timeScale = 1;

        endEventSO.RaiseEvent();

        this.enabled = false;
    }

    private void ChangeVideo(InputDevice device, InputDeviceChange change)
    {
        if (options.ControllerConnected) vidPlayer.clip = controllerVideoClip;
        else vidPlayer.clip = keyboardVideoClip;

        vidPlayer.Play();
    }

    private void ChangeVideo()
    {
        if (options.ControllerConnected) vidPlayer.clip = controllerVideoClip;
        else vidPlayer.clip = keyboardVideoClip;

        vidPlayer.Play();
    }
}
